---
id: lib
description: Binacle.Lib — the algorithm layer, the only project in lib/src
verified: 2026-08-13
check: Project list and test aliases match the solution
also_update:
  - lib/tests
  - shared
---

# Lib

If you don't know where to start, read `$lib/processors` first.

One src project:

- `lib/src/Binacle.Lib` — the algorithm code, processors and result selection. Its `Abstractions/` folder holds
  the engine interfaces; the vocabulary a caller needs is `Binacle.Packing`, in `shared/src`.

Plus, in this slice: `lib/test/Binacle.Lib.TestsKernel` (the result-selection fixture kernel) and
`lib/data/result-selection` (the fixtures it embeds).

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
| `lib/test/Binacle.Lib.Benchmarks` | (none) | BenchmarkDotNet microbenchmarks — run via `./tooling/benchmarks.lib.sh [AlgorithmRacing\|FastValidation]` |

See Lib Tests (`$lib/tests`) for fixtures and the test projects, and Shared (`$shared`) for the
scenario data format and providers.

## Notes

- One custom exception: `DimensionException` in `lib/src/Binacle.Lib/Exceptions/` — use it rather than inventing new types.
- Guard clauses live in `lib/src/Binacle.Lib/GuardClauses/` (Null, NullOrEmpty, Dimensions, Volume, Quantity).

## Dependencies

The composition-root rule (only `Binacle.Net` references the packer) and the two tests kernels are in
`$lib/dependencies`.

## Concepts

This slice implements Fit vs Pack (`$concepts`).
