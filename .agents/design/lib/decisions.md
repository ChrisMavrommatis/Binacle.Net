---
id: lib/decisions
description: Lib decisions ledger — why Algorithm.Best races a different set per path, and the open parallelization question.
verified: 2026-07-17
check: Algorithm sets match AlgorithmProcessorFactory.Create and BinProcessorFactory.CreateMultiAlgorithm
also_update:
  - lib/findings
---

# Lib — decisions ledger

Locked decisions and open questions for `lib/src`, with the *why*. Measured evidence lives in `$lib/findings`.
This file is the "what we settled and why", so a fresh session does not re-litigate it or "fix" a deliberate
choice.

## Locked

### D1 — `Algorithm.Best` races a different set depending on the path

**This is deliberate. Do not "align" the two.** The same parameter value means two things, on purpose:

| `Best` on | Path | Races |
|---|---|---|
| `fit/bin`, `pack/bin` | `SingleBinAsync` → `AlgorithmProcessorFactory.Create` | FFD, WFD, BFD |
| `compare-bins`, `smallest-bin`, `best-bin` | `BinProcessorFactory.CreateMultiAlgorithm` | **FFD, BFD** |

**Why:** WFD is not worth choosing for auto **across many bins**. Adding it to FFD+BFD costs **+131% to +373%**
— roughly 2.3× to 4.7× the run time (`$lib/findings#F1`) — for an algorithm that rarely produces the winner.
That cost is paid **per bin**, so on a compare across a preset's bins it multiplies by the bin count. One bin
can afford the third algorithm; many bins cannot.

The asymmetry is the point: the single-bin path buys a small extra chance of a better answer for a bounded
cost, and the multi-bin path refuses the same trade because the cost is no longer bounded.

**Consequence for the docs:** any page describing `Best` must say **which set the route uses**, and why WFD is
dropped. "Runs all algorithms" is wrong on the multi-bin routes.

## Open

### O1 — the `Parallel*` processors are unreachable

`BinProcessorFactory.Create` and `CreateMultiAlgorithm` take `binCount` and `itemCount` and **ignore both** —
they always return the `Loop` variants. Nothing in `lib/src` or `api/src` constructs `ParallelBinProcessor`,
`ParallelAlgorithmProcessor`, or `ParallelMultiAlgorithmBinProcessor`; only the benchmarks do.

The signature promises a decision that is never made.

**And the measurement argues against wiring it up.** On the set production actually uses (FFD+BFD), parallel
*algorithm* racing runs **0.93× to 1.48×** — slower than `Loop` on the cheapest scenario, and only clearly
ahead where the two algorithms take very unequal time (`$lib/findings#F2`). Two algorithms cap the win at 2×
before overhead. **D1 is what makes this so: the decision that makes racing cheap is the decision that makes
parallelising it pointless.**

The untested axis is `ParallelBinProcessor` — many *bins* at once, which scales with bin count rather than
algorithm count. That is the one that could still pay, and it has no finding yet.

**Undecided:** wire the threshold up, or delete the classes. Leaving three unreachable processors in place
invites someone to "fix" a path that never runs in production. Also `ParallelBinProcessor.concurrencyLevel`
only sizes the `ConcurrentDictionary` — it never reaches `MaxDegreeOfParallelism`, so the name overpromises.
