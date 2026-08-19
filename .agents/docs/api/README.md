---
id: api
description: Index for API slice docs — endpoints, contracts, service, kernel, presets, and module docs (Diagnostics, ServiceModule, UIModule)
verified: 2026-08-19
check: The startup order matches Program.cs top to bottom, builder half then pipeline half; the dependency map matches every ProjectReference in api/**/*.csproj and the projects those reach; every type named in the v4 request flow still resolves
also_update:
  - api/modules
paths:
  - "api/**"
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
Feature.Manager = ...CreateManager()     // reads config + env vars, before DI container
                                         // controls all feature flags (SERVICE_MODULE, UI_MODULE, SWAGGER_UI, SCALAR_UI, DEBUG_ENDPOINT, ...)
AddBinacleServices()                     // lib factories, IBinacleService
ConfigureForwardedHeaders()              // proxy trust; writes ForwardedHeaders.None when disabled
AddDiagnosticsModule()
if SERVICE_MODULE → AddServiceModule()   // calls AddInfrastructure() internally
if UI_MODULE      → AddUIModule()
if SWAGGER_UI     → register Swagger OpenAPI docs
if SCALAR_UI      → register Scalar OpenAPI docs
---
UseForwardedHeaders()                    // FIRST — rewrites RemoteIpAddress and Scheme before anything reads them
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

`UseForwardedHeaders()` must stay first. It rewrites `Connection.RemoteIpAddress` and `Request.Scheme`, and
everything downstream — HTTPS redirection, the health check IP allow-list, rate limiting — reads those. See
`$api/configuration` for the trust settings.

## Project Dependency Map

Projects live in four top-level directories: `shared/src/`, `lib/src/`, `api/src/`, `vipaq/src/`.

```
shared/src/Binacle.Packing                → Binacle.Geometry
shared/src/Binacle.CompactNotation        → Binacle.Geometry
lib/src/Binacle.Lib                       → Binacle.Packing
vipaq/src/Binacle.ViPaq                   → Binacle.Geometry

api/src/Binacle.Net.Kernel                → Binacle.CompactNotation
api/src/Binacle.Net.DiagnosticsModule     → Binacle.Net.Kernel, Binacle.Packing, Binacle.CompactNotation
api/src/Binacle.Net.ServiceModule.Domain  (no dependencies)
api/src/Binacle.Net.ServiceModule.Infrastructure → Binacle.Net.Kernel, ServiceModule.Domain
api/src/Binacle.Net.ServiceModule         → Binacle.Net.Kernel, ServiceModule.Domain, ServiceModule.Infrastructure
api/src/Binacle.Net.UIModule              → Binacle.Net.Kernel, Binacle.Packing, Binacle.ViPaq, Binacle.CompactNotation
api/src/Binacle.Net                       → Binacle.Lib, all modules, Binacle.ViPaq
```

This is the API slice's view. The full graph across every slice — including who sees internals — is
`$api/dependencies`; keep the two in step.

Key rules:
- Kernel has no dependency on `Binacle.Net` or any module — safe to use from anywhere
- Lib and Binacle.Packing have no API dependencies — pure algorithm layer
- Only `Binacle.Net` references the packer. The two modules that need the result vocabulary (Diagnostics, UI)
  and the integration suite take `Binacle.Packing` instead; the Kernel takes neither
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
  → ViPaqSerializer.Serialize<...>()    (only if IncludeViPaqData: true and items were packed)
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

Six projects under `api/test/` — two integration suites and four unit suites. API Tests (`$api/tests`) names
each one, what it covers and the `just` alias that runs it; `just test all` runs every suite in the repo.

See Shared (`$shared`) for the scenario data format, and Commands (`$commands`) for how to run the API locally.

## Dependencies

How the API projects reference each other — the composition root, the Kernel floor, and the module walls — is in
`$api/dependencies`.

## Concepts

This slice implements Fit vs Pack (`$concepts`).
