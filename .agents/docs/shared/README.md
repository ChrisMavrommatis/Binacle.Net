---
description: Shared slice — Binacle.TestsKernel (scenario data, compact-string formats, providers, fixtures) and shared/data (OR-Library benchmark data)
verified: 2026-07-05
check: Collection keys, compact-string parsers, and provider class names match shared/Binacle.TestsKernel; OR-Library files match shared/data
also_update:
  - lib/tests.md
  - api/tests.md
---

# Shared

`shared/` holds code used across more than one slice. Two parts:

- `shared/Binacle.TestsKernel` — shared test scenario infrastructure (data, parsers, providers, models, helpers)
- `shared/data` — raw OR-Library benchmark data (the upstream source for the embedded scenarios)

## Who uses Binacle.TestsKernel

Five project references:

- `lib/test/Binacle.Lib.UnitTests`
- `lib/test/Binacle.Lib.PerformanceTests`
- `lib/test/Binacle.Lib.Benchmarks`
- `api/test/Binacle.Net.IntegrationTests`
- `lib/src/Binacle.Lib.Abstractions` — note this is the **source** project, not a test project

`Binacle.Net.ServiceModule.IntegrationTests` and `vipaq/test/Binacle.ViPaq.UnitTests` do **not** use it —
they have their own self-contained fixtures (ServiceModule) or use Bogus fakers (ViPaq).

## Two independent areas

The kernel has two parallel namespaces, each with its own `CollectionKeys`, `Scenario`, `ScenarioReader`,
providers, and `Data/` folder:

- `Binacle.TestsKernel.Algorithms.*` — fit/pack scenarios (bin + items + expected metrics + expected result)
- `Binacle.TestsKernel.ResultSelection.*` — pre-built `OperationResult` sets for selection-strategy tests

## Scenario suites

Algorithms (`Algorithms/CollectionKeys.cs`):

| Key set | Members | Count |
|---|---|---|
| `BischoffSuite` | `BischoffSuite/orlib_thpack1` … `orlib_thpack7` | 7 |
| `CustomProblems` | `CustomProblems/baseline`, `/simple`, `/complex` | 3 |

ResultSelection (`ResultSelection/CollectionKeys.cs`) — one key each:
`BestAlgorithm/baseline`, `BestBin/baseline`, `SmallestBin/baseline`.

Data is embedded JSON under `Algorithms/Data/<suite>/` and `ResultSelection/Data/<suite>/`, loaded by resource
prefix. The collection key is `{folder}/{name}` lowercased.

## Compact-string formats

Scenario JSON keeps values terse. Each field has its own parser. Verify against the parser, not by guessing.

| Field | Parser | Rule | Real example |
|---|---|---|---|
| Dimensions | `TestBin.FromCompactString` / `TestItem.FromCompactString` (in `Models/`) via the shared `Binacle.CompactNotation` parser | `"LxWxH"` or `"LxWxH [Q]"` — the factory splits off the optional `[Q]` (quantity, default 1), then `CompactNotationParser.ParseDimensions<int>` → 3 ints L,W,H | `"108x76x30 [40]"`, `"60x40x10"` |
| Metrics (4) | `Algorithms/Helpers/ScenarioMetricsHelper.cs` | exactly 4 space-separated: `ItemsVolume BinVolume ItemsCount Percentage` (first 3 int, last decimal, trailing `%` trimmed) | `"29736390 30089620 112 98.83"` |
| Result (2) | `Algorithms/Helpers/ScenarioResultHelper.cs` | exactly 2 space-separated: **`parts[0]` = packing, `parts[1]` = fitting** | `"PartiallyPacked PartiallyPacked"` |
| OperationResult (5) | `ResultSelection/Helpers/OperationResultHelper.cs` | exactly 5: `Bin(LxWxH) Algorithm_vN Status BinPct ItemsPct` | `"60x40x30 FFD_v2 FullyPacked 95 100"` |
| AlgorithmInfo | `Helpers/AlgorithmInfoHelper.cs` | `"Name_vN"` — split on `_`: name = `Algorithm` enum (FFD/WFD/BFD), version after `v` | `"FFD_v2"` |

Both halves of the **result** string parse as the lib enum `OperationResultStatus`
(`Unknown=-1, FullyPacked, PartiallyPacked, NotPacked, EarlyExit`) — not the API fit/pack enums, so there is no
`AllItemsFit`/`NotAllItemsFit` here. A half may carry an early-exit reason as `Status-EarlyExitReason`
(`EarlyExitReason`: `None`, `ContainerVolumeExceeded`, `ContainerDimensionExceeded`); in real data only the
fitting half early-exits (pack never does), e.g. `"NotPacked EarlyExit-ContainerDimensionExceeded"`.

Metrics check: `ItemsVolume`, `BinVolume`, `ItemsCount` must match exactly; `Percentage` is an **upper bound** —
the actual `PackedBinVolumePercentage` must be ≤ expected, within a 0.1% tolerance (`PercentageComparer`).

## Providers

Static, lazily built, keyed by scenario `Name`. Each exposes `GetScenarioNames()`, `ScenarioNames`
(`IEnumerable<object[]>` for xUnit `[MemberData]`), `GetScenarios()`, `GetScenarioByName(name)`.

- Algorithms: `AllScenariosProvider` (Bischoff + Custom), `BischoffSuiteScenarioProvider`, `CustomProblemsScenarioProvider`
- ResultSelection: `AllScenariosProvider` (all three), `BestAlgorithmScenarioProvider`, `BestBinScenarioProvider`, `SmallestBinScenarioProvider`

## Models and helpers

Models: `TestBin` (`IWithID, IWithDimensions`), `TestItem` (`IWithID, IWithDimensions, IWithQuantity`),
`Dimensions`, `TestOperationParameters`. `TestBin`/`TestItem` each expose a `FromCompactString` factory (and a
`Binacle.CompactNotation.IWithDimensions<int>` ctor) that parse via the shared notation. Algorithms `Scenario`
carries bin + items +
`ScenarioMetrics` + `ScenarioResult`. ResultSelection `Scenario` carries `Name`, `ExpectedResult` (a bin-id
string), and `Results: Dictionary<string, OperationResult>`.

The kernel defines **no xUnit fixtures** — those live in the test projects (see [lib tests](../lib/tests.md)).
It provides:

- `TestAlgorithmFactory<TAlgorithm>` — `delegate TAlgorithm (TestBin bin, List<TestItem> items)`
- `EvaluateResult` extensions on `ScenarioMetrics` and `ScenarioResult` (the latter picks the packing vs fitting
  expected status by `result.AlgorithmOperation`, then throws on mismatch)
- `OperationResultExtensions` (volume/count totals) and `PercentageComparer` (0.1% tolerance)

## shared/data — OR-Library

`shared/data/or-library-packing-data/` holds the raw OR-Library container-loading text files
(`thpack1.txt` … `thpack9.txt`, plus `thpack9-fixed.txt` and a `README.md`). Source: OR-Library (J.E. Beasley);
`thpack1–7` are Bischoff & Ratcliff (1995). These raw files are the upstream origin that was converted into the
embedded `BischoffSuite/orlib_thpack1..7.json`. Only `thpack1–7` map to `BischoffSuite`; `thpack8/9` are not in
the embedded suite. `thpack9-fixed.txt` patches a missing indicator in thpack9 problems 18–20.
