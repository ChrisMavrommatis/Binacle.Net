---
description: Persistent ViPaq session prompt. Read first, work, then update it before you finish.
---

# ViPaq — session prompt

Read this after `CLAUDE.md` and `.agents/README.md`, not instead of them. Those two carry the repo-wide rules —
where guidance lives, how to write, and the one hard guardrail (never commit). This file only adds what is
specific to ViPaq. **Update it before you finish** (last section).

## Mission

Rebuild ViPaq to the new wire format: C# first, then the TypeScript mirror, then regenerate the test vectors.

## What is true

**`vipaq/PROTOCOL.md` is the format.** It is the only authority on what the bytes mean. It is language-neutral on
purpose — anyone can implement it. If the code disagrees with it, the code is wrong.

**These four files are true and binding.** The first three are reference in `.agents/docs/vipaq/` (this folder is
work-to-be-done only); the last tracks outstanding work here:

- [architecture.md](../../docs/vipaq/architecture.md) — the policy/mechanism split. The rebuild keeps it.
- [decisions.md](../../docs/vipaq/decisions.md) — D1–D16, settled with reasons. Do not re-open one without new measurements.
- [findings.md](../../docs/vipaq/findings.md) — real numbers. It measured the old implementation, so read it for magnitudes, not facts.
- [testskernel-restructure.md](testskernel-restructure.md) — test-kernel work still outstanding.

**Some `.agents/docs/vipaq/` files still describe the old wire.** `README.md`, `typescript.md` and
`cross-language-testing.md` are stale on the *format* and need rewriting when you get to it; their account of the
**public surface and file layout is current, and you must keep to it** — see the next section. The
`architecture.md`, `decisions.md` and `findings.md` in that folder are current (they moved here from the plan).

## What must not change

The format is being rebuilt. The project's shape is not. Breaking one of these is a bug, not a design choice.

- **The public surface is `ViPaqSerializer`** — `Serialize` / `Deserialize`, plus the typed wrappers
  (`SerializeInt32`, `DeserializeInt32`, `SerializeUInt16`, `DeserializeUInt16`) once they come back,
  `Dimensions<T>` / `Item<T>`, and `Limits`. Callers in `api/` and the benchmark harness depend on it
  (`decisions.md` D4). Nothing else in the library is public.
- **The chooser is public; the layer that obeys a supplied header is `internal`.** That is `architecture.md`.
  `Serialize` picks widths, layout and compression, then calls the blind layer. Do not ship only the blind layer.
- **File layout is `Models/`, `Helpers/`, `Layouts/` at the project root.** A new folder needs a reason that
  survives being said out loud. `ExtensionMethods/` folds into the reader and writer — see `architecture.md`.
- **No new abstraction for a local problem.** No interface, no factory, no project. Copy instead. Share only when a
  third consumer actually appears. The layout codecs (`ILayoutEncoder` + `ILayoutDecoder`, two implementations,
  one factory) are the one sanctioned exception, because `architecture.md` calls for them. Everything else needs
  asking first.

## How to work

**Say your plan in the response, then build.** Do not create a plan document — a plans/ file only earns its place
if it tracks work still to be done. Two or three sentences before you touch code is not a plan document. It is
thinking, and it is required.

**When the code forces a decision, make it there,** with the code in front of you. If the decision changes the
wire, write it into `PROTOCOL.md` **in the same change**. A decision that lives only in a code comment is lost.

**Nothing ships without a measured gain** — smaller base64, faster, less memory, or a concrete simplicity win.
Default to no.

**Do only what you set out to do.** If a fix pulls you into a restructure, stop and write it down instead.

**Never commit, stage, or push.** The human commits. Leave changes in the working tree.

## Done means

Not "the code is written." All four, every time:

1. `dotnet build` passes on `vipaq/src/Binacle.ViPaq`. **Five downstream projects are knowingly red** until each
   is migrated (see "Where you are now") — that is the only accepted red. Do not add a sixth, and do not leave
   the library itself broken.
2. The ViPaq tests pass — `./config/tests.sh vipaq`. They cannot until `.UnitTests` is migrated. When they can,
   they must.
3. Anything you settled that changes the wire is in `PROTOCOL.md`, not only in a comment.
4. Nothing you added is unreachable. If no code calls it, delete it.

## Where you are now

**Current state (2026-07-13): the rebuild is done end to end and both suites are green — C# 370/370, TS 328/328.**
The C# library, the TypeScript mirror, the `UnitTests` migration, the TS test suite, and the test vectors have all
landed. **Compression is now built and cross-language** (decision **D16**): one codec, **raw DEFLATE**, with
`NoOp`/`Deflate`/`Gzip` in both languages and `ProtocolEncoder` taking a **required** codec. The interop matrix
(`interop/{cs,ts}/{raw,deflate,gzip}.json` — foldered by language) proves each language decodes the other's raw,
deflate and gzip (decode-to-input; compressed bytes are never compared). **`ViPaqSerializer` still writes raw and refuses to read
compressed** — turning the toggle *on* in the serializer ("baking in") is the one deferred piece. The vector
generator owns only `header-bytes.json` and the interop artifacts (decision **D15**); every other vector is
hand-authored on purpose. Remaining work: consumer follow-ups in [migration-api-followups.md](migration-api-followups.md),
test-kernel work in [testskernel-restructure.md](testskernel-restructure.md), and `.agents/docs/vipaq/*` +
`test-vectors/README.md` still describe the old wire and need rewriting.

The rest of this section is the historical detail from when phase 1 first landed — read it for how the pieces fit,
not for current status.

**Phase 1 (2026-07-10): landed, and the old format is gone.** In `vipaq/src/Binacle.ViPaq/`:
`Header` / `Version` / `Width` / `Layout`, the `Layouts/` codecs, `Compression/` with `ICompressionCodec` +
`DeflateCodec` + `GzipCodec`, the header/width/validation helpers, and two layers:

- **`ProtocolEncoder`** — `Encode` and `Decode` on one class, because they are one agreement read in two
  directions. Handed a header, it obeys it: widths, layout, and whether to compress. **The codec is a constructor
  argument**, so a `NoOpCodec` makes the compressed path testable with the body still readable.
- **`ViPaqSerializer`** — the chooser, and the only public entry point. A `public static class`. It works the
  header out: narrowest widths per section, `RowMajor` always (layout is unmeasured), and `Compressed` always
  false (see below). `Deserialize` splits off only the two header bytes and hands the rest down — the item count
  lives *inside* the compressed body, so only the encoder can read it.
- **`HeaderNotation`** — written (2026-07-11). The header's text form for the test vectors. Grammar:
  `v{N}_{raw|comp}_{row|col}_{binW}_{itemDimW}_{itemCoordW}`, six tokens in wire order (`v1_comp_col_16_8_16`).
  Still needs a round-trip test — write it when `UnitTests` compiles.

`ProtocolWriter<T>` / `ProtocolReader<T>` are down to `WriteValue` / `ReadValue` (which dispatch to
`Write8Bits` / `Write16Bits`) and the uint16 item count. They move one value at one width; the callers group
them into triples, because the order of an item's three dimensions *is* the layout.

Verified against `PROTOCOL.md` §10: all three worked examples byte-for-byte, the forced
`(codec × compressed × layout × width)` matrix round-tripping to input, empty-items, and every §8 rejection.

**The whole old format is deleted** — no shim. `Binacle.ViPaq` builds clean, and so do the four projects migrated
so far: `PackedDataGenerator`, `Binacle.Net`, `Binacle.Net.UIModule` and `TestsKernel` (with `PerformanceTests`
and `Benchmarks` behind it). Two are knowingly red — `Binacle.ViPaq.UnitTests` and `.VectorGenerators` — and
**both wait on a decision, not on code.** This is deliberate (D11: breaking rebuild, no migration). See
`migration.md`.

**D4 was amended.** `TestsKernel` now has an `InternalsVisibleTo` grant so it reads the wire through the
library's `Header` instead of re-parsing the bytes. Read the amendment in `decisions.md` before touching the
harness.

**`Serialize` takes an `IReadOnlyList` of read-only items.** Encoding only ever reads an item, so a caller can
encode a type it cannot mutate. `Deserialize` still needs the settable interfaces and `new()`.

**Nothing the library writes is compressed.** §6 names no codec, so `ViPaqSerializer` hands the encoder a
`NoOpCodec`, never sets `Compressed`, and **refuses to read a compressed blob** rather than garble it. Do not
pick a codec by default — that is a decision made by accident.

D7's "encode both ways, keep the shorter" costs **one** compression per call, not two, provided the body is built
once and only the compress step runs on it. `ProtocolEncoder.Encode` needs splitting into a body half and a
compress-and-frame half to get that; it was written and reverted, because with compression off nothing called it.

**The typed wrappers are gone for good.** `SerializeInt32` / `DeserializeInt32` / the UInt16 pair. The API and
UIModule migrations named the generic arguments directly, and nothing else wants them.

**The codec race is built and has run once (2026-07-11).** Both halves are in:

- Size — `results/vipaq/`: `VipaqProtobufSizeComparison.{NoOp,Deflate,Gzip}.md` and
  `CodecCompressionCrossover.{Row,Columnar}.md`.
- Time — the `Curated{Encode,Decode}Benchmarks` artifacts, NoOp path only.

The standing read is **deflate + columnar.** Deflate beats gzip on size at every real pack; columnar wins on size
once a codec runs — contradicting `findings.md`'s bet — and costs only ~10% encode / ~3–5% decode. ViPaq stays
smaller than protobuf under matched codecs. **The decision is deliberately not locked:** more scenarios and codecs
are coming, so re-run before pinning. See [codec-race.md](codec-race.md) for the read in full.

**One cell is still empty: compression time.** The benchmarks measured the NoOp path only, so deflate's own
encode/decode cost — and D7's try-both price — is unmeasured. It cannot flip the pick (gzip is deflate plus a
header and is already bigger), but §6 should carry a real number, so run it before the final write.

**When you lock:** name the winner in §6, strike it from §12, record the numbers in `findings.md`, lock the codec
**and** the layout in `decisions.md`, point `ViPaqSerializer` at the codec, drop `Deserialize`'s
`NotSupportedException`, then un-invert `CuratedScenarioCheck` and `CompressionCrossoverTest`.

**All three codecs stay.** The wire pins one — `Version` fixes it, and there is no codec field in the header.
`ICompressionCodec` is not scaffolding: the harness measures both codecs forever, and `NoOpCodec` is what makes
the compressed path testable. Pinning the codec changes one line in `ViPaqSerializer`. Do not collapse anything.

**`UnitTests` and `VectorGenerators` are done** (this is the stale part of the old handover): `UnitTests` is
migrated and green (345/345), `HeaderNotation` has its round-trip test, and `VectorGenerators` regenerates the
uncompressed vectors. The compressed vectors still wait on the codec pin.

The library now has a **real test suite** — the throwaway-console-app era is over. The §10 worked examples,
the forced `(codec × compressed × layout × width)` matrix, round-trips, per-section width choice, empty items,
and every §8 rejection are all covered by `UnitTests` (and mirrored in TS).

**The first decision the code will force:** `PROTOCOL.md` §6 does not name a compression codec, and `Version` pins
it. The spec is not final until you pick one, and the TypeScript mirror cannot start without it. Pick it, write it
into §6, remove it from §12 — in the same change. `decisions.md` O2 holds the constraints: the codec must exist in
C# and in the browser; gzip-Optimal and brotli-Optimal tie on size at usable speed; never brotli q11 as a default.

Open work, verified and not in the spec, is listed in [README.md](README.md).

## Before you finish — update this file

This prompt is the handover. A session that leaves it stale has failed the next one.

- Rewrite **"Where you are now"**: what you did, what is next, what decision is now blocking.
- If you settled something that changes the wire, it belongs in `PROTOCOL.md`, not here.
- Delete any plans/ file whose work has landed — a file earns its place only while work remains.
- Keep this file short. It is a prompt, not a log. Nothing here should need to be read twice.
