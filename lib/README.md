# Binacle.Lib

The core 3D bin-packing engine. Pure C# with no API or web dependencies — given a bin and a list of
items, it works out whether they fit (**fit**) and packs as many as it can (**pack**).

## Projects

| Path | What it is |
|---|---|
| `src/Binacle.Lib.Abstractions` | Interfaces only — shared by `Binacle.Lib` and the API layer. No dependencies. |
| `src/Binacle.Lib` | The algorithms, processors, and result building. |

## How it works

Both use the same algorithm. The difference is what happens when an item doesn't fit — and both
return the same result shape: packed items and unpacked items.

- **Fit** — answers "do all items fit?" Stops at the first item that doesn't fit, so it's fast. Because it
  exits early, anything after the failure is reported as unpacked even if it was never tried.
- **Pack** — runs through every item regardless, so you learn how much actually packed, not just whether
  it all fit.

Packing uses three heuristics — First Fit Decreasing (FFD), Worst Fit Decreasing (WFD), and
Best Fit Decreasing (BFD), each with two versions (v1 and v2; the API uses v2). A result selector
then picks the best outcome across algorithms or across bins.

## Layout

| Folder | What it provides |
|---|---|
| `Algorithms/` | The FFD / WFD / BFD heuristics and their versions |
| `AlgorithmProcessing/` | Runs several algorithms against a single bin |
| `BinProcessing/` | Runs algorithms across many bins |
| `AlgorithmFactories/` | Creates algorithm instances |
| `ResultSelection/` | Picks the best result (best algorithm, smallest bin, best bin) |
| `Models/` | Bin, Item, packed/unpacked result types |
| `GuardClauses/` | Input checks — null, dimensions, volume, quantity |
| `Exceptions/` | `DimensionException` |

## Tests

| Project | Run with | Covers |
|---|---|---|
| `test/Binacle.Lib.UnitTests` | `just test lib-unit` | All algorithm versions × scenarios; result selection |
| `test/Binacle.Lib.PerformanceTests` | `./config/performance.lib.sh` | Algorithm performance (console runner) |
| `test/Binacle.Lib.Benchmarks` | `./config/benchmarks.lib.sh` | BenchmarkDotNet microbenchmarks |
