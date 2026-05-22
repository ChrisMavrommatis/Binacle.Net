---
description: Index for API slice docs — endpoints, contracts, service, kernel, presets, and module docs (Diagnostics, ServiceModule, UIModule)
---

# API

If you don't know where to start, read [add-endpoint.md](v4/add-endpoint.md) first.

## Projects

**Core:**
- `api/src/Binacle.Net.Kernel` — shared patterns used by all projects (endpoints, validation, feature flags, config)
- `api/src/Binacle.Net` — entry point; versioned endpoint groups; `Program.cs` wires everything

**Modules:**
- `api/src/Binacle.Net.DiagnosticsModule` — always-on; logging, OpenTelemetry, health checks, packing logs
- `api/src/Binacle.Net.ServiceModule` — optional (`SERVICE_MODULE` flag); JWT auth, rate limiting, account management
  - `api/src/Binacle.Net.ServiceModule.Domain` — domain layer; entities, repository interfaces
  - `api/src/Binacle.Net.ServiceModule.Infrastructure` — data layer; SQLite / PostgreSQL / Azure Tables backends
- `api/src/Binacle.Net.UIModule` — optional (`UI_MODULE` flag); Blazor interactive demo

## Startup Order

```
Feature.Manager.Initialize()             // reads config + env vars, before DI container
                                         // controls all feature flags (SERVICE_MODULE, UI_MODULE, SWAGGER_UI, SCALAR_UI, ...)
AddBinacleServices()                     // lib factories, IBinacleService
AddDiagnosticsModule()
if SERVICE_MODULE → AddServiceModule()   // calls AddInfrastructure() internally
if UI_MODULE      → AddUIModule()
if SWAGGER_UI     → register Swagger OpenAPI docs
if SCALAR_UI      → register Scalar OpenAPI docs
---
UseDiagnosticsModule()
if SERVICE_MODULE → UseServiceModule()   // auth, authz, rate limiter, v0 endpoints
if UI_MODULE      → UseUIModule()
if SWAGGER_UI     → UseSwaggerUI()
if SCALAR_UI      → UseScalarApiReference()
RegisterEndpointsFromAssemblyContaining<IApiMarker>()   // v3, v4 endpoints
RunStartupTasksAsync()
```

## Project Dependency Map

Projects live in three top-level directories: `lib/src/`, `api/src/`, `vipaq/src/`.

```
lib/src/Binacle.Lib.Abstractions          (no dependencies)
lib/src/Binacle.Lib                       → Binacle.Lib.Abstractions
vipaq/src/Binacle.ViPaq                   (no dependencies)

api/src/Binacle.Net.Kernel                → Binacle.Lib.Abstractions
api/src/Binacle.Net.DiagnosticsModule     → Binacle.Net.Kernel
api/src/Binacle.Net.ServiceModule.Domain  (no dependencies)
api/src/Binacle.Net.ServiceModule.Infrastructure → Binacle.Net.Kernel, ServiceModule.Domain
api/src/Binacle.Net.ServiceModule         → Binacle.Net.Kernel, ServiceModule.Domain, ServiceModule.Infrastructure
api/src/Binacle.Net.UIModule              → Binacle.Net.Kernel, Binacle.Lib.Abstractions, Binacle.ViPaq
api/src/Binacle.Net                       → Binacle.Lib, all modules, Binacle.ViPaq
```

Key rules:
- Kernel has no dependency on `Binacle.Net` or any module — safe to use from anywhere
- Lib and Lib.Abstractions have no API dependencies — pure algorithm layer
- Modules depend on Kernel but not on each other
- `Binacle.Net` is the only project that references everything

## v4 Request Flow

How a single v4 request moves through the system end to end:

```
HTTP POST /api/v4/...
  → BindingResult<TRequest>         deserialise JSON + run FluentValidation
  → endpoint handler                 calls IBinacleService method
  → IBinacleService                  picks processor path based on algorithm value
      → IBinProcessorFactory         creates LoopBinProcessor or LoopMultiAlgorithmBinProcessor
          → IAlgorithmProcessorFactory   (multi-algo path only) creates LoopAlgorithmProcessor
              → IAlgorithmFactory    creates the algorithm instance (FFD / WFD / BFD)
                  → algorithm.Execute()
                      → OperationResultBuilder   builds OperationResult (status, packed/unpacked items, percentages)
  ← OperationResult returned to handler
  → BinResponseBase.From<TResponse>()   maps OperationResult to the API response type
  → ViPaqSerializer.SerializeInt32()    (only if IncludeViPaqData: true and items were packed)
  → Results.Ok(response)
```

See [service.md](service.md) for the `IBinacleService` method signatures.
See [lib/processors.md](../lib/processors.md) for how the processor and factory layers work.
See [lib/result-building.md](../lib/result-building.md) for how `OperationResultBuilder` computes status and percentages.

## Active Development

- **v3** (`/api/v3`) — stable, do not modify
- **v4** (`/api/v4`) — active development

## Core Docs

- [Kernel](kernel.md) — BindingResult\<T\>, IOptionalDependency, Feature.Manager, IModuleMarker
- [Endpoints](endpoints.md) — endpoint pattern, registration, request flow
- [Contracts](v4/contracts.md) — v4 request/response types, validators
- [Service](service.md) — IBinacleService methods and how to call them from an endpoint
- [Presets](presets.md) — what presets are, config format, route params, adding test presets
- [Configuration](configuration.md) — config file layout, env-var conventions, override precedence, feature flags
- [v3](v3/README.md) — stable API, endpoints, response shape (do not modify)
- [v4](v4/README.md) — active development, implemented and planned endpoints
- [How to Add an Endpoint](v4/add-endpoint.md)

## Module Docs

- [Modules](modules/README.md) — how the module system works (feature flags, Add/Use pattern)
- [DiagnosticsModule](modules/diagnostics.md) — logging, OpenTelemetry, health checks, packing logs
- [ServiceModule](modules/service.md) — auth, rate limiting, accounts, subscriptions, clean arch layers
- [UIModule](modules/ui.md) — Blazor demo (not relevant to core API work)

## Related Tests

| Project | Alias | What it covers |
|---|---|---|
| `api/test/Binacle.Net.IntegrationTests` | `api` | HTTP behavior and scenario tests for v3 and v4 endpoints |
| `api/test/Binacle.Net.ServiceModule.IntegrationTests` | `api_service` | Auth and rate limiting (ServiceModule only) |

See [Tests](../tests/README.md) for stack, fixture patterns, and scenario data format.
See [Commands](../commands.md) for how to run the API locally.

## Concepts

This slice implements [Fit vs Pack](../concepts.md).
