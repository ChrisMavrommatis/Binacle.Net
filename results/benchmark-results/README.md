# Benchmark Results

Committed benchmark output for `Binacle.Lib`, kept in the repo so a rerun can be diffed against a known
baseline. These are records, not source — nothing reads them at build time.

| What | Files |
|---|---|
| Dated summaries, hand-written | `2024-02-20.md` … `2025-02-10.md` — one per notable run. A suffix names the algorithm that changed (`2024-11-27_WFD.md`, `2024-12-09_BFD.md`). |
| Raw BenchmarkDotNet reports | `results_net9/`, `results_net9_windows/`, `results_net9_10Installed/`, `results_net10/`, `results_net10_windows/` — GitHub-flavoured markdown, one file per benchmark class |

The raw folders are grouped by the runtime and machine they were measured on. Numbers only compare **within**
a folder — a Windows run and a Linux run are not the same ruler.

Regenerate with `./config/benchmarks.sh`, then copy the reports in and add a dated summary if the run is worth
keeping.
