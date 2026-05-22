---
description: Repo overview and index of agent documentation
---

# Binacle.Net — Agent Docs

Binacle.Net is an API that checks if items fit in boxes (fit) and packs them with position data (pack).
Built with ASP.NET Core (.NET 10) Minimal APIs. Main code is C#.

## Repo Layout

| Path | What it is |
|---|---|
| `api/src/Binacle.Net` | Main API — entry point, versioned endpoints, `Program.cs` |
| `api/src/Binacle.Net.Kernel` | Shared tools: endpoint registration, OpenAPI, feature flags, validation |
| `api/src/Binacle.Net.DiagnosticsModule` | Diagnostics middleware, always on |
| `api/src/Binacle.Net.ServiceModule` | Optional module (3 projects): JWT auth, rate limiting, account management |
| `api/src/Binacle.Net.ServiceModule.Domain` | Domain layer for ServiceModule — entities and repository interfaces |
| `api/src/Binacle.Net.ServiceModule.Infrastructure` | Infrastructure layer for ServiceModule — DB providers |
| `api/src/Binacle.Net.UIModule` | Optional: Blazor/Razor interactive packing demo |
| `api/test/Binacle.Net.IntegrationTests` | HTTP tests for v3 and v4 endpoints |
| `api/test/Binacle.Net.ServiceModule.IntegrationTests` | Tests for auth and rate limiting (ServiceModule only) |
| `lib/src/Binacle.Lib` | Core bin-packing algorithms and processors |
| `lib/src/Binacle.Lib.Abstractions` | Interfaces shared between `Binacle.Lib` and the API layer |
| `lib/test/` | Lib unit tests, performance tests, benchmarks |
| `vipaq/src/Binacle.ViPaq` | Compact binary format for encoding packing results |
| `vipaq/test/` | ViPaq unit tests |
| `vipaq/binacle-vipaq/` | TypeScript mirror of ViPaq |
| `shared/Binacle.TestsKernel` | Shared test fixtures and scenario data |
| `packages/` | TypeScript packages (npm workspaces) |
| `ruby/` | Ruby gems (Jekyll plugins) |
| `docs/` | Jekyll documentation site |
| `web/` | Jekyll marketing/web site |
| `api/requests/` | HTTP request files for manual testing (subfolders: v2, v3, v4, Service) |
| `samples/` | Docker and Kubernetes deployment samples |
| `shared/data/` | OR-library packing benchmark data |

## Commands

See [Commands](commands.md) — how to run the API, tests, benchmarks, and build the Docker image.

## Common Tasks

| Task | Read these |
|---|---|
| Add a v4 endpoint | `api/endpoints.md`, `api/v4/add-endpoint.md`, `api/v4/contracts.md`, `api/service.md`, `api/kernel.md` |
| Add or understand a contract type | `api/v4/contracts.md`, `api/v4/add-endpoint.md` |
| Work with ServiceModule (auth, rate limiting) | `api/modules/service.md`, `api/modules/README.md` |
| Understand startup and module wiring | `api/README.md`, `api/modules/README.md`, `api/kernel.md` |
| Understand fit vs pack | `concepts.md` |
| Understand how results are selected | `lib/result-selection.md`, `lib/processors.md` |
| Understand how OperationResult is built | `lib/result-building.md` |
| Add or modify algorithm processing | `lib/algorithm-factory.md`, `lib/processors.md` |
| Add or modify a test | `tests/README.md`, `tests/scenarios.md` |
| Work with presets | `api/presets.md`, `api/v4/README.md` |
| Understand v3 vs v4 differences | `api/v3/README.md`, `api/v4/README.md` |
| Work with ViPaq | `vipaq/README.md` |
| Configure modules / env vars / overrides | `api/configuration.md` |

## Slice Docs

- [Concepts](concepts.md) — fit vs pack; ideas that span slices
- [API](api/README.md) — endpoints, contracts, service, kernel, modules (Diagnostics, ServiceModule, UIModule)
- [Configuration](api/configuration.md) — config file layout, env-var conventions, feature flags
- [Lib](lib/README.md) — algorithms, processors, result building and selection
- [Tests](tests/README.md) — all test projects, fixture patterns, scenario data
- [ViPaq](vipaq/README.md) — `Binacle.ViPaq` binary format and TypeScript mirror
- [Packages](packages/README.md) — TypeScript npm packages
- [Ruby](ruby/README.md) — Ruby/Jekyll plugins
- [Docs Site](docs/README.md) — Jekyll docs site
- [Web Site](web/README.md) — Jekyll marketing site
