---
description: Bin processors, algorithm processors, factories, and result selection
---

# Processors

The API layer calls `IBinacleService` — it does not touch processors directly.
See [service.md](../api/service.md) for the full method table and how to call it from an endpoint.

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
