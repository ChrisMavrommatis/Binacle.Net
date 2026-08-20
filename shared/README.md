# Shared

Code and data that more than one slice uses. Nothing here is a product of its own - if only one slice reads
it, it belongs in that slice instead.

## 📂 What is in it

| Folder | What it is |
|---|---|
| `src/` | Three small libraries the whole repo compiles against - geometry, packing vocabulary, compact notation |
| `test/` | Test infrastructure shared by several suites - the scenario kernel, the report writer |
| `tools/` | The OR-Library converter, which writes `data/bischoff-suite` |
| `data/` | The fixture corpus more than one slice reads - see [`data/README.md`](data/README.md) |

## 📦 The libraries

Plain C# libraries, no framework and no I/O. They exist so `lib`, `api` and `vipaq` agree on the same types
rather than each defining their own.

| Project | What it is | Used by |
|---|---|---|
| `src/Binacle.Geometry` | Dimensions, coordinates, volume and quantity - the `IWith*` interfaces plus concrete `Dimensions`, `Coordinates`, `Item` | `Binacle.Packing`, `Binacle.CompactNotation`, ViPaq |
| `src/Binacle.Packing` | The packing vocabulary - `Algorithm`, `AlgorithmInfo`, `PackedBin`, `PackedItem`, `UnpackedItem`, `OperationResultStatus` | `Binacle.Lib`, the API, the test kernels |
| `src/Binacle.CompactNotation` | Parses and formats the terse strings used in fixtures and API payloads - `"60x40x30"`, `"108x76x30 [40]"` | The API, both test kernels, every generator |

`Binacle.Packing` and `Binacle.CompactNotation` both build on `Binacle.Geometry`; nothing points the other way.

## 🧪 Test infrastructure

### 🧩 `Binacle.TestsKernel`

Shared test library. Not a test runner - it provides the types and scenario data that several test projects
depend on.

| Folder | What it provides |
|---|---|
| `Models/` | `TestBin`, `TestItem`, `TestOperationParameters`, `Dimensions` - concrete types implementing the `IWith*` interfaces for use in tests |
| `Algorithms/` | `ScenarioCollectionsProvider`, `MultipleScenarioCollectionsProvider`, `ScenarioReader`, `CollectionKeys` - load and expose the JSON scenario data as xUnit `[MemberData]` |
| `Files/` | `EmbeddedResourceFile`, `EmbeddedResourceFileProvider` - read embedded JSON scenario files from the assembly |
| `TestAlgorithmFactory.cs` | Delegate-based factory for constructing algorithm instances directly in unit tests |
| `PercentageComparer.cs` | Custom equality comparer for floating-point volume percentages |

Its scenario JSON is **not** on disk here - it is linked in from `data/`, so edit the file under
[`data/`](data) and the kernel picks it up.

| Project | What it uses |
|---|---|
| `lib/test/Binacle.Lib.UnitTests` | Scenario providers, TestBin/TestItem, TestAlgorithmFactory |
| `lib/test/Binacle.Lib.Benchmarks` | TestBin/TestItem, scenario data |
| `lib/test/Binacle.Lib.PerformanceTests` | TestBin/TestItem, scenario data |
| `api/test/Binacle.Net.IntegrationTests` | Scenario providers for HTTP integration tests |

Result-selection fixtures are **not** here. They have one consumer, so they live in
[`lib/data/result-selection`](../lib/data/result-selection) with their own kernel.

### 📄 `Binacle.TestReporting`

The markdown report writer behind the performance suites. Register `ITest` implementations and an
`IFileWriter` in DI, and `TestRunner` runs each test, logs it, and groups the results into one file per report.
Used by `lib/test/Binacle.Lib.PerformanceTests`, `vipaq/test/Binacle.ViPaq.PerformanceTests` and both data
generators. The reports it writes are the ones committed under [`results/`](../results).

### 🔤 `Binacle.CompactNotation.UnitTests`

Parse, format and round-trip tests for `src/Binacle.CompactNotation`. Runs with `just test shared-cs-unit`.
The TypeScript twin of that parser lives in
[`packages/binacle-compact-notation`](../packages/binacle-compact-notation) and runs with `just test shared-ts-unit`.

## 🛠️ Tools

`tools/Binacle.OrLibrary.Converter` turns the raw OR-Library text into the scenario JSON under
`data/bischoff-suite`.

```bash
just regen or-lib-scenarios
```

It takes no arguments and always runs every converter, so it cannot half-run and leave the fixtures
inconsistent. It rewrites committed files - run it only when you meant to.

## 📊 Data

[`data/`](data) holds the fixture corpus: the Bischoff suite, the custom problems, and the raw OR-Library
source they come from. Each folder has its own README with the file format.
