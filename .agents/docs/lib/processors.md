---
id: lib/processors
description: IAlgorithmProcessor, IBinProcessor, and IMultiAlgorithmBinProcessor — their factories and which algorithms each execution path uses
verified: 2026-07-15
check: Interface names and Process() signatures match lib/src/Binacle.Lib/
also_update:
  - api/service
paths:
  - "lib/src/Binacle.Lib/AlgorithmProcessing/**"
  - "lib/src/Binacle.Lib/BinProcessing/**"

---

# Processors

## Two axes

Pick the right processor by bins × algorithms:

| | One algorithm | Many algorithms |
|---|---|---|
| **One bin** | `IAlgorithmProcessor` (explicit) | `IAlgorithmProcessor` (auto — via `AlgorithmProcessorFactory`) |
| **Many bins, one algorithm** | `IBinProcessor` → `LoopBinProcessor` | — |
| **Many bins, many algorithms** | — | `IMultiAlgorithmBinProcessor` → `LoopMultiAlgorithmBinProcessor` |

`IBinProcessor` and `IMultiAlgorithmBinProcessor` are separate interfaces.
`IBinProcessor.Process` takes an explicit `Algorithm` argument; `IMultiAlgorithmBinProcessor.Process` does not.

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

> The `services.AddSingleton<...>` registrations shown below live in the **API** project
> (`api/src/Binacle.Net/ExtensionMethods/ServiceCollectionExtensions.cs`), not in `lib/`. The factory classes
> themselves are in `lib/src/Binacle.Lib/`.

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

All active processors start an OpenTelemetry activity via `Diagnostics.ActivitySource` (`lib/src/Binacle.Lib/Diagnostics.cs`).
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
| `BestBin` | On the interface; not currently called by the service — see `$lib/result-selection` |

See `$lib/result-selection` for scoring rules and how tests verify each strategy.

## Parallel variants

`ParallelAlgorithmProcessor` (`lib/src/Binacle.Lib/AlgorithmProcessing/`) and `ParallelBinProcessor` /
`ParallelMultiAlgorithmBinProcessor` (`lib/src/Binacle.Lib/BinProcessing/`) exist but are **not wired up** by any factory.
Do not use them until they are.
