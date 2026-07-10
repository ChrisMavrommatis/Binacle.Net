---
description: Binacle.ViPaq.TestsKernel — remaining alignment work. Core alignment is done (2026-07-09, see D10); this tracks what is left.
---

# ViPaq TestsKernel — remaining alignment work

Core alignment is **done** (2026-07-09): the kernel uses the house words (`Scenario`, not `Sample`), owns its own
`Files/` trio (so `Assembly.GetExecutingAssembly()` resolves to the assembly that embeds the data), and splits its
real data into `BischoffDataProvider` + `CustomProblemsDataProvider`, with curated siblings merged by
`CuratedScenarioProvider`. The reverted shared-`TestFiles` attempt and why it failed is [decisions.md](decisions.md)
D10; git holds the move itself. What is left:

## What remains

- **Second-algorithm collision (half-fixed).** The file provider knows each file's `Algorithm`, but the providers
  key scenarios by `Name` alone and filter only by `family`. The row `Name` carries no algorithm
  (`OrLibrary_thpack1_1`), so if a `.bfd.json` sibling ever lands next to `.ffd.json`, `Read(family)` returns both
  and `scenarios.Add(name, …)` throws a duplicate key. Fix when a second algorithm is actually generated: filter
  `Read` by `(family, algorithm)`, or fold the algorithm into the key. No `.bfd` data exists today.
- **The `.csproj` glob is `*.ffd.json`.** A second algorithm's `.bfd.json` files would not be embedded until the
  glob is widened (to `*.json`, or a second entry). Do this together with the collision fix above.
- **Synthetic is still a stub.** `SyntheticDataProvider` returns nothing. Rebuild it **CPU/memory only**, scaling to
  item counts no real pack reaches (2000, 5000) — see [decisions.md](decisions.md) D9 — and **only then** add a
  `SyntheticBenchmarkBase`. A base bound to the empty stub gives BDN nothing to discover.
- **Document the curated Bischoff picks.** The curated slice already covers both paths (an uncompressed 8-bit ladder
  in `CustomProblemsCuratedProvider.UncompressedNames` plus compressed packs). Still worth picking the two Bischoff
  names from the size report with a written reason each (cf. lib's `BischoffCuratedProblemsProvider`). Growing the
  underlying problem set is tracked in
  [../shared/testskernel-data-extraction.md](../shared/testskernel-data-extraction.md).

Delete this file once the above land.

## References

[README.md](README.md) · [decisions.md](decisions.md) (D9 synthetic scope, D10 no shared TestFiles) ·
[reference/01-benchmark-permanent.md](reference/01-benchmark-permanent.md).
