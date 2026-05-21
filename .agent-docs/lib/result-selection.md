---
description: IResultSelector, IResultSelectionStrategy, and the three selection strategies — scoring rules, tie-breaking, and how tests verify them
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
Called by `LoopMultiAlgorithmBinProcessor` (per bin) and by `BinacleService` — see [processors.md](processors.md).

`OperationResultStatus` includes `Unknown = -1` as a sentinel default. If you implement a new strategy,
guard against it — a result with `Unknown` status should never win.

## DI Registration

Registered as a singleton with v2 for all three strategies:

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

Currently **not called** by `BinacleService` — it exists on the interface but no endpoint uses it yet.

## How tests verify selection

`ResultSelectionTestingFixture.RunTest(scenarioName, strategy, resultSelector)` takes:
- a scenario name (from JSON test data under `Binacle.TestsKernel/ResultSelection/Data/`)
- the strategy to test
- a key extractor: `x => x.AlgorithmInfo.GetAlgorithmIdentifierName()` for BestAlgorithm,
  or `x => x.Bin.ID` for BestBin / SmallestBin

Each scenario provides a pre-built `IDictionary<string, OperationResult>` and an `ExpectedResult` key.
The test calls `Select()`, extracts the key, and asserts it equals `ExpectedResult`.
