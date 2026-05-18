---
description: The full API slice — core API, Kernel, and all modules
---

# API

## Projects

**Core:**
- `src/Binacle.Net.Kernel` — shared patterns used by all projects (endpoints, validation, feature flags, config)
- `src/Binacle.Net` — entry point; versioned endpoint groups; `Program.cs` wires everything

**Modules:**
- `src/Binacle.Net.DiagnosticsModule` — always-on; logging, OpenTelemetry, health checks, packing logs
- `src/Binacle.Net.ServiceModule` — optional (`SERVICE_MODULE` flag); JWT auth, rate limiting, account management
- `src/Binacle.Net.ServiceModule.Domain` — domain layer for ServiceModule; entities, repository interfaces
- `src/Binacle.Net.ServiceModule.Infrastructure` — data layer; SQLite / PostgreSQL / Azure Tables backends
- `src/Binacle.Net.UIModule` — optional (`UI_MODULE` flag); Blazor interactive demo

## Startup Order

```
BootstrapLogger()
AddDiagnosticsModule()
if SERVICE_MODULE → AddServiceModule()   // calls AddInfrastructure() internally
if UI_MODULE      → AddUIModule()
AddBinacleServices()                     // lib factories, IBinacleService
---
UseDiagnosticsModule()
if SERVICE_MODULE → UseServiceModule()   // auth, authz, rate limiter, v0 endpoints
if UI_MODULE      → UseUIModule()
RegisterEndpointsFromAssemblyContaining<IApiMarker>()   // v3, v4 endpoints
RunStartupTasksAsync()
```

## Active Development

- **v3** (`/api/v3`) — stable, do not modify
- **v4** (`/api/v4`) — active development

## Core Docs

- [Kernel](kernel.md) — BindingResult\<T\>, IOptionalDependency, Feature.Manager, IModuleMarker
- [Endpoints](endpoints.md) — endpoint pattern, registration, request flow
- [Contracts](contracts.md) — request/response types, validators
- [Service](service.md) — IBinacleService methods and how to call them from an endpoint
- [Presets](presets.md) — what presets are, config format, route params, adding test presets
- [v3 vs v4](v3-vs-v4.md) — how the two versions differ
- [How to Add an Endpoint](add-endpoint.md)

## Module Docs

- [Modules](modules.md) — how the module system works (feature flags, Add/Use pattern)
- [DiagnosticsModule](module-diagnostics.md) — logging, OpenTelemetry, health checks, packing logs
- [ServiceModule](module-service.md) — auth, rate limiting, accounts, subscriptions, clean arch layers
- [UIModule](module-ui.md) — Blazor demo (not relevant to core API work)

## Related Tests

| Project | Alias | What it covers |
|---|---|---|
| `test/Binacle.Net.IntegrationTests` | `api` | HTTP behavior and scenario tests for v3 and v4 endpoints |
| `test/Binacle.Net.ServiceModule.IntegrationTests` | `api_service` | Auth and rate limiting (ServiceModule only) |

See [Tests](../tests/README.md) for stack, fixture patterns, and scenario data format.

## Concepts

This slice implements [Fit vs Pack](../concepts/fit-vs-pack.md).
