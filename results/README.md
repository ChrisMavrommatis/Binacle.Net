# Results

Measured evidence for the project — benchmark output, packing-efficiency analysis, and the ViPaq size reports.
Kept in the repo so a change can be diffed against a known baseline, and so there is one place to point at when
showing how a result was reached. These are **records, not source** — nothing reads them at build time.

Organized by slice, the same way `docs/` is:

| Folder | What |
|---|---|
| [lib/](lib/) | `Binacle.Lib` — algorithm speed (`benchmarks/`) and how well each fills a bin (`efficiency/`) |
| [vipaq/](vipaq/) | `Binacle.ViPaq` — encode/decode speed (`benchmarks/`) and encoded size vs protobuf (`compression/`) |

Both slices split the same way: **`benchmarks/`** is BenchmarkDotNet speed/allocation, and the second folder is
the slice's own quality metric — fill efficiency for the packer, encoded size for the format.

## Scratch vs curated

**Nothing here is auto-written.** Every harness — the `PerformanceTests` projects and the BenchmarkDotNet
`Benchmarks` projects — writes its raw run into its own build-local `*.Artifacts` folder next to the project
(gitignored scratch, overwritten each run). This committed vault holds only what you **copy in by hand**. So a
benchmark run never dirties `results/`; you decide what lands.

What you keep is two kinds of thing:

- **Current best (updated).** The representative report for where things stand — `PackingEfficiency.md`,
  `VipaqProtobufSizeComparison.Deflate.md`. Overwrite it when a run is worth promoting; `git diff` shows the move.
- **Dated history (added).** A snapshot dropped in as `YYYY-MM-DD.md` when a run marks a milestone — a real
  improvement, a new algorithm. A suffix names the one thing that changed (`2024-12-09_BFD.md`). This is the
  *"improvement over time"* ledger.

**To record a run:** diff the scratch output against what's committed here; if it's a win, copy it in. **Do not
date every run** — date when the story is worth telling, and write a line or two on *what changed and why the
numbers moved*. Snapshot everything and the folder fills with noise; the meaningful jump drowns in re-runs.

## Comparing numbers honestly

Only compare within the same ruler. Speed and time depend on the runtime and the machine — a Windows run and a
Linux run, or net9 and net10, are not the same measurement. Fill-rate and encoded size are stable across
machines and can be compared freely.
