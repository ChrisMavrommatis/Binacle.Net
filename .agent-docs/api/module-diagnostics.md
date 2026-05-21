---
description: DiagnosticsModule — always-on logging, OpenTelemetry, health checks, and packing logs
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

**Health checks** — always registered. Includes `SystemHealthCheck` tagged `"Core"`.
Path and enabled state are configured via `HealthCheckConfigurationOptions`.

## What UseDiagnosticsModule wires

- `HealthChecksProtectionMiddleware` — restricts health endpoint access (IP allow-list or similar)
- Maps `/_health` (configurable path) with full JSON response via `UIResponseWriter`
- Health status codes: `Healthy/Degraded → 200`, `Unhealthy → 503`

## Config files

| File | What it configures |
|---|---|
| `Config_Files/DiagnosticsModule/Serilog.json` | Serilog sinks, enrichers, log levels |
| `Config_Files/DiagnosticsModule/Serilog.{Environment}.json` | Environment overrides |
| `Config_Files/DiagnosticsModule/HealthChecks.json` | Health check path, IP restrictions, enabled state |
| `Config_Files/DiagnosticsModule/OpenTelemetry.json` | OTLP and Azure Monitor exporters |
| `Config_Files/DiagnosticsModule/PackingLogs.json` | Packing log channels and file paths |

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

<!-- sourced from docs site; verify against current code if behaviour changes -->

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

`RestrictedIPs` accepts single IPs, ranges, and CIDR notation:

```json
"RestrictedIPs": [
  "192.168.1.1",
  "192.168.1.0-192.168.1.255",
  "192.168.1.0/24"
]
```

`RestrictedChecks` lists check names to skip. Example: `["Database"]`.

Built-in checks:
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

<!-- sourced from docs site; verify against current code if behaviour changes -->

Default: **disabled**. Format: NDJSON. Config file: `Config_Files/DiagnosticsModule/PackingLogs.json`.

Two channels, one per operation type:

| Channel | Default path | Default filename |
|---|---|---|
| `Fitting` | `data/pack-logs/fitting/` | `{0}.ndjson` |
| `Packing` | `data/pack-logs/packing/` | `{0}.ndjson` |

`{0}` is replaced by the date according to `DateFormat` (default `yyyyMMdd`).

`ChannelLimit`:
- `0` — unbounded; limited only by available memory.
- `> 0` — queue cap. If the writer falls behind, newest log entries are dropped to prevent overload.
  Default is `100`.
