---
id: api/configuration
description: Config file layout, env-var conventions, override precedence, and feature flag list
verified: 2026-07-24
check: Config keys and env var names match appsettings.json and module config files; Cors.json and ForwardedHeaders.json present
also_update:
  - api/modules/service
  - api/modules/diagnostics
  - api/modules
---

# Configuration

> Note: sourced from docs site (pre-v4). Verify if this changes.

## Config File Tree

All config files live under `/app/Config_Files` in the container.

```text
app
└── Config_Files
    ├── Presets.json                             required — app fails to start without this
    ├── Cors.json                                optional — CORS allowed origins (core API, not a module)
    ├── Cors.{Environment}.json                  optional override
    ├── ForwardedHeaders.json                    optional — proxy trust for resolving the caller's address
    ├── ForwardedHeaders.{Environment}.json      optional override
    ├── appsettings.json                         optional — host settings (e.g. AllowedHosts)
    ├── DiagnosticsModule
    │   ├── HealthChecks.json
    │   ├── HealthChecks.{Environment}.json      optional override
    │   ├── OpenTelemetry.json
    │   ├── OpenTelemetry.{Environment}.json     optional override
    │   ├── PackingLogs.json
    │   ├── PackingLogs.{Environment}.json       optional override
    │   ├── Serilog.json
    │   └── Serilog.{Environment}.json           optional override
    ├── ServiceModule
    │   ├── ConnectionStrings.json               optional — DB connection strings
    │   ├── RateLimiter.json                     required when SERVICE_MODULE=True — rate limiter rules
    │   └── JwtAuth.json                         optional — JWT issuer, audience, secret
    └── UiModule
        └── ConnectionStrings.json               optional — override BinacleApi connection string
```

Each file can be overridden by a `.{EnvironmentName}.json` sibling (any ASP.NET environment name:
Development, Production, Staging, etc.). Only include the keys you want to change.
Bind-mount individual files in Docker with `-v $(pwd)/Presets.json:/app/Config_Files/Presets.json:ro`.

> **Note:** the `Cors.json` and `ServiceModule` base files (`ConnectionStrings.json`, `JwtAuth.json`) are not
> committed to the repo — only their `.Development.json` variants ship. The base files are supplied at deploy time.
> `RateLimiter.json` does ship a base file.

### CORS (`Cors.json`)

Bound to `CorsOptions` (section `Cors`), loaded for the core API (`Program.cs`, not a module). Defines a single
named policy `CoreApi` that endpoints opt into with `.RequireCors(CorsPolicy.CoreApi)`:

```json
{
  "Cors": {
    "CoreApi": { "AllowedOrigins": ["https://example.com"] }
  }
}
```

### Forwarded headers (`ForwardedHeaders.json`)

Bound to `ForwardedHeadersConfigurationOptions` (section `ForwardedHeaders`), loaded for the core API in
`Program.cs` via `builder.ConfigureForwardedHeaders()` (`ExtensionMethods/ForwardedHeadersExtensions.cs`).
Ships **disabled**.

```json
{
  "ForwardedHeaders": {
    "Enabled": false,
    "TrustLoopback": true,
    "TrustPrivateNetworks": true,
    "TrustedProxies": [],
    "ForwardLimit": 1,
    "ForwardedForHeaderName": null
  }
}
```

When enabled, `UseForwardedHeaders()` rewrites `Connection.RemoteIpAddress` and `Request.Scheme` from
`X-Forwarded-For` / `X-Forwarded-Proto` before anything reads them. It runs first in the pipeline, ahead of
`UseHttpsRedirection()`. Two consumers depend on it: the health check `RestrictedIPs` allow-list
(`$api/modules/diagnostics`) and the ServiceModule auth rate limiter.

The trust settings widen in order — loopback, then private networks, then named entries:

| Key | Default | Effect |
|---|---|---|
| `TrustLoopback` | `true` | Keeps the framework's loopback defaults (`127.0.0.0/8`, `::1`). `false` clears both framework lists. |
| `TrustPrivateNetworks` | `true` | Adds `10.0.0.0/8`, `172.16.0.0/12`, `192.168.0.0/16`, `fc00::/7`. Covers container and tunnel setups without naming a gateway address. |
| `TrustedProxies` | `[]` | Additive. CIDR entries go to `KnownIPNetworks`, single addresses to `KnownProxies`. |
| `ForwardLimit` | `1` | Header entries processed, right to left. |
| `ForwardedForHeaderName` | `null` | Read a vendor header instead — `CF-Connecting-IP`, `X-Real-IP`, `X-Azure-ClientIP`. |

**Both flags off with an empty `TrustedProxies` fails startup validation.** Two empty trust lists make the
middleware skip the check entirely rather than match nothing (`checkKnownIps` is false), so every caller's header
would be believed. That is the one unsafe state and it is refused at boot.

Three framework behaviours worth knowing, all measured against `Microsoft.AspNetCore.HttpOverrides` 10.0:

- `ForwardedHeadersOptions.ForwardedHeaders` defaults to `None`, so a bare `UseForwardedHeaders()` is a silent
  no-op. The flags must be set.
- `KnownProxies` and `KnownIPNetworks` **cannot be bound from configuration** — they are get-only and neither
  `IPAddress` nor `IPNetwork` has a type converter. Binding a section onto `ForwardedHeadersOptions` silently
  drops them with no error. Hence the separate options class and the hand-written translation.
- `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true` enables the middleware from the environment **and empties both trust
  lists**. Our `Configure` delegate runs after the framework's and writes `ForwardedHeaders.None` when the feature
  is disabled, so the variable has no effect. Do not document it as a supported option.

The startup log prints the expanded trust list, so the flags are never opaque.

## Override Conventions

**Env vars** — use `__` to separate nested keys. This is the highest-priority override.

```bash
Settings__Logs__Retention=5
Settings__Enabled=True
```

**Production override files** — place a `<filename>.Production.json` next to the base file.
Only include the keys you want to change. The rest come from the base file.

```json
{
  "Settings": {
    "Enabled": true,
    "Logs": { "Retention": 5 }
  }
}
```

**Connection-string fallback** — for connection strings that contain credentials, use:

```
<NAME>_CONNECTION_STRING=<value>
```

Example: `DATABASE_CONNECTION_STRING=endpoint=https://localhost:1413`

The name must be uppercase. This maps to `ConnectionStrings.<Name>` in the config system.

## Precedence

Higher number wins.

| Priority | Method | Example (Logs.Retention) | Example (ConnectionStrings.Database) |
|---|---|---|---|
| 1 (lowest) | Connection-string fallback | N/A | `DATABASE_CONNECTION_STRING=...` |
| 2 | Direct file edit (`Settings.json`) | `Settings.json` | `ConnectionStrings.json` |
| 3 | Production override (`Settings.Production.json`) | `Settings.Production.json` | `ConnectionStrings.Production.json` |
| 4 (highest) | Environment variable | `Settings__Logs__Retention=5` | `ConnectionStrings__Database=...` |

## Feature Flags

All flags are boolean env vars. All default to `False` (disabled).

| Env Var | What it enables | Default |
|---|---|---|
| `SERVICE_MODULE=True` | JWT auth, rate limiting, account management | False |
| `UI_MODULE=True` | Blazor/Razor interactive packing demo | False |
| `SWAGGER_UI=True` | Swagger UI at `/swagger` | False |
| `SCALAR_UI=True` | Scalar UI at `/scalar` (alternative OpenAPI UI) | False |
| `DEBUG_ENDPOINT=True` | `/_debug` — echoes the caller's own request: connection address, every header, and server info. Unauthenticated; use it to read the proxy address when configuring forwarded headers, then turn it off. | False |

`ASPNETCORE_HTTP_PORTS` controls the internal listen port. Default is `8080`.

`BINACLE_ADMIN_CREDENTIALS` — sets the default admin account credentials on first run (ServiceModule only).
Not a feature flag and not in any config file — set it as an environment variable.
Format is defined by ServiceModule; see `$api/modules/service` for details.

```bash
ASPNETCORE_HTTP_PORTS=80
```

See `$api/modules` for how modules are wired at startup.
