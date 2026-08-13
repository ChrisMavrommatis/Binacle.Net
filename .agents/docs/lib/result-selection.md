---
id: lib/result-selection
description: IResultSelector, IResultSelectionStrategy, and the three selection strategies — scoring rules, tie-breaking, and how tests verify them
verified: 2026-07-24
check: Strategy class names and scoring rules match lib/src/Binacle.Lib/
also_update:
  - api/service
---

# Result Selection

## Interfaces

`IResultSelector` is what `BinacleService` calls. It has three methods:

```csharp
OperationResult BestAlgorithm(IDictionary<string, OperationResult> results)
OperationResult SmallestBin(IDictionary<string, OperationResult> results)
OperationResult BestBin(IDictionary<string, OperationResult> results)
```

Each delegates to an `IResultSelectionStrategy`:

```csharp
IResultSelectionStrategy.Select(IDictionary<string, OperationResult> results) → OperationResult
```

All strategies: throw `ArgumentException` if the dictionary is empty; return immediately if it has one entry.
Called by `LoopMultiAlgorithmBinProcessor` (per bin) and by `BinacleService` — see `$lib/processors`.

`OperationResultStatus` includes `Unknown = -1` as a sentinel default. If you implement a new strategy,
guard against it — a result with `Unknown` status should never win.

## DI Registration

Registered as a singleton with v2 for all three strategies. This registration lives in the **API** project
(`api/src/Binacle.Net/ExtensionMethods/ServiceCollectionExtensions.cs`), not in `lib/`:

```csharp
services.AddSingleton<IResultSelector>(sp => new ResultSelector(
    bestBin:       new BestBin_v2(),
    smallestBin:   new SmallestBin_v2(),
    bestAlgorithm: new BestAlgorithm_v2()
));
```

## Strategies

### BestAlgorithm_v2

Used by: `BinacleService.SingleBinAsync` (auto-select) and `LoopMultiAlgorithmBinProcessor` (per bin).

Scoring: `score = (FullyPacked ? 1000 : 0) + PackedItemsVolumePercentage`

Picks the highest score. The +1000 bonus means any fully-packed result always beats a partial one.

### SmallestBin_v2

Used by: `BinacleService.SmallestBinAsync`.

Priority order:
1. FullyPacked beats non-FullyPacked
2. Among same status: smaller bin volume wins
3. Tie on volume: higher `PackedItemsVolumePercentage` wins

### BestBin_v2

Same scoring as `BestAlgorithm_v2` but uses `PackedBinVolumePercentage` instead of `PackedItemsVolumePercentage`.

Used by: `BinacleService.BestBinAsync`, behind `pack/best-bin` and `pack/best-bin/{preset}` (`$api/v4`).

It agrees with `SmallestBin_v2` whenever some bin packs fully: the +1000 bonus puts fully-packed results first
in both, and among those the least roomy bin is also the most filled. They diverge only when nothing packs
fully — this one then takes the highest utilization, `SmallestBin_v2` the least volume.

## How tests verify selection

`ResultSelectionTestingFixture.GetScenarioByName(scenarioName)` resolves the scenario (from JSON test data
under `lib/data/result-selection/`, embedded by `lib/test/Binacle.Lib.TestsKernel` under the manifest prefix
`ResultSelection.` — see `$lib/tests`).

`ResultSelectionTestingFixture.Select(scenario, strategy, resultSelector)` then takes:
- that scenario
- the strategy to test
- a key extractor: `x => x.AlgorithmInfo.GetAlgorithmIdentifierName()` for BestAlgorithm,
  or `x => x.Bin.ID` for BestBin / SmallestBin

Each scenario provides a pre-built `IDictionary<string, OperationResult>` and an `ExpectedResult` key.
`Select` calls `Select()` on the strategy and extracts the key; the test itself asserts it equals
`ExpectedResult`, so the comparison stays visible in the test body.
