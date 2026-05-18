---
description: Binacle.Lib and Binacle.Lib.Abstractions — the algorithm layer
---

# Lib

Two projects:

- `src/Binacle.Lib.Abstractions` — interfaces only; used by both `Binacle.Lib` and the API layer
- `src/Binacle.Lib` — the actual algorithm code and processors

## Docs

- [Algorithms](algorithms.md) — heuristics, versions, operation types
- [Algorithm Factory](algorithm-factory.md) — IAlgorithmFactory, DI registration, how tests construct algorithms
- [Processors](processors.md) — IAlgorithmProcessor, bin processors, factories, algorithm sets per path
- [Result Building](result-building.md) — OperationResultBuilder, status rules, volume percentages
- [Result Selection](result-selection.md) — BestAlgorithm, SmallestBin, BestBin strategies and scoring

## Related Tests

| Project | Alias | What it covers |
|---|---|---|
| `test/Binacle.Lib.UnitTests` | `lib` | All algorithm versions × all scenarios; result selection strategies |
| `test/Binacle.Lib.PerformanceTests` | `performance` | Algorithm performance (console runner, not xUnit) |

See [Tests](../tests/README.md) for stack, fixture patterns, and scenario data format.

## Concepts

This slice implements [Fit vs Pack](../concepts/fit-vs-pack.md).
