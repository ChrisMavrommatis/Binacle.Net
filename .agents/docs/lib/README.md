---
id: lib
description: Binacle.Lib and Binacle.Lib.Abstractions — the algorithm layer
verified: 2026-07-15
check: Project list and test aliases match the solution
also_update:
  - lib/tests
  - shared
---

# Lib

If you don't know where to start, read `$lib/processors` first.

Two projects:

- `lib/src/Binacle.Lib.Abstractions` — interfaces only; used by both `Binacle.Lib` and the API layer
- `lib/src/Binacle.Lib` — the actual algorithm code and processors

## Docs

- Models (`$lib/models`) — Bin, Item, IWith* interfaces, packed/unpacked result types
- Algorithms (`$lib/algorithms`) — heuristics, versions, operation types
- Algorithm Factory (`$lib/algorithm-factory`) — IAlgorithmFactory, DI registration, how tests construct algorithms
- Processors (`$lib/processors`) — IAlgorithmProcessor, bin processors, factories, algorithm sets per path
- Result Building (`$lib/result-building`) — OperationResultBuilder, status rules, volume percentages
- Result Selection (`$lib/result-selection`) — BestAlgorithm, SmallestBin, BestBin strategies and scoring
- Lib Tests (`$lib/tests`) — unit/perf/benchmark projects, AlgorithmFactories, fixtures

## Related Tests

| Project | Alias | What it covers |
|---|---|---|
| `lib/test/Binacle.Lib.UnitTests` | `lib` | All algorithm versions × all scenarios; result selection strategies |
| `lib/test/Binacle.Lib.PerformanceTests` | `performance` | Algorithm performance (console runner, not xUnit) |
| `lib/test/Binacle.Lib.Benchmarks` | (none) | BenchmarkDotNet microbenchmarks — run via `./config/benchmarks.lib.sh [AlgorithmRacing\|FastValidation]` |

See Lib Tests (`$lib/tests`) for fixtures and the test projects, and Shared (`$shared`) for the
scenario data format and providers.

## Notes

- One custom exception: `DimensionException` in `lib/src/Binacle.Lib/Exceptions/` — use it rather than inventing new types.
- Guard clauses live in `lib/src/Binacle.Lib/GuardClauses/` (Null, NullOrEmpty, Dimensions, Volume, Quantity).

## Dependencies

The Abstractions/Lib split and the composition-root rule (only `Binacle.Net` references the concrete lib) are in
`$lib/dependencies`.

## Concepts

This slice implements Fit vs Pack (`$concepts`).
