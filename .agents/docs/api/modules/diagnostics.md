---
id: api/modules/diagnostics
description: DiagnosticsModule — always-on logging, OpenTelemetry, health checks, and packing logs
verified: 2026-07-27
check: Env var names match DiagnosticsModule config handling
also_update:
  - api/configuration
---

# DiagnosticsModule

`api/src/Binacle.Net.DiagnosticsModule`

Always registered — no feature flag. Must be added before optional modules in `Program.cs`.

## Registration

```csharp
builder.BootstrapLogger();        // call first, before everything
builder.AddDiagnosticsModule();   // during builder phase
// ...
app.UseDiagnosticsModule();       // during app phase
```

## BootstrapLogger

Creates a Serilog bootstrap logger before the DI container exists.
Minimum level: `Information` (ASP.NET Core and Azure.Core suppressed to `Warning`).
Enriches with: machine name, process ID, thread ID, Binacle version.

Must be called before `AddDiagnosticsModule()` so module initialisation messages are captured.

## What AddDiagnosticsModule registers

**Logging** — replaces the bootstrap logger with a full Serilog instance configured from
`Config_Files/DiagnosticsModule/Serilog.json` (and `Serilog.{Environment}.json` if present).

**OpenTelemetry** — conditional on `OpenTelemetryConfigurationOptions.IsEnabled()` (config-driven).
When enabled: metrics (runtime + ASP.NET Core), tracing (ASP.NET Core + HTTP client), logging.
Exporters: OTLP and/or Azure Monitor — each enabled independently via config.

**Packing logs** — conditional on `PackingLogsConfigurationOptions.Enabled`.
When enabled: registers a channel-based log processor for algorithm operation logs.
`BinacleService` writes to this channel via `IOptionalDependency<Channel<...>>` — it's a no-op when disabled.

**Health checks** — always registered. Includes the `SystemHealthCheck` class, registered under the
name `"System"` with tag `"Core"` (failure status `Unhealthy`). Use the name `"System"` — not the class
name — in config such as `RestrictedChecks`.
Path and enabled state are configured via `HealthCheckConfigurationOptions`.

## What UseDiagnosticsModule wires

- `ForwardedHeadersDiagnosticsMiddleware` — always registered; warns once when a forwarding header arrived and
  did not take effect (see below)
- `HealthChecksProtectionMiddleware` — restricts health endpoint access by caller address (`RestrictedIPs`)
- Maps `/_health` (configurable path) with full JSON response via `UIResponseWriter`
- Health status codes: `Healthy/Degraded → 200`, `Unhealthy → 503`

## Forwarded headers diagnostic

`ForwardedHeadersDiagnosticsMiddleware` exists because the two ways forwarded headers fail are both silent, and
both leave every component downstream — the health check allow-list, the login throttle, the logs — reading the
proxy as the caller:

| State | What it warns |
|---|---|
| Feature off, request carries the header | Something in front is rewriting the caller and the app ignores it |
| Feature on, header not applied | The trust list does not name the proxy (the framework says only `Unknown proxy`, at Debug) |

It reads `IOptions<ForwardedHeadersOptions>` — the framework type, not `Binacle.Net`'s config class, which the
module cannot see. `ConfigureForwardedHeaders` writes `ForwardedHeaders.None` when the feature is off, on
purpose, so `None` is the app's own answer to whether the feature is live. The same options give the configured
`ForwardedForHeaderName` (so a vendor header such as `CF-Connecting-IP` is watched instead of `X-Forwarded-For`)
and `OriginalForHeaderName`.

**`X-Original-For` is the signal that the header was applied**, not the presence of `X-Forwarded-For`: the
framework rewrites the forwarded header as it consumes entries and removes it once empty, so what is left says
nothing. Original-for is written only when an address was actually replaced.

Warns **once per process** — a misconfigured proxy sends the header on every request, and a warning per request
buries the log it is drawing attention to. Both states are fixed at startup, so one flag covers both. It runs
after `UseForwardedHeaders()` (`Program.cs`), which is the only place the outcome is visible.

Nothing here trusts a header; they are read only to decide whether to warn.

## Config files

| File | What it configures |
|---|---|
| `Config_Files/DiagnosticsModule/Serilog.json` | Serilog sinks, enrichers, log levels |
| `Config_Files/DiagnosticsModule/Serilog.{Environment}.json` | Environment override |
| `Config_Files/DiagnosticsModule/HealthChecks.json` | Health check path, IP restrictions, enabled state |
| `Config_Files/DiagnosticsModule/HealthChecks.{Environment}.json` | Environment override |
| `Config_Files/DiagnosticsModule/OpenTelemetry.json` | OTLP and Azure Monitor exporters |
| `Config_Files/DiagnosticsModule/OpenTelemetry.{Environment}.json` | Environment override |
| `Config_Files/DiagnosticsModule/PackingLogs.json` | Packing log channels and file paths |
| `Config_Files/DiagnosticsModule/PackingLogs.{Environment}.json` | Environment override |

## Logging

<!-- sourced from docs site; verify against current code if behaviour changes -->

Default sinks: Console + rolling File.

- Console: outputs with ANSI theme and `[HH:mm:ss LVL] Message <s:SourceContext>` template.
- File: `/app/data/logs/{date}.ndjson`, daily rolling, 7-day retention (`retainedFileCountLimit: 7`).
  Format: `Serilog.Formatting.Compact.RenderedCompactJsonFormatter`.

Minimum level: `Information`.
Overrides: `Microsoft.AspNetCore → Warning`, `Azure.Core → Warning`.

Enrichers: `FromLogContext`, `WithMachineName`, `WithProcessId`, `WithThreadId`.
Property: `Application: Binacle.Net`.

Override logging by creating `Serilog.Production.json` at `/app/Config_Files/DiagnosticsModule/`.

## Health checks

Default: **disabled**. Default path: `/_health`.

```json
{
  "HealthChecks": {
    "Enabled": false,
    "Path": "/_health",
    "RestrictedIPs": [],
    "RestrictedChecks": []
  }
}
```

`RestrictedIPs` accepts a single address or a CIDR range. Empty means no restriction:

```json
"RestrictedIPs": [
  "192.168.1.1",
  "192.168.1.0/24"
]
```

Entries are parsed by `IPEntry.TryParse` (`$api/kernel`, Network) into `System.Net.IPNetwork`; a single address
becomes a `/32` or `/128` so the middleware matches one kind of entry. An invalid entry fails startup validation
— which spellings are invalid, and why, is the Kernel's business and is documented there.

`HealthCheckAllowList` holds the parsed list and answers `RestrictsNobody` and `Allows(caller)`. It is nested in
`HealthChecksProtectionMiddleware`, whose `BuildAllowList` is the only thing that constructs it — so moving to
`IOptionsMonitor` is a second call to that method and nothing else. `BuildAllowList` also logs a warning for an
entry whose host bits were masked off: `192.168.1.1/24` covers 256 addresses and does not look like it.

The caller's address is normalised with `IPEntry.Normalize` before matching, which unmaps IPv4-mapped IPv6
addresses. The server listens on a dual-mode socket, so an IPv4 caller arrives as `::ffff:x.x.x.x` and no IPv4
entry would match without it.

**Behind a proxy this list matches the proxy, not the caller** — it compares `Connection.RemoteIpAddress`, which
is the proxy's address until forwarded headers resolve it. See [Forwarded headers](../configuration.md#forwarded-headers-forwardedheadersjson).

`RestrictedChecks` is an **allow-list**, not a skip-list. When empty (the default), all checks run.
When non-empty, ONLY the checks whose name is in the list run — everything else is filtered out.
Example: `["Database"]` means "run only the Database check", not "skip Database".
Match on the registered name (e.g. `"System"`, `"Database"`), not the class name.

Built-in checks (by registered name):
- `System` — always present (the `SystemHealthCheck` class, tag `"Core"`).
- `Database` — available only when `ServiceModule` is active.

## OpenTelemetry

<!-- sourced from docs site; verify against current code if behaviour changes -->

Config file: `Config_Files/DiagnosticsModule/OpenTelemetry.json`.

Top-level fields:
- `ServiceNamespace` — service namespace label (default: `"binacle"`).
- `ServiceInstanceId` — unique instance ID (default: `"1"`).
- `AdditionalAttributes` — key-value attributes attached to all telemetry.

OTLP exporter (`Otlp`):
- `Enabled` (bool, default false)
- `Endpoint` (string, e.g. `http://localhost:4317`)
- `Protocol` — `"grpc"` (default) or `"http/protobuf"`

The recommended way to configure OTLP is via the standard OpenTelemetry .NET SDK env vars
(e.g. `OTEL_EXPORTER_OTLP_ENDPOINT`). See the [OTLP exporter docs](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Exporter.OpenTelemetryProtocol/README.md#environment-variables).

Azure Monitor exporter (`AzureMonitor`):
- `Enabled` (bool, default false)
- `ConnectionString` (string)
- `EnableLiveMetrics` (bool, default false)
- `SamplingRatio` (float, default 1 = 100%)

Each signal group (`Metrics`, `Tracing`, `Logging`) also accepts `AdditionalAttributes` and a list
of additional meters/sources to include.

## Packing logs

Default: **disabled**. Format: NDJSON. Config file: `Config_Files/DiagnosticsModule/PackingLogs.json`.

### How it's wired

The **generic** log pipeline lives in the Kernel — `ILogEntryConvertible<TLog>`, `LogsProcessor<TRequest, TLog>`,
`LogsProcessorOptions`, and `AddLogProcessor<TChannelRequest, TLog>` (see
[kernel.md](../kernel.md#logs-generic-pipeline)). The **concrete** packing feature lives here, in one file,
`DiagnosticsModule/Logs/Models/AlgorithmOperationLogChannelRequest.cs`:

- `AlgorithmOperationLogChannelRequest : ILogEntryConvertible<PackingLogEntry>` — the channel message. Carries
  `Bins` (`IReadOnlyCollection<IIdentifiableBin>`), `Items` (`IReadOnlyCollection<IIdentifiableItem>`),
  `Parameters` (`ILogParametersProvider?`), and `Results` (`IDictionary<string, OperationResult>`). Its static
  `From<TBin, TItem, TParams>(...)` builds it with **no copy** (covariant read-only collections); `ToLogEntry(timestamp)`
  maps it to a `PackingLogEntry` in the background — the request thread only enqueues references. No `UserId`
  (per-user logging is a ServiceModule concern).
- `PackingLogEntry` (record) — the JSON line: `Timestamp`, `Parameters` (`IReadOnlyList<string>?`, omitted when
  null), `Bins`/`Items` (compact strings keyed by id, e.g. `"small-box" -> "10x10x10"`), and `Results`
  (`IReadOnlyDictionary<string, LogResult>`). Compact strings come from `CompactNotationFormatter`.
- `LogResult` (record) — one algorithm's result: `Status`, `PackedBinVolumePercentage`,
  `PackedItemsVolumePercentage`, and `PackedItems`/`UnpackedItems` (compact strings grouped by id).

Registration: `AddOptionsBasedPackingLogProcessor(optionsSelector)` (DiagnosticsModule
`ExtensionMethods/LogProcessorServiceCollectionExtensions.cs`) reads the config and calls the Kernel's
`AddLogProcessor<AlgorithmOperationLogChannelRequest, PackingLogEntry>`. Gated by `PackingLogs.Enabled`.

### Config

Flat — `PackingLogs` has `Enabled`, `Path` (default `data/pack-logs/`), `FileName` (default `{0}.ndjson`),
`DateFormat` (default `yyyyMMdd`), `ChannelLimit`. Both fit and pack flow through the one channel and land there.
`{0}` is replaced by the date.

`ChannelLimit`:
- `0` or absent — unbounded; limited only by available memory.
- `> 0` — bounded queue with drop-newest (`DropWrite`). If the writer falls behind, newest entries are dropped
  to prevent overload. Default is `100`.
