# Binacle.Lib

The core 3D bin-packing engine. Pure C# with no API or web dependencies — given a bin and a list of
items, it works out what fits (**fit**) and where each item goes (**pack**).

## Projects

| Path | What it is |
|---|---|
| `src/Binacle.Lib.Abstractions` | Interfaces only — shared by `Binacle.Lib` and the API layer. No dependencies. |
| `src/Binacle.Lib` | The algorithms, processors, and result building. |

## How it works

Two operations, same result shape (packed items + unpacked items):

- **Fit** — stops at the first item that does not fit. A fast yes/no answer.
- **Pack** — keeps going and returns the position of every placed item.

Packing uses three heuristics — First-Fit Decreasing (FFD), Worst-Fit Decreasing (WFD), and
Best-Fit Decreasing (BFD), each with versioned variants. A result selector then picks the best
outcome across algorithms or across bins.

## Layout

| Folder | What it provides |
|---|---|
| `Algorithms/` | The FFD / WFD / BFD heuristics and their versions |
| `AlgorithmProcessing/` | Runs one algorithm over a bin |
| `BinProcessing/` | Runs algorithms across one or many bins |
| `AlgorithmFactories/` | Creates algorithm instances |
| `ResultSelection/` | Picks the best result (best algorithm, smallest bin, best bin) |
| `Models/` | Bin, Item, packed/unpacked result types |
| `GuardClauses/` | Input checks — null, dimensions, volume, quantity |
| `Exceptions/` | `DimensionException` |

## Tests

| Project | Run with | Covers |
|---|---|---|
| `test/Binacle.Lib.UnitTests` | `./config/tests.sh lib` | All algorithm versions × scenarios; result selection |
| `test/Binacle.Lib.PerformanceTests` | `./config/tests.sh performance` | Algorithm performance (console runner) |
| `test/Binacle.Lib.Benchmarks` | `./config/benchmarks.sh` | BenchmarkDotNet microbenchmarks |
