# Results Migration Plan

Status: **deferred — decisions needed before any work starts**

`results/` (renamed from `doc/`) holds benchmark reports and packing efficiency analysis.
Currently raw markdown sitting in the repo. Goal is to surface this content somewhere useful.

> **Dependent (2026-07-07):** the ViPaq Session-1 benchmark will record its size-report + BDN summary under
> `results/` and diff each rerun against the committed baseline (protobuf as the in-run anchor). That workflow
> assumes **Option C — stay in `results/`**. If Decision 1 picks A or B, reconcile with the "Baselining without a
> v1/v2 pair" section in [vipaq/README.md](vipaq/README.md).

---

## Current Contents

### `results/benchmark-results/`

Dated benchmark reports (hand-written summaries):
`2024-02-20.md`, `2024-04-12.md`, `2024-08-30.md`, `2024-09-03.md`, `2024-09-11.md`,
`2024-09-12.md`, `2024-09-15.md`, `2024-10-15.md`, `2024-10-18.md`, `2024-11-15.md`,
`2024-11-22.md`, `2024-11-24.md`, `2024-11-27.md`, `2024-11-27_WFD.md`,
`2024-12-09_BFD.md`, `2024-12-12.md`, `2025-02-10.md`

Raw BenchmarkDotNet GitHub-flavoured markdown reports (auto-generated):
- `results_net9/` — 10 files (FFD/BFD/WFD, fitting + packing, multiple bins/items)
- `results_net9_windows/` — same 10 files, Windows run
- `results_net9_10Installed/` — same 10 files, .NET 9 with .NET 10 also installed
- `results_net10/` — same 10 files, .NET 10 run
- `results_net10_windows/` — same 10 files, .NET 10 Windows run

Also: `benchmark-results.proj` (MSBuild project), `README.md` (empty stub)

### `results/packing-efficiency-results/`

Named analysis docs:
- `PackingEfficiency.md` — efficiency analysis write-up
- `PackingEfficiencyComparison.md` — comparison across algorithms
- `PackingTime.md` — timing analysis

Dated snapshots: `2024-11-24.md`, `2024-11-27.md`, `2024-11-27_WFD.md`,
`2024-12-09_BFD.md`, `2025-02-10.md`

Also: `packing-efficiency-results.proj` (MSBuild project), `README.md` (empty stub)

---

## Decision 1 — Where does the content live?

- **Option A: docs site** — add a "Performance" or "Benchmarks" section under the Jekyll docs site
- **Option B: web site** — data-driven page on the marketing/web site
- **Option C: stay in `results/`** — keep as raw markdown, just add a better README (least effort)

---

## Decision 2 — What happens to the raw BenchmarkDotNet files?

- Keep raw JSON/MD in `results/raw/` as an archive
- Or discard and only keep rendered output in the site

---

## Tasks (fill in once decisions above are made)

- [ ] Decide Option A / B / C
- [ ] If A or B: move or link benchmark markdown into the site's collections or pages
- [ ] If A or B: add navigation entry in the site's `_data/` header/footer config
- [ ] If A or B: decide what to do with raw result files
- [ ] If C: write a README explaining the folder contents and file format
