---
description: DiagnosticsModule — always-on logging, OpenTelemetry, health checks, and packing logs
---

# DiagnosticsModule

`src/Binacle.Net.DiagnosticsModule`

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
- Maps `/health` (configurable path) with full JSON response via `UIResponseWriter`
- Health status codes: `Healthy/Degraded → 200`, `Unhealthy → 503`

## Config files

| File | What it configures |
|---|---|
| `Config_Files/DiagnosticsModule/Serilog.json` | Serilog sinks, enrichers, log levels |
| `Config_Files/DiagnosticsModule/Serilog.{Environment}.json` | Environment overrides |

OT, health checks, and packing logs are configured via `appsettings.json` sections
(`OpenTelemetryConfigurationOptions`, `HealthCheckConfigurationOptions`, `PackingLogsConfigurationOptions`).
