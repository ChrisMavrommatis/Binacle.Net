---
id: lib/findings
description: Lib findings — the measured evidence (algorithm racing cost, parallel racing gain) behind the decisions.
verified: 2026-07-17
check: Numbers match the latest AlgorithmRacing_Packing_v2 report in lib/test/Binacle.Lib.Benchmarks/BenchmarkDotNet.Artifacts/results/
also_update:
  - lib/decisions
---

# Lib — findings (the measured evidence)

The measured truth behind `$lib/decisions`. **No session keeps its own numbers** — they live here.

Ranges show the effect, not a guarantee.

## Environment

BenchmarkDotNet v0.15.8 · Linux Ubuntu 26.04 · AMD Ryzen 9 9900X, 12 physical cores · .NET 10.0.9 · measured
2026-07-17 from `AlgorithmRacing_Packing_v2`. Within-run error is **0.3–1.0%** of the mean, so the effects
below are far outside the noise.

Racing benchmarks run **one bin** and race algorithms against each other. They say nothing about running many
**bins** in parallel — that is a different axis, covered by the `Parallelization` benchmark.

## F1 — WFD roughly triples the cost of a race

Adding WFD to the FFD+BFD pair, single bin, `Loop`:

| Scenario | FFD,BFD | FFD,BFD,WFD | Cost of WFD |
|---|---|---|---|
| Baseline | 73.9μs | 170.7μs | **+131%** |
| BFD dominance | 119.5μs | 291.4μs | **+144%** |
| High efficiency | 353.0μs | 997.3μs | **+183%** |
| WFD weakness | 112.9μs | 357.3μs | **+217%** |
| Max complexity | 204.9μs | 968.8μs | **+373%** |

Roughly 2.3× to 4.7× the time for a third algorithm that rarely produces the winner. This is the evidence for
`$lib/decisions#D1`: the cost is paid **per bin**, so across a compare it multiplies by the bin count. One bin
can afford it; many bins cannot.

Reproduced 2026-07-17 against the 2026-07-14 run (which predates the cancellation-token commit): the effect
holds in the same shape and size on every scenario.

## F2 — parallel *algorithm* racing is marginal on the production set

`Parallel` vs `Loop`, racing FFD+BFD on one bin — the set production actually uses:

| Scenario | Loop | Parallel | Speedup |
|---|---|---|---|
| Baseline | 73.9μs | 79.2μs | **0.93× (slower)** |
| High efficiency | 353.0μs | 343.4μs | 1.03× |
| WFD weakness | 112.9μs | 104.7μs | 1.08× |
| Max complexity | 204.9μs | 186.7μs | 1.10× |
| BFD dominance | 119.5μs | 80.5μs | 1.48× |

**Two algorithms cap the win at 2× before any overhead**, and on the cheapest scenario the thread handoff
costs more than it saves. Parallel racing only clearly pays on `BFD dominance`, where the two algorithms take
very unequal time.

This is why `$lib/decisions#O1` is an open question rather than an obvious "wire it up": on the set production
uses, parallel racing is close to free but close to worthless. Wider sets look better (the discarded WFD-heavy
combinations gain more) — but those are the sets D1 rules out. **The decision that makes racing cheap is the
same decision that makes parallelising it pointless.**

Not measured here: `ParallelBinProcessor` (many bins at once), which scales with bin count rather than with
the number of algorithms. That is the one that might matter, and it has no finding yet.

## Note — the cancellation-token guard has no measurable cost

`8a7580f3` added `cancellationToken.ThrowIfCancellationRequested()` to each loop iteration in
`LoopAlgorithmProcessor` and friends — the exact hot path these benchmarks measure.

Comparing FFD,BFD `Loop` before (2026-07-14) and after (2026-07-17): **−2.0%, −1.4%, −0.7%, +2.0%, +6.2%**
across the five scenarios. **The signs are mixed**, which is the point: a real per-iteration tax would push
every scenario the same way. This is run-to-run variance, not a regression.

**Caveat:** these are two separate runs on different days, not a controlled A/B, and the machine was in use
during the second. Within-run error is ~1%, so the ±2% moves are between-run variance. If a definitive answer
is ever needed, stash the guard and run both back to back.

## F3 — v3 fitting on the packing lineage matches the old fitting family

The v3.0.0 release unified fitting and packing onto one algorithm: fitting stopped running its own family
(`Binacle.Lib/Fitting/Algorithms/*/`, version 3) and now runs the packing lineage (version 2) with early exit.
Fitting answers a yes/no question with a heuristic, so a different heuristic could disagree on edge cases —
worth confirming because it sits inside the **frozen v3 contract**.

Differential-tested 2026-07-19 against the real `binacle/binacle-net:2.1.1` image: **~5,400 fit requests** (random
bins/items, all three algorithms, weighted to the near-full boundary where heuristics diverge) ran old vs new
side by side — **identical answers every time, zero disagreements**. No behaviour change; no release-notes caveat
needed. (Old ViPaq tokens rejecting loudly is the other v3 verification — that one lives in `vipaq/PROTOCOL.md`
plus the committed regression vectors, not here.)
