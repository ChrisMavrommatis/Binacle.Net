---
description: Flat manifest of every agent doc — path and one-line description. Regenerate with config/docs.sh.
---

# Agent Docs Index

Full list of every doc in `.agent-docs/`. Read the relevant file for details.

| Doc | Description |
|---|---|
| [api/configuration.md](api/configuration.md) | Config file layout, env-var conventions, override precedence, and feature flag list |
| [api/endpoints.md](api/endpoints.md) | Endpoint pattern, registration, request validation flow, and route groups for v3 and v4 |
| [api/kernel.md](api/kernel.md) | Binacle.Net.Kernel — shared patterns used by all API projects and modules |
| [api/modules/diagnostics.md](api/modules/diagnostics.md) | DiagnosticsModule — always-on logging, OpenTelemetry, health checks, and packing logs |
| [api/modules/README.md](api/modules/README.md) | Optional module system — feature flags, structure, available modules |
| [api/modules/service.md](api/modules/service.md) | ServiceModule — JWT auth, rate limiting, account/subscription management. Three projects using clean architecture. |
| [api/modules/ui.md](api/modules/ui.md) | UIModule — optional Blazor Web App interactive packing demo. Pages, JS stack, API connection, config, and services. |
| [api/openapi.md](api/openapi.md) | OpenAPI wiring — IOpenApiDocument, the Kernel transformers (JWT, 429, response descriptions, enum-as-string), what endpoint groups auto-wire, and the external OpenApiExamples package |
| [api/presets.md](api/presets.md) | What presets are, where they're configured, how route params map to bins, and how to add one for tests |
| [api/README.md](api/README.md) | Index for API slice docs — endpoints, contracts, service, kernel, presets, and module docs (Diagnostics, ServiceModule, UIModule) |
| [api/service.md](api/service.md) | IBinacleService — method reference for SingleBinAsync, MultipleBinsAsync, SmallestBinAsync; return types, call pattern, and algorithm selection |
| [api/tests.md](api/tests.md) | api/test integration tests — v3/v4 HTTP conventions, validBinId, preset keys, special bins, base-class asserts, and test host config |
| [api/v3/contracts.md](api/v3/contracts.md) | v3 request and response contracts — field names, outer response wrapper, and enum values for fit and pack. |
| [api/v3/README.md](api/v3/README.md) | v3 API — stable, do not modify. Endpoints, algorithm selection, response shape, field names, and enum values. |
| [api/v4/add-endpoint.md](api/v4/add-endpoint.md) | Step-by-step guide for adding a new v4 endpoint |
| [api/v4/contracts.md](api/v4/contracts.md) | Request/response contract types, validators, and OpenAPI examples for v4 (v3 follows the same shape) |
| [api/v4/README.md](api/v4/README.md) | v4 API — active development. Endpoints (implemented and planned), algorithm selection, parameters, contracts, and response shape. |
| [build-topology.md](build-topology.md) | Build & workspace topology — the .slnx solution, npm workspaces, gulp asset copy, Directory.Build.props, the Dockerfile/build.sh chain, and the NoTargets content projects |
| [commands.md](commands.md) | How to run the API, tests, benchmarks, and build the Docker image |
| [concepts.md](concepts.md) | Fit exits early on first failure; pack continues and returns positions. Both return the same result shape — packed items and unpacked items. Used by both Lib algorithms and API endpoints. |
| [config/README.md](config/README.md) | config/ — maintainer local-dev tooling: run/test/benchmark/build scripts, the doc-index and tmux scripts, local docker-compose, env files, and emulator state |
| [docs/README.md](docs/README.md) | Jekyll documentation site at docs/ — versioned API docs with Swagger UI embed. |
| [lib/algorithm-factory.md](lib/algorithm-factory.md) | IAlgorithmFactory — how algorithm instances are created, DI registration, and how tests construct algorithms directly |
| [lib/algorithms.md](lib/algorithms.md) | Packing heuristics (FFD/WFD/BFD) — versions, operation types, trade-offs, and the fit/pack guarantee |
| [lib/models.md](lib/models.md) | Lib model types and IWith* interfaces — Bin, Item, packed/unpacked results, and the constraints used in generic type parameters |
| [lib/processors.md](lib/processors.md) | IAlgorithmProcessor, IBinProcessor, and IMultiAlgorithmBinProcessor — their factories and which algorithms each execution path uses |
| [lib/README.md](lib/README.md) | Binacle.Lib and Binacle.Lib.Abstractions — the algorithm layer |
| [lib/result-building.md](lib/result-building.md) | OperationResultBuilder — how OperationResult is constructed, status rules, volume percentages, and integrity checks |
| [lib/result-selection.md](lib/result-selection.md) | IResultSelector, IResultSelectionStrategy, and the three selection strategies — scoring rules, tie-breaking, and how tests verify them |
| [lib/tests.md](lib/tests.md) | lib/test projects — unit tests, performance tests, benchmarks; AlgorithmFactories, CommonTestingFixture, ResultSelectionTestingFixture, and run aliases |
| [packages/binacle-net-ui.md](packages/binacle-net-ui.md) | packages/binacle-net-ui — Alpine.js components + Three.js visualizer for the packing demo. Components, plugins, model layers, and the window.binacle global. |
| [packages/README.md](packages/README.md) | TypeScript packages under packages/ (npm workspaces) — UI components, cookie utilities, and theme switching. |
| [README.md](README.md) | Repo overview and index of agent documentation |
| [ruby/README.md](ruby/README.md) | Ruby gems under ruby/ — Jekyll plugins used by docs/ and web/ sites. |
| [samples/README.md](samples/README.md) | Deployment samples — Docker Compose (minimal, ui, service-npgsql, service-azure) and Kubernetes (minimal); feature flags, config wiring, and the keep-in-sync rule |
| [shared/README.md](shared/README.md) | Shared slice — Binacle.TestsKernel (scenario data, compact-string formats, providers, fixtures) and shared/data (OR-Library benchmark data) |
| [vipaq/README.md](vipaq/README.md) | Binacle.ViPaq — compact binary format for encoding packing results. Wire layout, encoding-info header, C# API surface, and limits. |
| [vipaq/typescript.md](vipaq/typescript.md) | Binacle.ViPaq TypeScript mirror (vipaq/binacle-vipaq) — public API, how it differs from the C# library, and the known buffer-size bug |
| [web/README.md](web/README.md) | Jekyll marketing/landing site at web/ — product home, apps listing, and interactive packing demo. |
