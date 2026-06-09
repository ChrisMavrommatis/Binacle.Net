---
description: All test projects — stack, aliases, and what each covers
verified: 2026-06-09
check: Project list and aliases match solution test projects
also_update:
  - lib/README.md
---

# Tests

If you don't know where to start, read [scenarios.md](scenarios.md) first.
For run commands and aliases, see [Commands](../commands.md).

## Stack

All test projects use xUnit v3 + Shouldly. Random data uses Bogus. Run with `dotnet run`.

## Test Projects

### `Binacle.Lib.UnitTests` (alias: `lib`)

Tests the algorithm library directly — no HTTP.

| Class | Trait | What it covers |
|---|---|---|
| `SanityTests` | Sanity Tests | Test wiring check |
| `CreationTests` | Behavioral Tests | Exceptions on bad input: null bin, null/empty items, zero dimensions |
| `FittingBischoffSuiteTests` | Scenario Tests | All algorithm versions × all Bischoff Suite scenarios, fitting mode |
| `PackingBischoffSuiteTests` | Scenario Tests | All algorithm versions × all Bischoff Suite scenarios, packing mode |
| `FittingCustomProblemsTests` | Scenario Tests | All algorithm versions × custom scenarios, fitting mode |
| `PackingCustomProblemsTests` | Scenario Tests | All algorithm versions × custom scenarios, packing mode |
| `ResultSelectionTests` | Scenario Tests | `BestAlgorithm`, `BestBin`, and `SmallestBin` strategies, v1 and v2 |

Each scenario test is parameterized via `[MemberData]` — one test case per scenario per algorithm version.

### `Binacle.Net.IntegrationTests` (alias: `api`)

HTTP tests using `WebApplicationFactory<IApiMarker>`. `BinacleApi` wraps it and exposes an `HttpClient`.
Runs in `"Test"` mode with logging off and test presets loaded.

Tests are organized by API version (`Tests/v3/`, `Tests/v4/`) with two types:
- **Behavior tests** — check HTTP status codes: 200, 422, 400, 404
- **Scenario tests** — check response payloads (bin results, item lists, early exit statuses)

For the conventions these tests follow (v3 array vs v4 single result, naming, hardcoded bin IDs,
test data), see [integration-conventions.md](integration-conventions.md).

### `Binacle.Net.ServiceModule.IntegrationTests` (alias: `api_service`)

Integration tests for the ServiceModule (auth, rate limiting).
Tests live under `Endpoints/Admin/` and `Endpoints/Auth/`.

### `Binacle.ViPaq.UnitTests` (alias: `vipaq`)

Tests for the ViPaq binary format. Covers encoding/decoding and roundtrip behavior.

### `Binacle.Lib.PerformanceTests` (alias: `performance`)

Console runner (not xUnit) for algorithm performance tests. Outputs to console and file.
Separate from the BenchmarkDotNet benchmarks in `Binacle.Lib.Benchmarks`.

### `Binacle.Lib.Benchmarks`

BenchmarkDotNet runner for algorithm comparison. Uses internal `AlgorithmFactory_v1` and `AlgorithmFactory_v2`
to race algorithm versions against each other. Not xUnit — run directly via `dotnet run`.

## Shared Infrastructure

See [scenarios.md](scenarios.md) for test data format, suites, and providers.
