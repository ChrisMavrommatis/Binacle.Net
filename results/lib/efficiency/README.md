# Packing Efficiency Results

How well each algorithm fills a bin, and how long it takes. Written by
`lib/test/Binacle.Lib.PerformanceTests` (`./config/performance.lib.sh`) and committed so a rerun can be
diffed against a known baseline. Records, not source.

| File | What it shows |
|---|---|
| `PackingEfficiency.md` | Volume used per algorithm — min, mean, median, max, as a percentage |
| `PackingEfficiencyComparison.md` | The same measure, one row per scenario, one column per algorithm |
| `PackingTime.md` | Time per algorithm — min, mean, median, max, in microseconds |
| `2024-11-24.md` … `2025-02-10.md` | Dated snapshots. A suffix names the algorithm that changed (`2024-11-27_WFD.md`, `2024-12-09_BFD.md`). |

Efficiency is stable across machines; the times in `PackingTime.md` are not. Compare times only against a run
on the same hardware.
