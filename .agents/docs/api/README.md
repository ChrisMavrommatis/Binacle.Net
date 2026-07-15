---
id: api
description: Index for API slice docs — endpoints, contracts, service, kernel, presets, and module docs (Diagnostics, ServiceModule, UIModule)
verified: 2026-07-06
check: Startup sequence matches Program.cs; dep map matches actual project references
also_update:
  - api/modules
---

# API

If you don't know where to start, read `$api/v4/add-endpoint` first.

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
UseHttpsRedirection() / UseExceptionHandler() / UseCors()
if SWAGGER_UI or SCALAR_UI → MapOpenApi()          // maps /openapi/{documentName}.json
    if SWAGGER_UI → UseSwaggerUI()                 // these run BEFORE the module Use* calls
    if SCALAR_UI  → MapScalarApiReference()
UseDiagnosticsModule()
if SERVICE_MODULE → UseServiceModule()   // auth, authz, rate limiter, v0 endpoints
if UI_MODULE      → UseUIModule()
RegisterEndpointsFromAssemblyContaining<IApiMarker>()   // v3, v4 endpoints
RunStartupTasksAsync()
```

Note: Scalar is wired with `app.MapScalarApiReference(...)` (not a `Use*` method), and the
Swagger/Scalar UI block is registered **before** `UseDiagnosticsModule()` and the optional-module
`Use*` calls — not after them.

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

See `$api/service` for the `IBinacleService` method signatures.
See `$lib/processors` for how the processor and factory layers work.
See `$lib/result-building` for how `OperationResultBuilder` computes status and percentages.

## Active Development

- **v3** (`/api/v3`) — stable, do not modify
- **v4** (`/api/v4`) — active development

## Core Docs

- Kernel (`$api/kernel`) — BindingResult\<T\>, IOptionalDependency, Feature.Manager, IModuleMarker
- Endpoints (`$api/endpoints`) — endpoint pattern, registration, request flow
- Contracts (`$api/v4/contracts`) — v4 request/response types, validators
- Service (`$api/service`) — IBinacleService methods and how to call them from an endpoint
- Presets (`$api/presets`) — what presets are, config format, route params, adding test presets
- Configuration (`$api/configuration`) — config file layout, env-var conventions, override precedence, feature flags
- v3 (`$api/v3`) — stable API, endpoints, response shape (do not modify)
- v4 (`$api/v4`) — active development, implemented and planned endpoints
- How to Add an Endpoint (`$api/v4/add-endpoint`)

## Module Docs

- Modules (`$api/modules`) — how the module system works (feature flags, Add/Use pattern)
- DiagnosticsModule (`$api/modules/diagnostics`) — logging, OpenTelemetry, health checks, packing logs
- ServiceModule (`$api/modules/service`) — auth, rate limiting, accounts, subscriptions, clean arch layers
- UIModule (`$api/modules/ui`) — Blazor demo (not relevant to core API work)

## Related Tests

| Project | Alias | What it covers |
|---|---|---|
| `api/test/Binacle.Net.IntegrationTests` | `api` | HTTP behavior and scenario tests for v3 and v4 endpoints |
| `api/test/Binacle.Net.ServiceModule.IntegrationTests` | `api_service` | Auth and rate limiting (ServiceModule only) |

See API Tests (`$api/tests`) for integration-test conventions, and Shared (`$shared`) for the scenario
data format. See Commands (`$commands`) for how to run the API locally.

## Dependencies

How the API projects reference each other — the composition root, the Kernel floor, and the module walls — is in
`$api/dependencies`.

## Concepts

This slice implements Fit vs Pack (`$concepts`).
