---
id: lib/tests
description: lib/test projects — unit tests, performance tests, benchmarks; AlgorithmFactories, CommonTestingFixture, ResultSelectionTestingFixture, and run aliases
verified: 2026-08-09
check: Project list, AlgorithmFactories/CommonTestingFixture/ResultSelectionTestingFixture, and aliases match lib/test/ and config/tests.just + config/performance.lib.sh + config/benchmarks.lib.sh
also_update:
  - shared
  - lib/algorithm-factory
  - lib/result-selection
---

# Lib Tests

Three projects under `lib/test/`. Scenario data and the `TestAlgorithmFactory<>` delegate come from the shared
kernel — see shared (`$shared`).

| Project | Kind | Run |
|---|---|---|
| `Binacle.Lib.UnitTests` | xUnit | `just test lib-unit` |
| `Binacle.Lib.PerformanceTests` | console host (writes markdown reports) | `./config/performance.lib.sh` |
| `Binacle.Lib.Benchmarks` | BenchmarkDotNet | `./config/benchmarks.lib.sh [FastValidation\|AlgorithmRacing\|BischoffSuite\|Parallelization\|ResultSelection]` |

## Binacle.Lib.UnitTests

`AlgorithmFactories.cs` (in this project) defines six `TestAlgorithmFactory<IPackingAlgorithm>` statics —
`FFD_v1/_v2`, `WFD_v1/_v2`, `BFD_v1/_v2` — each constructing the algorithm directly
(`new FirstFitDecreasing_v2<TestBin, TestItem>(bin, items)`), **not** through `IAlgorithmFactory`/DI.
This keeps every version (including v1) under test without coupling it to the production factory.

Both fixtures split arrange, act and assert into separate members, so a test body shows all three steps
rather than handing them to one helper.

`CommonTestingFixture` holds all six factories in `AlgorithmsUnderTest[]` and exposes:

```csharp
Scenario GetScenarioByName(string scenarioName)
OperationResult Run(TestAlgorithmFactory<IPackingAlgorithm> factory, Scenario scenario, AlgorithmOperation operation)
void AssertResult(Scenario scenario, OperationResult result)
```

`GetScenarioByName` resolves from the kernel's `Algorithms` `AllScenariosProvider`. `Run` builds the algorithm
and calls `Execute(parameters)`, checking nothing. `AssertResult` does both checks —
`scenario.Metrics.EvaluateResult` and `scenario.Result.EvaluateResult` — and is marked `[AssertionMethod]`
so the analyser knows where the assertion lives. A test reads:

```csharp
var testScenario = this.Fixture.GetScenarioByName(scenario);

var result = this.Fixture.Run(AlgorithmFactories.FFD_v1, testScenario, AlgorithmOperation.Fitting);

this.Fixture.AssertResult(testScenario, result);
```

Test classes: `FittingBischoffSuiteTests`, `FittingCustomProblemsTests`, `PackingBischoffSuiteTests`,
`PackingCustomProblemsTests` (each a `[Theory]` × `[MemberData]` over all six versions), plus `CreationTests`,
`SanityTests`, `ResultSelectionTests`.

`ResultSelectionTestingFixture`:

```csharp
Scenario GetScenarioByName(string scenarioName)
string Select(Scenario scenario, IResultSelectionStrategy strategy, Func<OperationResult, string> resultSelector)
```

`GetScenarioByName` pulls from the kernel's `ResultSelection` `AllScenariosProvider`; `Select` calls
`strategy.Select(scenario.Results)` and applies `resultSelector`. There is no assert member here — the check
is a single comparison, so the test makes it itself with `selected.ShouldBe(scenario.ExpectedResult)`.
`ResultSelectionTests` runs both strategy versions: `BestAlgorithm_v1/v2` (selector
`x => x.AlgorithmInfo.GetAlgorithmIdentifierName()`), `BestBin_v1/v2` and `SmallestBin_v1/v2` (selector
`x => x.Bin.ID`). See `$lib/result-selection`.

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
via `Order/AttributeOrderer` + `[BenchmarkOrder]`. Filter with `./config/benchmarks.lib.sh FastValidation` or
`AlgorithmRacing`; no argument runs all.
