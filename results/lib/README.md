# Binacle.Lib results

Measured output for the packing algorithms. Two kinds, one per folder:

| Folder | What it measures | Written by |
|---|---|---|
| [benchmarks/](benchmarks/) | Raw speed — fit and pack, per algorithm (FFD/BFD/WFD), across runtimes and machines | `lib/test/Binacle.Lib.Benchmarks` (`./tooling/benchmarks.lib.sh`) |
| [efficiency/](efficiency/) | How well each algorithm fills a bin, and how long it takes | `lib/test/Binacle.Lib.PerformanceTests` (`./tooling/performance.lib.sh`) |

Both harnesses write to a build-local artifacts folder first (`BenchmarkDotNet.Artifacts` /
`PerformanceTests.Artifacts`, gitignored). Nothing writes here automatically — you copy the reports worth keeping
into these folders by hand, following the scratch-vs-curated convention in the [parent README](../README.md):
overwrite the current best, and add a dated snapshot when a run is worth marking.
