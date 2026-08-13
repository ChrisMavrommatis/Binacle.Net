---
name: results-curated
description: results/ is a hand-curated vault — harnesses write to gitignored scratch, never straight into results/
type: convention
when: writing anything into results/
paths:
  - "results/**"
---

`results/` holds committed benchmark and size evidence, organized by slice — `lib/` and `vipaq/`, each split into
`benchmarks/` (BenchmarkDotNet speed) and a quality folder (`efficiency/` fill rate, `compression/` encoded size).
Every folder has a README; `results/README.md` is canonical.

**Nothing auto-writes here.** Every harness — the `PerformanceTests` projects and the BenchmarkDotNet `Benchmarks`
projects — writes its raw run to its own build-local `*.Artifacts` folder (gitignored scratch), pinned to the
project directory so it never wanders to the repo root. You diff scratch against the committed files and copy the
keepers in by hand: a current-best report plus dated `YYYY-MM-DD.md` snapshots for milestones.

**Why:** results/ is a curated ledger for showing improvement and anchoring a baseline, not a dump. Auto-writing
would dirty the tree on every run and bake machine-specific timings into the committed baseline.

**How to apply:** never point a harness's file writer at `results/`. Write to a project-local
`PerformanceTests.Artifacts` (resolve via `AppContext.BaseDirectory`), or pin BenchmarkDotNet's `ArtifactsPath` the
same way; both names are already gitignored. For BDN, export markdown only (`MarkdownExporter.GitHub` from
`ManualConfig.CreateEmpty()` plus the copied-back defaults) so there's one clean `.md` to curate. ViPaq's
diff-against-baseline rule is `$vipaq#D3`.
