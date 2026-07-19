---
id: vipaq/decisions
description: ViPaq decisions ledger — the locked decisions and their reasons, plus the open questions.
verified: 2026-07-14
check: Locked decisions are not contradicted by vipaq/PROTOCOL.md or vipaq/src/Binacle.ViPaq
also_update:
  - vipaq/architecture
  - vipaq/findings
---

# ViPaq — decisions ledger

Locked decisions and open questions, with the *why*. Evidence lives in `$vipaq/findings` (superseded prototype
numbers in `$vipaq/history`); design detail in `$vipaq/architecture`. This file is the "what we settled and why"
so a fresh session doesn't re-litigate it.

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
Varint deferred to Session 7, may never happen. **Not a size play** — two tiers cost the same as four on ≤16-bit
data, so ~0% smaller. The payoff is a **simpler format** (2 tiers not 4; no 2⁵³ ceiling to reason about) and a
clean base if varint is ever wanted. Do not sell "20% smaller" — that was a Brotli-q11 artifact (see `$vipaq/findings`).

### D2 — 16-bit cap in v2.0; throw above 65,535
Fixed 8/16 caps at 65,535. v2.0 throws above it; varint (Session 7) lifts the cap later. mm → 65 m, fine for
physical bins. Retire/repoint `ViPaqLimits.MaxInteger` (2⁵³−1) to the new ceiling in Session 4.

### D3 — Baselining without a v1/v2 pair (CONFIRMED 2026-07-07)
ViPaq has one implementation, so there's no in-code baseline like lib's v1-vs-v2 racing. Two mechanisms replace it:
- **Protobuf is the in-run anchor** — `[Benchmark(Baseline = true)]`. ViPaq is reported as a *ratio* to protobuf,
  so a rerun on another machine/day stays comparable; the anchor absorbs environment drift.
- **Committed result files are the recorded baseline** — the size reports under `results/vipaq/compression/`. A *win* = a
  diff showing smaller base64 / lower ns / lower allocs **while the protobuf anchor is unchanged**. Small
  increments; keep only measured wins.
- **The perf test writes to build-local scratch, not to the committed vault** (2026-07-14). It emits into its
  `PerformanceTests.Artifacts/` folder (gitignored); to check for a win, diff that against
  `results/vipaq/compression/`, and copy the report in by hand only when it's a keeper. The committed baseline is
  hand-curated, never auto-overwritten — same model as `results/lib/`.
- Results stay in the repo under `results/`, organized by slice — see [results/README.md](../../../results/README.md)
  for the layout and the scratch-vs-curated convention. (Settled 2026-07-14; the old results migration is closed.)

### D4 — The permanent harness uses only the minimal public API (CONFIRMED 2026-07-07)
The permanent benchmark **encodes and decodes** through `ViPaqSerializer.Serialize`/`Deserialize` only — that is
what makes the harness layout-agnostic. It reads the header through the library's internal `Header`, not by
re-parsing bytes: `Binacle.ViPaq` grants `InternalsVisibleTo` to `Binacle.ViPaq.TestsKernel`, and `ViPaqHeader`
wraps `Header` behind a private field, exposing only `bool`/`int`/`string`. **One copy of the spec beats a clean
boundary here** — `Header` is a frozen wire description, not an evolving API, so if it churns the format churned and
the harness *should* break. (This reading-via-internals rule replaced an earlier re-parse-the-bytes rule; the
superseded version is in `$vipaq/history`.)

Two consequences of the public-API rule, both still true:
- **Layout-agnostic for free** — when v2 swaps row→columnar internally, the bytes change but the harness call
  sites don't. This is automatic from living at the public boundary, not something the harness engineers.
- **It measures real behavior only** — the public `Serialize` chooses compression itself (D7), so the harness
  measures what callers get, not a tuned mode. It *detects* whether ViPaq compressed by reading the header's
  `Compressed` bit (byte 0, bit 5 — **not** the `Version` field; the spec separated them), then mirrors that on
  protobuf for a fair comparison.
- **Phase 1 adds a compression override** (D13) that the harness may use to measure raw vs compressed. That is a
  measurement entry point, not a caller knob, and it does not change what the public default does.
- **Why minimal:** a permanent ruler must not churn when the lib evolves. Coupling it to an evolving API would
  defeat the point. The codec race is part of the ruler, not a separate experiment (D5).

### D5 — The codec race lives in the harness, permanently (CONFIRMED 2026-07-07)
- **Permanent harness**: measures real-mode size + CPU/mem + protobuf ratio, and *observes* the shipped
  compression crossover by sweeping item count.
- **The codec race is part of it, permanently.** The harness encodes every scenario in each mode — `Raw`, `NoOp`,
  and deflate/gzip across both layouts — and mirrors each codec onto protobuf. The reports are in
  `results/vipaq/compression/`.

Why it belongs in the permanent ruler, not a throwaway — **the race is not only about the codec:**
- It also settles **row-major vs columnar**, which is unmeasured and is a permanent harness concern.
- It fixes a real bug. The harness would otherwise compare a **compressed** ViPaq token against **raw** protobuf,
  so the gap it prints is mostly the compression, not the format. Mirroring each codec onto protobuf is the fix —
  the ruler being honest, not an experiment.
- "Is deflate still the right pick on this data?" is worth re-asking as the data changes, not answering once.

(This reverses an earlier decision that scoped the race as a one-off experiment; the superseded version is in
`$vipaq/history`.)

Consequences: `ICompressionCodec`, `DeflateCodec`, `GzipCodec` and `NoOpCodec` are **permanent**. Nothing
collapses when the codec is pinned. The wire still pins exactly one codec — `Version` fixes it and there is no
codec field in the header — so pinning changes one line in `ViPaqSerializer` and nothing else.

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
`$vipaq/findings`.

The spec states this as a **SHOULD**, not a MUST: the `Compressed` bit is normative, the choosing policy is not.
That is what lets phase 1 force compression on or off to measure it — see D13.

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
principle already recorded for the reader. **Revisit sharing only if a third
consumer appears** — and even then, share the enumeration, not the parse.

### D11 — Breaking rebuild; the old format is ignored (CONFIRMED 2026-07-09)
No compatibility, no migration, no fallback. No decoder reads the old wire and no code path detects it; stored
tokens must be re-encoded. Nothing in the repo says the old format existed — the break is announced in the
release notes only.

What settled it: reading old blobs means keeping four integer widths, a 64-bit tier, and the whole `2^53 − 1`
range apparatus alive in every decoder, in both languages, forever. That apparatus is the biggest thing we delete.

### D12 — Two-byte header, split by purpose (CONFIRMED 2026-07-09)
`Version`(2) + `Compressed`(1) + `Layout`(1) + three 2-bit widths is 10 bits. Byte 0 is **how to read** the body,
byte 1 is **how wide** its integers are. The second byte is nearly free — base64 encodes 3 bytes to 4 characters.
Widths keep 2 bits, so each section has two spare codes: one for varint, one in hand. `vipaq/PROTOCOL.md` §2.

### D13 — `Compressed` and `Layout` are per-blob flags, not versions (CONFIRMED 2026-07-09)
Both describe what the encoder did to *this* blob, so one decoder reads all four combinations. That makes them
measurable — row/columnar × raw/compressed, raced on real packs instead of guessed at spec time. It is also the
phase-1 switch. It does **not** re-open the threshold question (D7): try-both stays the default, and the spec
makes the *bit* normative while the *policy* is not.

### D14 — Widths are policy too; only the header is normative (CONFIRMED 2026-07-09)
Found by reviewing the spec against these plans. Widths, `Layout` and `Compressed` are all the encoder's choice,
all recorded in the header, and a decoder obeys the header rather than re-deriving anything. Two consequences:

- **Every combination is forceable** and still conformant — force 16-bit on sub-255 data, force columnar, force
  raw. That is what the forced-combo matrix needs (`vipaq/PROTOCOL.md` §4 "Selection").
- **"Uncompressed bytes are byte-identical across languages" only holds with the header pinned** (§6.1). Two
  conformant encoders may choose differently for the same input and both are right. Golden vectors must state the
  header they expect bytes under. The old blanket claim was wrong; sessions 5 and 6 are corrected.

### D15 — Generators are for combinatorial and derived vectors only (CONFIRMED 2026-07-13)
The vector generator (`Binacle.ViPaq.VectorGenerators`) earns its keep on two files: `header-bytes.json` (32
combinatorial rows, tedious and error-prone by hand) and the interop artifact (`artifact-cs.json` — the actual
bytes C#'s encoder emits, which TS must match, so it *has* to be derived). Everything else stays **hand-authored**
JSON: `exact-bytes.json`, `little-endian/*.json`, `width-selection.json`, `width-invalid.json`,
`decode-invalid.json`, `encode-invalid.json`, `round-trip-scenarios.json`.

What settled it: those are small, curated sets. Writing a C# scenario record plus a bespoke formatter to emit JSON
a human writes directly is more machinery than the payoff. The cross-language value comes from *both suites reading
one shared file* — not from generating that file. And for the oracle/spec files (width-*, invalid, round-trip),
recomputing the oracle from the library under test would make the tests tautological — the library grading its own
homework. So task #9 ("generate ALL vectors") is closed as effectively done: the two files that justify a generator
are generated; the rest stay hand-authored.

### D16 — One codec (raw DEFLATE); compression is a user toggle, not a pinned policy (CONFIRMED 2026-07-13)
Resolves O2. The wire has a `Compressed` bit but **no codec field**, so multiple codecs was never really on the
table — a decoder could not tell them apart. So there is exactly **one** compression codec, and it is **raw
DEFLATE** (RFC 1951, no wrapper). Gzip is the same DEFLATE stream plus ~18 bytes of framing that buys nothing
here — measured on a small pack, gzip `56` vs deflate `32` vs raw `48`, so gzip can be *bigger* than raw — so gzip
stays only for the race. Deflate is portable and proven end to end: C# `DeflateStream` ↔ browser
`CompressionStream('deflate-raw')` ↔ Node `zlib.deflateRaw`. It **must** be the `-raw` variant; plain `deflate`
adds a zlib header C# does not write. Compressed bytes are not byte-identical across engines, so the guarantee is
decode-to-input — the interop matrix (`interop/{cs,ts}/{raw,deflate,gzip}.json`) proves each language decodes the
other's deflate and gzip.

**It is not a ship blocker.** Layout and compression are the encoder's choice, recorded in the header, exposed as
options with defaults **RowMajor** and **uncompressed**. The default is off, so v2 can ship with the toggle
present and unused. **Baked in 2026-07-14:** `ViPaqSerializer.Serialize` now takes a `ViPaqSerializationOptions`
(C#: `Action<ViPaqSerializationOptions>`; TS: an optional options object) with `Compress` and `Layout`, both
defaulting off / RowMajor. They set the header's `Compressed` and `Layout` bits; a single `ResolveCodec(header)`
maps the bit to the codec (raw DEFLATE when set, a pass-through `NoOpCodec` when not), and the encoder just runs
that codec — the same three lines for encode and decode. `Compress` is a straight on/off: it does not check
whether compression paid, so a small pack can come out larger, which §6 allows (D7's try-both is **not** wired —
it stays available in the harness for measurement, not in the serializer). `Deserialize` reads compressed blobs
again; the old refusal is gone. `ProtocolEncoder` takes the codec as a **required** argument in both languages;
`NoOpCodec` keeps the compressed path testable with the body readable.

## Open — decide with data

### O2 — Codec + level (RESOLVED 2026-07-13 → D16)
Resolved by **D16**: one codec, raw DEFLATE, exposed as a user toggle (default off). Compression *level* never
reaches the wire, so it stays a free encoder-side choice. The old worry — "name the codec before v2 ships" — is
moot now the default is uncompressed; nothing blocks shipping.

## Ruled out — do not rebuild
24-bit ladder (8/16/24/32) + coords-ride-bin · Brotli q11 as default · byte-plane/transpose layout · raw Deflate
as a third codec · selling "20% smaller". See `$vipaq/findings` for the numbers behind each.
