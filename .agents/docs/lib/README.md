---
description: Binacle.Lib and Binacle.Lib.Abstractions — the algorithm layer
verified: 2026-06-10
check: Project list and test aliases match the solution
also_update:
  - lib/tests.md
  - shared/README.md
---

# Lib

If you don't know where to start, read [processors.md](processors.md) first.

Two projects:

- `lib/src/Binacle.Lib.Abstractions` — interfaces only; used by both `Binacle.Lib` and the API layer
- `lib/src/Binacle.Lib` — the actual algorithm code and processors

## Docs

- [Models](models.md) — Bin, Item, IWith* interfaces, packed/unpacked result types
- [Algorithms](algorithms.md) — heuristics, versions, operation types
- [Algorithm Factory](algorithm-factory.md) — IAlgorithmFactory, DI registration, how tests construct algorithms
- [Processors](processors.md) — IAlgorithmProcessor, bin processors, factories, algorithm sets per path
- [Result Building](result-building.md) — OperationResultBuilder, status rules, volume percentages
- [Result Selection](result-selection.md) — BestAlgorithm, SmallestBin, BestBin strategies and scoring
- [Lib Tests](tests.md) — unit/perf/benchmark projects, AlgorithmFactories, fixtures

## Related Tests

| Project | Alias | What it covers |
|---|---|---|
| `lib/test/Binacle.Lib.UnitTests` | `lib` | All algorithm versions × all scenarios; result selection strategies |
| `lib/test/Binacle.Lib.PerformanceTests` | `performance` | Algorithm performance (console runner, not xUnit) |
| `lib/test/Binacle.Lib.Benchmarks` | (none) | BenchmarkDotNet microbenchmarks — run via `./config/benchmarks.lib.sh [AlgorithmRacing\|FastValidation]` |

See [Lib Tests](tests.md) for fixtures and the test projects, and [Shared](../shared/README.md) for the
scenario data format and providers.

## Notes

- One custom exception: `DimensionException` in `lib/src/Binacle.Lib/Exceptions/` — use it rather than inventing new types.
- Guard clauses live in `lib/src/Binacle.Lib/GuardClauses/` (Null, NullOrEmpty, Dimensions, Volume, Quantity).

## Concepts

This slice implements [Fit vs Pack](../concepts.md).
