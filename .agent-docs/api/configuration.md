---
description: Config file layout, env-var conventions, override precedence, and feature flag list
---

# Configuration

> Note: sourced from docs site (pre-v4). Verify if this changes.

## Config File Tree

All config files live under `/app/Config_Files` in the container.

```text
app
└── Config_Files
    ├── Presets.json
    ├── DiagnosticsModule
    │   ├── HealthChecks.json
    │   ├── OpenTelemetry.json
    │   ├── PackingLogs.json
    │   └── Serilog.json
    └── UiModule
        └── ConnectionStrings.json
```

Each file can be overridden by a `.Production.json` sibling (see below). Bind-mount individual files
in Docker with `-v $(pwd)/Presets.json:/app/Config_Files/Presets.json:ro`.

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
| `SCALAR_UI=True` | Scalar UI (alternative OpenAPI UI) | False |

`ASPNETCORE_HTTP_PORTS` controls the internal listen port. Default is `8080`.

```bash
ASPNETCORE_HTTP_PORTS=80
```

See [modules.md](modules.md) for how modules are wired at startup.
