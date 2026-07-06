# ViPaq — decisions ledger

Locked decisions and open questions, with the *why*. Evidence lives in [findings.md](findings.md); design detail in
[architecture-v2.md](architecture-v2.md). This file is the "what we settled and why" so a fresh session doesn't
re-litigate it.

## What we must reach (must-have)

The target for v2. A change only ships if it keeps all of these true:

- **Stays a small base64 text token.** Storage comes first. The stored form is the base64 string, and it must stay
  small. Everything else is measured against that.
- **Simpler than v1.** Two width tiers (8/16), not four. No huge value ceiling to reason about.
- **Round-trips exactly, in both C# and TypeScript.** Decode must give back the input. When not compressed, the
  bytes must be identical across the two languages.
- **Reads at least as fast as v1**, ideally faster (that is the point of the Session 2 decode fix). Never slower.
- **Keeps the public API small.** Do not grow the public surface. Anything needed only for tests stays internal.
- **Only ships if measured.** Smaller base64, or faster / less memory, or a clear simplicity win. No measured gain,
  no ship — the worth-it gate below.

## The worth-it gate (governs every decision here)

Every decision — a header bit, a codec, a layout, a feature — must answer **"is it worth it?"**, written as:
- **Cost:** effort + risk + complexity + cross-language churn (C#, TS, interop vectors).
- **Benefit:** measured, in the terms that matter — **base64 size** (the stored form), **encode/decode ms**, or a
  concrete simplicity/maintenance gain. "Might be nice" is not a benefit.
- **Verdict + why. Default to NO.** The format is already good; the bias is against churn.

## Locked

### D1 — v2 is `8/16 + reserved codes`, for simplicity (CONFIRMED 2026-07-05)
Varint deferred to Session 7, may never happen. **Not a size play** — 8/16 = v1 on ≤16-bit data, so ~0% smaller.
The payoff is a **simpler format** (2 tiers not 4; no 2⁵³ ceiling to reason about) and a clean base if varint is
ever wanted. Do not sell "20% smaller" — that was a Brotli-q11 artifact (see findings).

### D2 — 16-bit cap in v2.0; throw above 65,535
Fixed 8/16 caps at 65,535. v2.0 throws above it; varint (Session 7) lifts the cap later. mm → 65 m, fine for
physical bins. Retire/repoint `ViPaqLimits.MaxInteger` (2⁵³−1) to the new ceiling in Session 4.

### D3 — Baselining without a v1/v2 pair (CONFIRMED 2026-07-07)
ViPaq has one implementation, so there's no in-code baseline like lib's v1-vs-v2 racing. Two mechanisms replace it:
- **Protobuf is the in-run anchor** — `[Benchmark(Baseline = true)]`. ViPaq is reported as a *ratio* to protobuf,
  so a rerun on another machine/day stays comparable; the anchor absorbs environment drift.
- **Committed result files are the recorded baseline** — size-report + BDN summary under `results/`. A *win* = a
  diff showing smaller base64 / lower ns / lower allocs **while the protobuf anchor is unchanged**. Small
  increments; keep only measured wins.
- Depends on the still-open results-storage decision in [../results-migration.md](../results-migration.md); this
  workflow assumes **Option C — stay in `results/`**.

### D4 — The permanent harness uses only the minimal public API (CONFIRMED 2026-07-07)
The permanent benchmark calls **only** `ViPaqSerializer.Serialize`/`Deserialize`. It never touches internals,
header bits, or layout. Two consequences:
- **Layout-agnostic for free** — when v2 swaps row→columnar internally, the bytes change but the harness call
  sites don't. This is automatic from living at the public boundary, not something the harness engineers.
- **It measures real behavior only** — compression is baked into `Serialize` (fixed 255-byte threshold, hard-wired
  gzip-Optimal; no caller control). So the harness can't set a compression/codec knob. It *detects* whether ViPaq
  compressed by reading byte 0's `Version` bit, then mirrors that on protobuf for a fair comparison.
- **Why minimal:** a permanent ruler must not churn when the lib evolves. Coupling it to an evolving API would
  defeat the point. Controlled codec/threshold experiments are a **separate one-off** (see D5), not the ruler.

### D5 — Two tiers: permanent ruler vs one-off experiment (CONFIRMED 2026-07-07)
- **Permanent harness** (Session 1): minimal public API, never changes, measures real-mode size + CPU/mem +
  protobuf ratio, and *observes* the shipped compression crossover by sweeping item count.
- **One-off experiment:** tunes the threshold / chooses the codec — the only questions needing a compression knob.
  Asked once, answer recorded in findings.md and baked into the lib. Not part of the permanent ruler. In v2 this
  experiment stops being throwaway because it drives the stable directive seam (see architecture-v2.md D-arch).

### D6 — Scope: 8/16 only, no 32/64 in the permanent tool
32/64 is pointless to keep measuring — v2 drops it. Craft payloads whose values force ViPaq into 8- or 16-bit
selection; don't try to benchmark widths v2 won't have.

## Open — decide with data

### O1 — Compression trigger (Session 1 owns it)
Recommend **try-both-keep-smaller** (smallest token, no threshold to tune, never inflates tiny tokens). Simpler
fallback: a fixed byte threshold (currently 255 B in the lib; findings say brotli pays from ~150 B, gzip from
~400 B). Lock from the crossover data in Session 1.

### O2 — Codec + level (deferred; decide later with data)
Build the harness codec-agnostic. Findings: gzip-Optimal ≈ brotli-Optimal on size, both fast; **never q11 as
default** (~98 ms encode — archival opt-in only). Run the codec-tradeoff experiment, then lock.

## Ruled out — do not rebuild
24-bit ladder (8/16/24/32) + coords-ride-bin · Brotli q11 as default · byte-plane/transpose layout · raw Deflate
as a third codec · selling "20% smaller". See [findings.md](findings.md) for the numbers behind each.
