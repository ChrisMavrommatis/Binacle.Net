---
description: Manifest of every file under .agents/docs, grouped by area. Regenerate with just agents all.
---

# Agent Docs Index

Every doc in `.agents/docs/`, grouped by area. Scan for your topic, then read that file — do not work
from this summary. Task-based entry points ("I want to add a v4 endpoint") are in the "Common Tasks"
table of [README.md](README.md).

## General

```yaml
- file: build-topology.md
  description: "Build & workspace topology — the .slnx solution, npm workspaces, gulp asset copy, Directory.Build.props (including the SonarQubeTestProject rule for support projects), central package management, the global.json test-runner opt-in, the publish/Dockerfile chain, and the NoTargets content projects"
  paths: ["Binacle.Net.slnx", "Directory.*.props", "global.json", "**/*.csproj", "Dockerfile"]
- file: commands.md
  description: "How to set up a clone, run the API and the two sites, run tests and benchmarks, and build the Docker image"
  paths: ["justfile", "tooling/**"]
- file: concepts.md
  description: "Fit exits early on first failure; pack continues and returns positions. Both return the same result shape — packed items and unpacked items. Used by both Lib algorithms and API endpoints."
  paths: ["lib/src/Binacle.Lib/Algorithms/**"]
```

## API

```yaml
- file: api/configuration.md
  description: "Config file layout, env-var conventions, override precedence, and feature flag list"
  paths: ["api/src/Binacle.Net/Config_Files/**", "api/src/Binacle.Net/Configuration/**"]
- file: api/dependencies.md
  description: "API slice dependency tree — Binacle.Net as composition root, the Kernel floor, the always-compiled modules (Diagnostics, Service, UI), the ServiceModule clean-architecture split, and who sees internals."
  paths: ["api/**"]
- file: api/endpoints.md
  description: "Endpoint pattern, registration, request validation flow, and route groups for v3 and v4"
  paths: ["api/src/Binacle.Net/v3/**", "api/src/Binacle.Net/v4/**"]
- file: api/kernel.md
  description: "Binacle.Net.Kernel — shared patterns used by all API projects and modules"
  paths: ["api/src/Binacle.Net.Kernel/**"]
- file: api/modules/diagnostics.md
  description: "DiagnosticsModule — always-on logging, OpenTelemetry, health checks, and packing logs"
  paths: ["api/src/Binacle.Net.DiagnosticsModule/**"]
- file: api/modules/README.md
  description: "Optional module system — feature flags, structure, available modules"
  paths: ["api/src/Binacle.Net.*Module*/**"]
- file: api/modules/service.md
  description: "ServiceModule — JWT auth, rate limiting, account/subscription management. Three projects using clean architecture."
  paths: ["api/src/Binacle.Net.ServiceModule/**", "api/src/Binacle.Net.ServiceModule.Domain/**", "api/src/Binacle.Net.ServiceModule.Infrastructure/**"]
- file: api/modules/ui.md
  description: "UIModule — optional Razor Pages demo host. Routes, the webpack and sass build, the applet list, and how error pages are decided."
  paths: ["api/src/Binacle.Net.UIModule/**"]
- file: api/openapi.md
  description: "OpenAPI wiring — IOpenApiDocument, the Kernel transformers (JWT, 429, response descriptions, enum-as-string), what endpoint groups auto-wire, and the external OpenApiExamples package"
  paths: ["api/src/**/OpenApi/**"]
- file: api/presets.md
  description: "What presets are, where they're configured, how route params map to bins, and how to add one for tests"
  paths: ["api/src/Binacle.Net/Configuration/BinPresetOptions.cs", "api/src/Binacle.Net/Config_Files/Presets.json"]
- file: api/README.md
  description: "Index for API slice docs — endpoints, contracts, service, kernel, presets, and module docs (Diagnostics, ServiceModule, UIModule)"
  paths: ["api/**"]
- file: api/service.md
  description: "IBinacleService — method reference for SingleBinAsync, MultipleBinsAsync, SmallestBinAsync, BestBinAsync; return types, call pattern, and algorithm selection"
  paths: ["api/src/Binacle.Net/Services/**"]
- file: api/tests.md
  description: "api/test integration tests — layout, v3/v4 HTTP conventions, validBinId, preset keys, special bins, base-class asserts, and test host config"
  paths: ["api/test/**"]
- file: api/v3/contracts.md
  description: "v3 request and response contracts — field names, outer response wrapper, and enum values for fit and pack."
  paths: ["api/src/Binacle.Net/v3/Contracts/**"]
- file: api/v3/README.md
  description: "v3 API — stable, do not modify. Endpoints, algorithm selection, response shape, field names, and enum values."
  paths: ["api/src/Binacle.Net/v3/**"]
- file: api/v4/add-endpoint.md
  description: "Step-by-step guide for adding a new v4 endpoint"
  paths: ["api/src/Binacle.Net/v4/**"]
- file: api/v4/contracts.md
  description: "Request/response contract types, validators, and OpenAPI examples for v4 (v3 follows the same shape)"
  paths: ["api/src/Binacle.Net/v4/Contracts/**"]
- file: api/v4/README.md
  description: "v4 API — active development. Endpoints, algorithm selection, parameters, contracts, and response shape."
  paths: ["api/src/Binacle.Net/v4/**"]
```

## CI/CD

```yaml
- file: ci-cd/README.md
  description: "CI/CD — the nine GitHub Actions workflows in .github/workflows and the nine shared actions in .github/actions, what triggers each, the conventions they all follow, and the repo variables, secrets and environments they need"
  paths: [".github/workflows/**", ".github/actions/**"]
- file: ci-cd/release-pipeline.md
  description: "The release pipeline in release-docker-image.yml — seven jobs from a pushed tag to a published GitHub release, GHCR as the staging registry, the copy-to-Docker-Hub step every tag reaches with a prerelease narrowed to its immutable tag, the CHANGELOG.md release body, and the Docker Hub page written last"
  paths: [".github/workflows/**"]
```

## Lib

```yaml
- file: lib/algorithm-factory.md
  description: "IAlgorithmFactory — how algorithm instances are created, DI registration, and how tests construct algorithms directly"
  paths: ["lib/src/Binacle.Lib/AlgorithmFactor*"]
- file: lib/algorithms.md
  description: "Packing heuristics (FFD/WFD/BFD) — versions, operation types, trade-offs, and the fit/pack guarantee"
  paths: ["lib/src/Binacle.Lib/Algorithms/**"]
- file: lib/dependencies.md
  description: "Lib slice dependency tree — Binacle.Lib as the single src project, its own result-selection tests kernel, who sees internals (IVT), and the composition-root rule (only Binacle.Net references the packer)."
  paths: ["lib/**"]
- file: lib/models.md
  description: "Lib model types and IWith* interfaces — Bin, Item, packed/unpacked results, and the constraints used in generic type parameters"
  paths: ["lib/src/Binacle.Lib/Models/**", "shared/src/Binacle.Packing/**", "shared/src/Binacle.Geometry/**"]
- file: lib/processors.md
  description: "IAlgorithmProcessor, IBinProcessor, and IMultiAlgorithmBinProcessor — their factories and which algorithms each execution path uses"
  paths: ["lib/src/Binacle.Lib/AlgorithmProcessing/**", "lib/src/Binacle.Lib/BinProcessing/**"]
- file: lib/README.md
  description: "Binacle.Lib — the algorithm layer, the only project in lib/src"
  paths: ["lib/**"]
- file: lib/result-building.md
  description: "OperationResultBuilder — how OperationResult is constructed, status rules, volume percentages, and integrity checks"
  paths: ["lib/src/Binacle.Lib/Models/OperationResultBuilder.cs", "shared/src/Binacle.Packing/Models/**"]
- file: lib/result-selection.md
  description: "IResultSelector, IResultSelectionStrategy, and the three selection strategies — scoring rules, tie-breaking, and how tests verify them"
  paths: ["lib/src/Binacle.Lib/ResultSelection/**"]
- file: lib/tests.md
  description: "lib/test projects — unit tests, performance tests, benchmarks; AlgorithmFactories, CommonTestingFixture, ResultSelectionTestingFixture, and run aliases"
  paths: ["lib/test/**"]
```

## Packages

```yaml
- file: packages/binacle-net-ui.md
  description: "packages/binacle-net-ui — Alpine.js components + Three.js visualizer for the packing demo. Components, plugins, model layers, and the window.binacle global."
  paths: ["packages/**"]
- file: packages/dependencies.md
  description: "TypeScript packages dependency tree — the npm workspaces and which package imports (and declares) which."
  paths: ["packages/**"]
- file: packages/README.md
  description: "TypeScript packages under packages/ (npm workspaces) — UI components, compact-notation mirror, cookie utilities, and theme switching."
  paths: ["packages/**"]
```

## Ruby

```yaml
- file: ruby/README.md
  description: "Ruby gems under ruby/ — Jekyll plugins used by the two sites under sites/."
  paths: ["ruby/**"]
```

## Samples

```yaml
- file: samples/README.md
  description: "Deployment samples — Docker Compose (minimal, quickstart, prod, service, full) and Kubernetes (minimal); each folder name is a smoke profile name, feature flags, config wiring, and the keep-in-sync rule"
  paths: ["samples/**"]
```

## Shared

```yaml
- file: shared/dependencies.md
  description: "Shared slice dependency tree — Geometry (the BCL-only leaf everything geometric bottoms out on), CompactNotation, Packing, TestReporting, and the algorithm TestsKernel; who references them and who sees internals."
  paths: ["shared/**"]
- file: shared/README.md
  description: "Shared slice — Binacle.TestsKernel (algorithm scenario data, compact-string formats, providers, fixtures) and shared/data (the fixture corpus more than one slice reads)"
  paths: ["shared/**"]
```

## Sites

```yaml
- file: sites/demo.md
  description: "The published Jekyll demo site at sites/demo/ — product home, apps listing, and interactive packing demo. `$sites/demo` always means sites/demo/."
  paths: ["sites/demo/**"]
- file: sites/docs.md
  description: "The published Jekyll documentation site at sites/docs/ — versioned API docs with Swagger UI embed. `$sites/docs` always means sites/docs/, never .agents/docs/."
  paths: ["sites/docs/**"]
- file: sites/README.md
  description: "Every published site lives under sites/, one directory each. What the two share, and what is per-site."
  paths: ["sites/**"]
```

## Tooling

```yaml
- file: tooling/README.md
  description: "tooling/ — every task the repo can run, called by CI and by hand alike: the test, coverage, openapi, agents, regen, changelog, serve, build, check, image and smoke modules for just, the benchmark/performance scripts, the tmux script, the local compose stacks, and emulator state"
  paths: ["tooling/**"]
```

## ViPaq

```yaml
- file: vipaq/architecture.md
  description: "ViPaq architecture — the blind encode/decode layer, the layout codecs, and the serializer that chooses. The policy/mechanism split the rebuild keeps."
  paths: ["vipaq/**"]
- file: vipaq/cross-language-testing.md
  description: "ViPaq cross-language wire testing — the C#/TS shared-vector apparatus, its inventory, and the decode-to-input contract"
  paths: ["vipaq/**"]
- file: vipaq/dependencies.md
  description: "ViPaq project dependency tree — who references whom, who can see internals, and the deliberate walls (UnitTests never references TestsKernel; no test project references a generator)."
  paths: ["vipaq/**"]
- file: vipaq/README.md
  description: "Binacle.ViPaq — compact binary format for packing results. The wire is defined in PROTOCOL.md; this covers the C# API surface, repo layout, and tests."
  paths: ["vipaq/**"]
- file: vipaq/typescript.md
  description: "Binacle.ViPaq TypeScript mirror (vipaq/packages/binacle-vipaq) — public API and how it differs from the C# library"
  paths: ["vipaq/**"]
```
