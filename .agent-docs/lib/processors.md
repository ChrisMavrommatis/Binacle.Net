---
description: Algorithm and bin processors — IAlgorithmProcessor, BinProcessorFactory, algorithm sets per execution path, result selection, and diagnostics
---

# Processors

## What it does

`IAlgorithmProcessor` runs multiple algorithms against a single bin and returns all results.

```csharp
IAlgorithmProcessor.Process<TBin, TItem>(bin, items, parameters)
    → IDictionary<string, OperationResult>
```

Keys in the returned dictionary are algorithm identifier names (e.g., `"FFD_v2"`) from `GetAlgorithmIdentifierName()`.

## LoopAlgorithmProcessor

The only active implementation. Takes an array of `Algorithm` values and runs each in order using `IAlgorithmFactory`.

## Factories

### AlgorithmProcessorFactory

Creates a `LoopAlgorithmProcessor` with **FFD + WFD + BFD**. Used by `BinacleService.SingleBinAsync(bin, items, params)`.

```csharp
services.AddSingleton<IAlgorithmProcessorFactory, AlgorithmProcessorFactory>();
```

### BinProcessorFactory

Creates bin-level processors. Two factory methods:

| Method | Returns | Algorithms used |
|---|---|---|
| `Create(binCount, itemCount)` | `LoopBinProcessor` | One explicit algorithm (caller provides it) |
| `CreateMultiAlgorithm(binCount, itemCount)` | `LoopMultiAlgorithmBinProcessor` | **FFD + BFD only** (no WFD) |

`LoopMultiAlgorithmBinProcessor` runs its inner `LoopAlgorithmProcessor` on each bin and applies
`IResultSelector.BestAlgorithm` to pick one result per bin.

```csharp
services.AddSingleton<IBinProcessorFactory, BinProcessorFactory>();
```

## Algorithm sets — important distinction

| Used by | Algorithms |
|---|---|
| `AlgorithmProcessorFactory` (single bin, auto) | FFD + WFD + BFD |
| `BinProcessorFactory.CreateMultiAlgorithm` (multi bin, auto) | FFD + BFD |

WFD is excluded from the multi-bin path. This is intentional.

## Diagnostics

All active processors start an OpenTelemetry activity via `Diagnostics.ActivitySource` (`src/Binacle.Lib/Diagnostics.cs`).
If you add a new processor, follow the same pattern:

```csharp
using var activity = Diagnostics.ActivitySource.StartActivity("Process <Something>: Loop");
activity?.SetTag("Operation", parameters.Operation);
```

Activity names used by existing processors: `"Process Algorithms: Loop"`, `"Process Bins: Loop"`,
`"Process Multi Algorithm Bins: Loop"`.

## Result Selection

After processors produce results, `IResultSelector` picks one. Quick reference:

| Strategy | Used by |
|---|---|
| `BestAlgorithm` | `SingleBinAsync` auto-select; `LoopMultiAlgorithmBinProcessor` per bin |
| `SmallestBin` | `SmallestBinAsync` — picks smallest successful bin |
| `BestBin` | On the interface; not currently called by the service |

See [result-selection.md](result-selection.md) for scoring rules and how tests verify each strategy.

## Parallel variants

`ParallelAlgorithmProcessor` and `ParallelBinProcessor` / `ParallelMultiAlgorithmBinProcessor` exist in the codebase
but are **not wired up** by any factory. Do not use them until they are.
