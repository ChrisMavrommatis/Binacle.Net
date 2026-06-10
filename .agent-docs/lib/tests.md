---
description: lib/test projects — unit tests, performance tests, benchmarks; AlgorithmFactories, CommonTestingFixture, ResultSelectionTestingFixture, and run aliases
verified: 2026-06-10
check: Project list, AlgorithmFactories/CommonTestingFixture/ResultSelectionTestingFixture, and aliases match lib/test/ and config/tests.sh + config/benchmarks.sh
also_update:
  - shared/README.md
  - lib/algorithm-factory.md
  - lib/result-selection.md
---

# Lib Tests

Three projects under `lib/test/`. Scenario data and the `TestAlgorithmFactory<>` delegate come from the shared
kernel — see [shared](../shared/README.md).

| Project | Kind | Run |
|---|---|---|
| `Binacle.Lib.UnitTests` | xUnit | `./config/tests.sh lib` |
| `Binacle.Lib.PerformanceTests` | console host (writes markdown reports) | `./config/tests.sh performance` |
| `Binacle.Lib.Benchmarks` | BenchmarkDotNet | `./config/benchmarks.sh [FastValidation\|AlgorithmRacing]` |

## Binacle.Lib.UnitTests

`AlgorithmFactories.cs` (in this project) defines six `TestAlgorithmFactory<IPackingAlgorithm>` statics —
`FFD_v1/_v2`, `WFD_v1/_v2`, `BFD_v1/_v2` — each constructing the algorithm directly
(`new FirstFitDecreasing_v2<TestBin, TestItem>(bin, items)`), **not** through `IAlgorithmFactory`/DI.
This keeps every version (including v1) under test without coupling it to the production factory.

`CommonTestingFixture` holds all six factories in `AlgorithmsUnderTest[]` and exposes:

```csharp
RunTest(TestAlgorithmFactory<IPackingAlgorithm> factory, string scenarioName, AlgorithmOperation operation)
```

It resolves the scenario from the kernel's `Algorithms` `AllScenariosProvider`, builds the algorithm, calls
`Execute(new TestOperationParameters { Operation })`, then asserts via `scenario.Metrics.EvaluateResult` and
`scenario.Result.EvaluateResult`. Test classes: `FittingBischoffSuiteTests`, `FittingCustomProblemsTests`,
`PackingBischoffSuiteTests`, `PackingCustomProblemsTests` (each a `[Theory]` × `[MemberData]` over all six
versions), plus `CreationTests`, `SanityTests`, `ResultSelectionTests`.

`ResultSelectionTestingFixture`:

```csharp
RunTest(string scenarioName, IResultSelectionStrategy strategy, Func<OperationResult, string> resultSelector)
```

Pulls the scenario from the kernel's `ResultSelection` `AllScenariosProvider`, calls `strategy.Select(results)`,
applies `resultSelector`, and asserts it equals `scenario.ExpectedResult`. `ResultSelectionTests` runs both
strategy versions: `BestAlgorithm_v1/v2` (selector `x => x.AlgorithmInfo.GetAlgorithmIdentifierName()`),
`BestBin_v1/v2` and `SmallestBin_v1/v2` (selector `x => x.Bin.ID`). See [result-selection.md](result-selection.md).

## Binacle.Lib.PerformanceTests

Console host (not xUnit). Has its own copy of the six `AlgorithmFactories`. `Program.cs` wires a `TestRunner` +
`MarkdownFileWriter` and runs Bischoff-suite `ITest` implementations: `PackingEfficiencyTests`, `RegressionTests`
(FFD/WFD/BFD v1-vs-v2), `EfficiencyStatisticsTests`, `BaselineComparisonTests`. Output is markdown reports, not
pass/fail assertions.

## Binacle.Lib.Benchmarks

BenchmarkDotNet. Two factory paths:

- Bischoff-suite benchmarks use the project's own six `TestAlgorithmFactory` statics via `BischoffSuiteBenchmarkBase`.
- AlgorithmProcessing benchmarks use the lib's **internal** `AlgorithmFactory_v1()` / `AlgorithmFactory_v2()`
  (`lib/src/Binacle.Lib/AlgorithmFactories/`) fed into `LoopAlgorithmProcessor` / `ParallelAlgorithmProcessor`.

Families: Fitting + Packing × {BischoffSuite, FastValidation}, Packing × {AlgorithmProcessing (AlgorithmRacing,
AlgorithmParallelizationThreshold), BinProcessing (BinParallelizationThreshold)}, and ResultSelection. Ordering
via `Order/AttributeOrderer` + `[BenchmarkOrder]`. Filter with `./config/benchmarks.sh FastValidation` or
`AlgorithmRacing`; no argument runs all.
