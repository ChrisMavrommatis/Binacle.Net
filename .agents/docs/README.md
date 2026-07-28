---
id: docs
description: Repo overview and index of agent documentation
verified: 2026-07-10
check: Repo layout table matches actual directories in the root
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
| `api/test/*.UnitTests` | One unit suite per source project — `Binacle.Net`, `Kernel`, `DiagnosticsModule`, `ServiceModule`. `Kernel.UnitTests` is split by feature folder |
| `lib/src/Binacle.Lib` | Core bin-packing algorithms and processors |
| `lib/src/Binacle.Lib.Abstractions` | Interfaces shared between `Binacle.Lib` and the API layer |
| `lib/test/` | Lib unit tests, performance tests, benchmarks |
| `vipaq/src/Binacle.ViPaq` | Compact binary format for encoding packing results |
| `vipaq/test/` | ViPaq unit tests |
| `vipaq/packages/binacle-vipaq/` | TypeScript mirror of ViPaq |
| `shared/src/Binacle.Geometry` | Shared geometry leaf — generic `IWith*` interfaces + concrete `Dimensions<T>`/`Coordinates<T>` (BCL-only, referenced by lib, ViPaq, CompactNotation) |
| `shared/src/Binacle.CompactNotation` | Shared compact-string parser/formatter (`LxWxH (X,Y,Z) [Q]`) |
| `shared/test/Binacle.TestsKernel` | Shared test fixtures and scenario data |
| `shared/test/Binacle.CompactNotation.UnitTests` | Tests for the shared compact notation |
| `packages/` | TypeScript packages (npm workspaces) |
| `ruby/` | Ruby gems (Jekyll plugins) |
| `docs/` | Jekyll documentation site — the published one (`$docs-site`), not `.agents/docs/` |
| `web/` | Jekyll marketing/web site (`$web-site`) |
| `api/requests/` | HTTP request files for manual testing (subfolders: v3, v4, Service) |
| `samples/` | Docker and Kubernetes deployment samples (user-facing starting points) |
| `config/` | Maintainer local-dev tooling — the `just` modules (test, coverage, openapi, agents, serve), the benchmark/performance/build scripts, local compose, env, emulator state |
| `shared/data/` | OR-library packing benchmark data |

## Commands

See Commands (`$commands`) — how to set up a clone, run the API and the two sites, run tests and benchmarks,
and build the Docker image.

## Common Tasks

| Task | Read these |
|---|---|
| Add a v4 endpoint | `$api/endpoints`, `$api/v4/add-endpoint`, `$api/v4/contracts`, `$api/service`, `$api/kernel`, `$api/openapi` |
| Add or understand a contract type | `$api/v4/contracts`, `$api/v4/add-endpoint` |
| Work with ServiceModule (auth, rate limiting) | `$api/modules/service`, `$api/modules` |
| Understand startup and module wiring | `$api`, `$api/modules`, `$api/kernel` |
| Understand fit vs pack | `$concepts` |
| Understand how results are selected | `$lib/result-selection`, `$lib/processors` |
| Understand how OperationResult is built | `$lib/result-building` |
| Add or modify algorithm processing | `$lib/algorithm-factory`, `$lib/processors` |
| Add or modify a lib test | `$lib/tests`, `$shared` (scenario data & formats) |
| Add or modify an API integration test | `$api/tests`, `$shared` (scenario data & formats) |
| Work with presets | `$api/presets`, `$api/v4` |
| Understand v3 vs v4 differences | `$api/v3`, `$api/v4` |
| Work with ViPaq | `$vipaq` |
| Configure modules / env vars / overrides | `$api/configuration` |
| Run behind a proxy / resolve the real client IP | `$api/configuration`, `$api/modules/diagnostics` |
| Run or deploy with Docker / Kubernetes | `$samples`, `$commands`, `$build-topology` |
| Understand the build & workspace layout | `$build-topology`, `$commands` |

## Slice Docs

- Concepts (`$concepts`) — fit vs pack; ideas that span slices
- API (`$api`) — endpoints, contracts, service, kernel, modules (Diagnostics, ServiceModule, UIModule)
- Configuration (`$api/configuration`) — config file layout, env-var conventions, feature flags
- Lib (`$lib`) — algorithms, processors, result building and selection; lib tests
- Shared (`$shared`) — Binacle.TestsKernel scenario data & compact formats; OR-Library data
- ViPaq (`$vipaq`) — `Binacle.ViPaq` binary format and TypeScript mirror
- Packages (`$packages`) — TypeScript npm packages
- Ruby (`$ruby`) — Ruby/Jekyll plugins
- Docs Site (`$docs-site`) — the published Jekyll site at repo-root `docs/`
- Web Site (`$web-site`) — the published Jekyll site at repo-root `web/`
- Samples (`$samples`) — Docker & Kubernetes deployment starting points
- Config (`$config`) — maintainer local-dev tooling: scripts, local compose, env, emulator state
- Build Topology (`$build-topology`) — solution, npm workspaces, asset copy, Docker build chain
