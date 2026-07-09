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

### D7 — Compression trigger is **try-both-keep-smaller** (was O1; CONFIRMED 2026-07-08)
Compress, keep whichever is shorter, never inflate. Session 1 measured both sides and the fixed 255-byte threshold
the lib ships today is **wrong in both directions**: it inflates random data (gzip saved −8% to −0%) and would miss
small compressible data (real packed data saves 45–68%). A threshold cannot be tuned to fit both, because the right
answer depends on the data, not its size. Try-both has no knob to get wrong and can never inflate.

Cost: one extra compress pass on the encode path. Session 4 must measure it (encode is the priority — D8). If the
cost is unacceptable, the fallback is a threshold, and we are back to picking the wrong one. Evidence:
[findings.md](findings.md) Round 2.

### D8 — Encode speed is the priority; decode is second (CONFIRMED 2026-07-08)
ViPaq's job is to produce a token fast and store it; reads are rarer. **Optimise encode first.** Take decode wins
only when they are cheap — Session 2's span fix is exactly that, so it still belongs. Read the benchmark this way:
encode is the number that gates a change, decode is watch-not-block.

### D9 — Synthetic data measures CPU/memory; real data measures size (CONFIRMED 2026-07-08)
The two things we measure depend on different properties of the data.
- **CPU and memory** depend on item count and byte width, not on whether values repeat — encode/decode do the same
  work either way. So **synthetic random is fine, and preferred**: deterministic, scales freely to counts no real
  pack reaches (2000, 5000), and it deliberately exercises the expensive path — compression runs but does not help,
  so the encoder pays the cost and discards it. That wasted-gzip cost is real and worth measuring.
- **Size and compression** are the one place random lies: gzip has nothing to grip, so it reports the *opposite* of
  real behaviour. **Size and crossover use real data only.**

The contrast itself (synthetic inflates, real saves 45–68%) is a keep-it finding, not a bug.

### D10 — ViPaq test kernel owns its file plumbing; no shared TestFiles (CONFIRMED 2026-07-09)
An earlier session extracted the embedded-file plumbing into a shared `shared/test/Binacle.TestFiles` so both the
shared kernel and the ViPaq kernel could use it. **Reverted.** The only genuinely shared part is ~15 lines of
"enumerate manifest resources by prefix"; the *parse* differs — ViPaq's name is `<family>.<name>.<algorithm>`, the
shared kernel's is `<folder>.<name>` — so sharing needed a generic factory seam plus loosened visibility, for
little gain. Worse, a shared copy is silently broken: `Assembly.GetExecutingAssembly()` inside a shared library
resolves to *that* library, which embeds nothing, so lookups return empty and tests quietly vanish.

The ViPaq kernel now has its own `Files/` trio (`IFile`, `EmbeddedResourceFile`, `EmbeddedResourceFileProvider`),
where `GetExecutingAssembly` correctly resolves to the assembly that embeds the data. This matches the standalone
principle already recorded for the reader (memory `vipaq-generator-standalone`). **Revisit sharing only if a third
consumer appears** — and even then, share the enumeration, not the parse.

## Open — decide with data

### O2 — Codec + level (deferred; Session 4 owns it)
Build the harness codec-agnostic. Findings: gzip-Optimal ≈ brotli-Optimal on size, both fast; **never q11 as
default** (~98 ms encode — archival opt-in only). The real-data harness could not touch this — it has no codec
knob by design (D4/D5). Run the one-off codec-tradeoff experiment, then lock.

## Ruled out — do not rebuild
24-bit ladder (8/16/24/32) + coords-ride-bin · Brotli q11 as default · byte-plane/transpose layout · raw Deflate
as a third codec · selling "20% smaller". See [findings.md](findings.md) for the numbers behind each.
