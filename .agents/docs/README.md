---
id: docs
description: Repo overview and index of agent documentation
verified: 2026-08-19
check: The repo layout table matches `ls -d */` at the root plus the subpaths it names; the workflow count matches .github/workflows/; the just module list matches tooling/*.just. The root-directory set itself is deliberately not in `paths:` — see below.
paths:
  - ".github/workflows/**"
  - "tooling/*.just"
---

# Binacle.Net — Agent Docs

Binacle.Net is an API that checks if items fit in boxes (fit) and packs them with position data (pack).
Built with ASP.NET Core (.NET 10) Minimal APIs. Main code is C#.

## Repo Layout

> **What `paths:` can and cannot watch here.** The two entries above fire when a workflow or a `just` module is
> added or removed, which is most of what goes stale in the table below. **The set of top-level directories is
> not among them**: a pathspec broad enough to catch a new root folder (`*/`) matches every commit in the repo
> and would report this file as stale forever. So a new or deleted root directory has to be noticed by a reader,
> not by the date check.

| Path | What it is |
|---|---|
| `api/src/Binacle.Net` | Main API — entry point, versioned endpoints, `Program.cs` |
| `api/src/Binacle.Net.Kernel` | Shared tools: endpoint registration, OpenAPI, feature flags, validation |
| `api/src/Binacle.Net.DiagnosticsModule` | Diagnostics middleware, always on |
| `api/src/Binacle.Net.ServiceModule` | Optional module (3 projects): JWT auth, rate limiting, account management |
| `api/src/Binacle.Net.ServiceModule.Domain` | Domain layer for ServiceModule — entities and repository interfaces |
| `api/src/Binacle.Net.ServiceModule.Infrastructure` | Infrastructure layer for ServiceModule — DB providers |
| `api/src/Binacle.Net.UIModule` | Optional: Razor Pages demo host — packing demo and ViPaq decoder |
| `api/test/Binacle.Net.IntegrationTests` | HTTP tests for v3 and v4 endpoints |
| `api/test/Binacle.Net.ServiceModule.IntegrationTests` | Tests for auth and rate limiting (ServiceModule only) |
| `api/test/*.UnitTests` | One unit suite per source project — `Binacle.Net`, `Kernel`, `DiagnosticsModule`, `ServiceModule`. `Kernel.UnitTests` is split by feature folder |
| `lib/src/Binacle.Lib` | Core bin-packing algorithms and processors |
| `shared/src/Binacle.Packing` | The packing vocabulary shared between `Binacle.Lib` and the API layer |
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
| `sites/` | Every published site, one directory each (`$sites`) |
| `sites/docs/` | Jekyll documentation site — the published one (`$sites/docs`), not `.agents/docs/` |
| `sites/web/` | Jekyll marketing/web site (`$sites/web`) |
| `api/requests/` | HTTP request files for manual testing (subfolders: v3, v4, Service) |
| `samples/` | Docker and Kubernetes deployment samples (user-facing starting points) |
| `tooling/` | Every task the repo can run, called by CI and by hand alike — eleven `just` modules (agents, build, changelog, check, coverage, image, openapi, regen, serve, smoke, tests), the benchmark/performance scripts, local compose, env, emulator state |
| `.github/workflows/` | The eight GitHub Actions workflows — the PR gate, the shared test suite, Sonar, CodeQL, the release pipeline, image smoke, and the two site deploys (`$ci-cd`) |
| `shared/data/` | OR-library packing benchmark data |
| `assets/` | Shared images, js, css and fonts, copied into both Jekyll sites by `gulpfile.js` |
| `results/` | The hand-curated measurement vault — benchmark and size reports, never auto-written (`$build-topology`) |
| `artifacts/` | Build output only — `binacle-net/`, `docs/`, `web/`, `openapi/`, `tests/`, `coverage/`. Never edit |

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
| Change a GitHub Actions workflow | `$ci-cd`, `$tooling` (the recipe it calls) |
| Understand how the image gets released | `$ci-cd/release-pipeline`, `$build-topology` |
| Add a CI check or a PR gate | `$ci-cd`, `$tooling` |

## Slice Docs

- Concepts (`$concepts`) — fit vs pack; ideas that span slices
- API (`$api`) — endpoints, contracts, service, kernel, modules (Diagnostics, ServiceModule, UIModule)
- Configuration (`$api/configuration`) — config file layout, env-var conventions, feature flags
- Lib (`$lib`) — algorithms, processors, result building and selection; lib tests
- Shared (`$shared`) — Binacle.TestsKernel scenario data & compact formats; OR-Library data
- ViPaq (`$vipaq`) — `Binacle.ViPaq` binary format and TypeScript mirror
- Packages (`$packages`) — TypeScript npm packages
- Ruby (`$ruby`) — Ruby/Jekyll plugins
- Sites (`$sites`) — every published site, and what they share
- Docs Site (`$sites/docs`) — the published Jekyll site at `sites/docs/`
- Web Site (`$sites/web`) — the published Jekyll site at `sites/web/`
- Samples (`$samples`) — Docker & Kubernetes deployment starting points
- Tooling (`$tooling`) — every task the repo can run: the `just` modules, scripts, local compose, env, emulator state
- CI/CD (`$ci-cd`) — the GitHub Actions workflows, their conventions, vars and secrets; the release pipeline
  is `$ci-cd/release-pipeline`
- Build Topology (`$build-topology`) — solution, npm workspaces, asset copy, Docker build chain
