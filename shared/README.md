# Shared

Shared infrastructure used by test projects across the repo.

## `Binacle.TestsKernel`

Shared test library. Not a test runner — it provides types and data that multiple test projects depend on.

### What's in it

| Folder | What it provides |
|---|---|
| `Models/` | `TestBin`, `TestItem`, `TestOperationParameters`, `Dimensions`, `DimensionsAndQuantity` — concrete types implementing the `IWith*` interfaces for use in tests |
| `Algorithms/` | `ScenarioCollectionsProvider`, `MultipleScenarioCollectionsProvider`, `ScenarioReader`, `CollectionKeys` — load and expose the JSON scenario data as xUnit `[MemberData]` |
| `ResultSelection/` | Same pattern as `Algorithms/` but for result selection scenarios |
| `Files/` | `EmbeddedResourceFile`, `EmbeddedResourceFileProvider` — read embedded JSON scenario files from the assembly |
| `Helpers/` | `AlgorithmInfoHelper`, `DimensionHelper` — small utilities for test assertions |
| `TestAlgorithmFactory.cs` | Delegate-based factory for constructing algorithm instances directly in unit tests |
| `PercentageComparer.cs` | Custom equality comparer for floating-point volume percentages |

### Who uses it

| Project | What it uses |
|---|---|
| `lib/test/Binacle.Lib.UnitTests` | Scenario providers, TestBin/TestItem, TestAlgorithmFactory |
| `lib/test/Binacle.Lib.Benchmarks` | TestBin/TestItem, scenario data |
| `lib/test/Binacle.Lib.PerformanceTests` | TestBin/TestItem, scenario data |
| `api/test/Binacle.Net.IntegrationTests` | Scenario providers for HTTP integration tests |

## `data/`

Test datasets used by benchmarks and performance tests. See `data/or-library-packing-data/README.md`.
