---
id: lib/result-selection
description: IResultSelector, IResultSelectionStrategy, and the three selection strategies — scoring rules, tie-breaking, and how tests verify them
verified: 2026-08-19
check: Strategy class names, scoring rules and the strict > comparison match lib/src/Binacle.Lib/ResultSelection/; the DI registration matches api/src/Binacle.Net/ExtensionMethods/ServiceCollectionExtensions.cs; the fixture signature and Scenario members match lib/test/Binacle.Lib.UnitTests/ResultSelectionTestingFixture.cs and the TestsKernel Scenario
also_update:
  - api/service
paths:
  - "lib/src/Binacle.Lib/ResultSelection/**"

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

`OperationResultStatus` includes `Unknown = -1` as a sentinel default. **None of the three strategies checks
for it** — they only ever test `== FullyPacked`, so an `Unknown` result carrying a high percentage would score
like any other partial one. They are safe because nothing produces `Unknown`, not because they defend against
it. Keep that in mind before you make one reachable.

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

**On an exact tie the first entry wins**, and `BestBin_v2` compares the same way. The test is
`score > bestScore`, strictly, so a later result never displaces an equal earlier one — which means the answer
depends on the dictionary's enumeration order. In practice that is insertion order: the processors only ever
add, and a `Dictionary<,>` with no removals enumerates in insertion order. So a tie between two heuristics
resolves to whichever came first in the factory's algorithm array — FFD, WFD, BFD on the single-bin path,
FFD, BFD on the multi-bin one (`$lib/processors`). **`Dictionary<,>` does not promise that order**, so treat
it as what happens today rather than as a contract; if a tie ever has to resolve a particular way, the
strategy has to say so itself.

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

Each `Scenario` carries a `Name`, a `Results` dictionary parsed from compact strings by
`OperationResultHelper.ParseManyFromCompactStrings`, and the `ExpectedResult` key.
`Select` calls `Select()` on the strategy and extracts the key; the test itself asserts it equals
`ExpectedResult`, so the comparison stays visible in the test body.
