---
description: Manifest of every file under .agents/docs, grouped by area. Regenerate with just agents all.
---

# Agent Docs Index

Every doc in `.agents/docs/`, grouped by area. Scan for your topic, then read that file — do not work
from this summary. Task-based entry points ("I want to add a v4 endpoint") are in the "Common Tasks"
table of [README.md](README.md).

## General

| File | Description |
|---|---|
| [build-topology.md](build-topology.md) | Build & workspace topology — the .slnx solution, npm workspaces, gulp asset copy, Directory.Build.props (including the SonarQubeTestProject rule for support projects), central package management, the global.json test-runner opt-in, the publish/Dockerfile chain, and the NoTargets content projects |
| [commands.md](commands.md) | How to set up a clone, run the API and the two sites, run tests and benchmarks, and build the Docker image |
| [concepts.md](concepts.md) | Fit exits early on first failure; pack continues and returns positions. Both return the same result shape — packed items and unpacked items. Used by both Lib algorithms and API endpoints. |

## API

| File | Description |
|---|---|
| [api/configuration.md](api/configuration.md) | Config file layout, env-var conventions, override precedence, and feature flag list |
| [api/dependencies.md](api/dependencies.md) | API slice dependency tree — Binacle.Net as composition root, the Kernel floor, the always-compiled modules (Diagnostics, Service, UI), the ServiceModule clean-architecture split, and who sees internals. |
| [api/endpoints.md](api/endpoints.md) | Endpoint pattern, registration, request validation flow, and route groups for v3 and v4 |
| [api/kernel.md](api/kernel.md) | Binacle.Net.Kernel — shared patterns used by all API projects and modules |
| [api/modules/diagnostics.md](api/modules/diagnostics.md) | DiagnosticsModule — always-on logging, OpenTelemetry, health checks, and packing logs |
| [api/modules/README.md](api/modules/README.md) | Optional module system — feature flags, structure, available modules |
| [api/modules/service.md](api/modules/service.md) | ServiceModule — JWT auth, rate limiting, account/subscription management. Three projects using clean architecture. |
| [api/modules/ui.md](api/modules/ui.md) | UIModule — optional Blazor Web App interactive packing demo. Pages, JS stack, API connection, config, and services. |
| [api/openapi.md](api/openapi.md) | OpenAPI wiring — IOpenApiDocument, the Kernel transformers (JWT, 429, response descriptions, enum-as-string), what endpoint groups auto-wire, and the external OpenApiExamples package |
| [api/presets.md](api/presets.md) | What presets are, where they're configured, how route params map to bins, and how to add one for tests |
| [api/README.md](api/README.md) | Index for API slice docs — endpoints, contracts, service, kernel, presets, and module docs (Diagnostics, ServiceModule, UIModule) |
| [api/service.md](api/service.md) | IBinacleService — method reference for SingleBinAsync, MultipleBinsAsync, SmallestBinAsync, BestBinAsync; return types, call pattern, and algorithm selection |
| [api/tests.md](api/tests.md) | api/test integration tests — layout, v3/v4 HTTP conventions, validBinId, preset keys, special bins, base-class asserts, and test host config |
| [api/v3/contracts.md](api/v3/contracts.md) | v3 request and response contracts — field names, outer response wrapper, and enum values for fit and pack. |
| [api/v3/README.md](api/v3/README.md) | v3 API — stable, do not modify. Endpoints, algorithm selection, response shape, field names, and enum values. |
| [api/v4/add-endpoint.md](api/v4/add-endpoint.md) | Step-by-step guide for adding a new v4 endpoint |
| [api/v4/contracts.md](api/v4/contracts.md) | Request/response contract types, validators, and OpenAPI examples for v4 (v3 follows the same shape) |
| [api/v4/README.md](api/v4/README.md) | v4 API — active development. Endpoints, algorithm selection, parameters, contracts, and response shape. |

## CI/CD

| File | Description |
|---|---|
| [ci-cd/README.md](ci-cd/README.md) | CI/CD — the six GitHub Actions workflows in .github/workflows, what triggers each, the conventions they all follow, and the repo variables, secrets and environments they need |
| [ci-cd/release-pipeline.md](ci-cd/release-pipeline.md) | The release pipeline in release-docker-image.yml — six jobs from a pushed tag to a published GitHub release, GHCR as the staging registry, the copy-to-Docker-Hub step every tag reaches with a prerelease narrowed to its immutable tag, and the CHANGELOG.md release body |

## Docs Site

| File | Description |
|---|---|
| [docs-site/README.md](docs-site/README.md) | The published Jekyll documentation site at repo-root docs/ — versioned API docs with Swagger UI embed. `$docs-site` always means repo-root docs/, never .agents/docs/. |

## Lib

| File | Description |
|---|---|
| [lib/algorithm-factory.md](lib/algorithm-factory.md) | IAlgorithmFactory — how algorithm instances are created, DI registration, and how tests construct algorithms directly |
| [lib/algorithms.md](lib/algorithms.md) | Packing heuristics (FFD/WFD/BFD) — versions, operation types, trade-offs, and the fit/pack guarantee |
| [lib/dependencies.md](lib/dependencies.md) | Lib slice dependency tree — the Abstractions/Lib split, who sees internals (IVT), and the composition-root rule (only Binacle.Net references the concrete Binacle.Lib; everyone else uses Abstractions). |
| [lib/models.md](lib/models.md) | Lib model types and IWith* interfaces — Bin, Item, packed/unpacked results, and the constraints used in generic type parameters |
| [lib/processors.md](lib/processors.md) | IAlgorithmProcessor, IBinProcessor, and IMultiAlgorithmBinProcessor — their factories and which algorithms each execution path uses |
| [lib/README.md](lib/README.md) | Binacle.Lib and Binacle.Lib.Abstractions — the algorithm layer |
| [lib/result-building.md](lib/result-building.md) | OperationResultBuilder — how OperationResult is constructed, status rules, volume percentages, and integrity checks |
| [lib/result-selection.md](lib/result-selection.md) | IResultSelector, IResultSelectionStrategy, and the three selection strategies — scoring rules, tie-breaking, and how tests verify them |
| [lib/tests.md](lib/tests.md) | lib/test projects — unit tests, performance tests, benchmarks; AlgorithmFactories, CommonTestingFixture, ResultSelectionTestingFixture, and run aliases |

## Packages

| File | Description |
|---|---|
| [packages/binacle-net-ui.md](packages/binacle-net-ui.md) | packages/binacle-net-ui — Alpine.js components + Three.js visualizer for the packing demo. Components, plugins, model layers, and the window.binacle global. |
| [packages/dependencies.md](packages/dependencies.md) | TypeScript packages dependency tree — the npm workspaces and which package imports (and declares) which. |
| [packages/README.md](packages/README.md) | TypeScript packages under packages/ (npm workspaces) — UI components, compact-notation mirror, cookie utilities, and theme switching. |

## Ruby

| File | Description |
|---|---|
| [ruby/README.md](ruby/README.md) | Ruby gems under ruby/ — Jekyll plugins used by docs/ and web/ sites. |

## Samples

| File | Description |
|---|---|
| [samples/README.md](samples/README.md) | Deployment samples — Docker Compose (minimal, quickstart, prod, service, full) and Kubernetes (minimal); each folder name is a smoke profile name, feature flags, config wiring, and the keep-in-sync rule |

## Shared

| File | Description |
|---|---|
| [shared/dependencies.md](shared/dependencies.md) | Shared slice dependency tree — Geometry (the BCL-only leaf everything geometric bottoms out on), CompactNotation, TestReporting, and the TestsKernel test hub; who references them and who sees internals. |
| [shared/README.md](shared/README.md) | Shared slice — Binacle.TestsKernel (scenario data, compact-string formats, providers, fixtures) and shared/data (OR-Library benchmark data) |

## Tooling

| File | Description |
|---|---|
| [tooling/README.md](tooling/README.md) | tooling/ — every task the repo can run, called by CI and by hand alike: the test, coverage, openapi, agents, changelog, serve, build, image and smoke modules for just, the benchmark/performance scripts, the tmux script, local docker-compose, and emulator state |

## ViPaq

| File | Description |
|---|---|
| [vipaq/architecture.md](vipaq/architecture.md) | ViPaq architecture — the blind encode/decode layer, the layout codecs, and the serializer that chooses. The policy/mechanism split the rebuild keeps. |
| [vipaq/cross-language-testing.md](vipaq/cross-language-testing.md) | ViPaq cross-language wire testing — the C#/TS shared-vector apparatus, its inventory, and the decode-to-input contract |
| [vipaq/dependencies.md](vipaq/dependencies.md) | ViPaq project dependency tree — who references whom, who can see internals, and the deliberate walls (UnitTests never references TestsKernel; no test project references a generator). |
| [vipaq/README.md](vipaq/README.md) | Binacle.ViPaq — compact binary format for packing results. The wire is defined in PROTOCOL.md; this covers the C# API surface, repo layout, and tests. |
| [vipaq/typescript.md](vipaq/typescript.md) | Binacle.ViPaq TypeScript mirror (vipaq/packages/binacle-vipaq) — public API and how it differs from the C# library |

## Web Site

| File | Description |
|---|---|
| [web-site/README.md](web-site/README.md) | The published Jekyll marketing site at repo-root web/ — product home, apps listing, and interactive packing demo. `$web-site` always means repo-root web/. |
