---
description: Bin processors, algorithm processors, factories, and result selection
---

# Processors

## IBinacleService

Defined in `src/Binacle.Net/Services/BinacleService.cs`. The API layer calls this — it does not touch processors directly.

| Method | What it does |
|---|---|
| `SingleBinAsync(algorithm, bin, items, params)` | Runs one algorithm on one bin directly via `IAlgorithmFactory` |
| `SingleBinAsync(bin, items, params)` | Runs all algorithms on one bin via `IAlgorithmProcessorFactory`, picks `BestAlgorithm` |
| `MultipleBinsAsync(algorithm, bins, items, params)` | Runs one algorithm on each bin via `IBinProcessorFactory.Create` |
| `MultipleBinsAsync(bins, items, params)` | Runs all algorithms on each bin via `IBinProcessorFactory.CreateMultiAlgorithm` |
| `SmallestBinAsync(algorithm, bins, items, params)` | Same as multiple explicit, then picks `SmallestBin` |
| `SmallestBinAsync(bins, items, params)` | Same as multiple auto-select, then picks `SmallestBin` |

## BinProcessorFactory

`Create(binCount, itemCount)` → `LoopBinProcessor`
- Takes an explicit algorithm, loops bins, runs that algorithm on each

`CreateMultiAlgorithm(binCount, itemCount)` → `LoopMultiAlgorithmBinProcessor`
- No algorithm specified; runs FFD + BFD per bin via `LoopAlgorithmProcessor`, picks `BestAlgorithm` per bin

## AlgorithmProcessorFactory

`Create(itemCount)` → `LoopAlgorithmProcessor` with FFD + WFD + BFD
- Runs all three algorithms on a single bin

## Result Selection

`IResultSelector` has three strategies:

| Strategy | Used by |
|---|---|
| `BestAlgorithm` | `SingleBinAsync` auto-select; `LoopMultiAlgorithmBinProcessor` per bin |
| `SmallestBin` | `SmallestBinAsync` — picks smallest successful bin from results |
| `BestBin` | Available on the interface; not currently called by the service |

## Notes

- Parallel variants (`ParallelBinProcessor`, `ParallelAlgorithmProcessor`, etc.) exist but are not wired up by the factories — currently unused.
- `BinProcessorFactory.CreateMultiAlgorithm` uses FFD + BFD only.
  `AlgorithmProcessorFactory.Create` uses FFD + WFD + BFD.
