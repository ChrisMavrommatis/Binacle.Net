---
id: vipaq/architecture
description: ViPaq architecture — the blind encode/decode layer, the layout codecs, and the serializer that chooses. The policy/mechanism split the rebuild keeps.
verified: 2026-08-19
check: Policy/mechanism split matches vipaq/src/Binacle.ViPaq — ProtocolEncoder obeys the header, ViPaqSerializer chooses widths/layout/compression, Layouts/ hold the codecs; every type named here has the visibility claimed; the ViPaqSerializer call sites listed still exist and still name their types; the report files under results/vipaq/compression/ resolve
paths:
  - "vipaq/**"
---

# ViPaq — architecture

The shape of the rebuilt C# library, aligned to `vipaq/PROTOCOL.md`. This is a constraint, not a suggestion.
Where this file and the spec disagree, the spec wins.

## The core split

Two layers, and the whole design turns on keeping them apart.

- **Blind (mechanism).** Does exactly what the header says — "1 byte per number, columnar, compressed" — even
  when that is a silly choice for the data. It decides nothing.
- **Choosing (policy).** Looks at the bin and items, picks the header, then calls the blind layer.

Why: every combination becomes forceable. A test can write 2 bytes per number where 1 would do, or force
columnar where row is better, and still get the input back. That is how the "is it worth it?" questions
(row vs columnar, raw vs compressed) get measured instead of guessed — and it is what `PROTOCOL.md` §4
"Selection" means when it says a wider width is *conformant*, just larger.

## The header is the instruction

There is one type, `Header`, and it is both the thing the encoder is told and the thing that lands on the
wire. It carries the six fields of `PROTOCOL.md` §2: `Version`, `Compressed`, `Layout`, and the three `Width`
values (bin dimensions, item dimensions, item coordinates).

No separate directive type. The spec already forces the header to describe the blob completely — a decoder reads
it and never re-derives anything (§4) — so a second type would carry the same fields under a different name.

**`Compressed` is the odd one.** The three widths are decided by looking at the input. `Compressed` cannot be:
you only know whether compressing paid after you compress and compare lengths (§6). So the blind encoder takes it
as an instruction and obeys it — "compress this" or "don't" — and the decision is passed out to the caller through
`ViPaqSerializationOptions`. The blind layer never decides; it just does what the bit says.

## Phase 1 — the base structure

Only the blind layer. No choosing. In `vipaq/src/Binacle.ViPaq/`:

| Piece | What it does |
|---|---|
| `ProtocolWriter<T>` / `ProtocolReader<T>` | Read and write **one** value at a given `Width`, little-endian |
| `Layouts/ILayoutEncoder` + `ILayoutDecoder`, both on `RowMajorCodec` and `ColumnarCodec` | Write and read the *items*, in the order `Layout` names |
| `Layouts/LayoutCodecFactory` | Hands back the encoding or decoding half of the codec for a `Layout` |
| `Compression/ICompressionCodec` + `DeflateCodec` + `GzipCodec` + `NoOpCodec` | Squeeze the body, and unsqueeze it |
| `ProtocolEncoder` | `Encode` and `Decode`. Handed a header, obeys it — widths, layout, and whether to compress. |
| `ViPaqSerializer` | The chooser: works the header out from the bin and items, then calls the encoder. |
| `HeaderNotation` | The header's text form, for the test vectors. Grammar: `v{N}_{raw\|comp}_{row\|col}_{binW}_{itemDimW}_{itemCoordW}` |

**One `ProtocolEncoder`, with `Encode` and `Decode` on it.** They are one agreement read in two directions:
whatever `Encode` writes, `Decode` must read back. Keeping them in one class keeps that pair in one place.

**The codec is a constructor argument on `ProtocolEncoder`.** That is what makes the blind layer fully testable:
hand it a `NoOpCodec` and every combination of widths, layout and compression becomes forceable *with the body
still readable*, so framing can be checked byte for byte — impossible through a real codec, because compressed
bytes must never be compared (§6.1). Racing DEFLATE against gzip is two encoders and nothing else. Once the race
is settled the codec is pinned by `Version` (§6) and the constructor takes the winner.

**Why the encoder, and not the serializer, owns the codec and the item count.** The count is a uint16 at the
front of the *body* (§3), and the body is what gets compressed (§1). So the count cannot be read until after the
body is inflated (§7, steps 4-5). `Deserialize` therefore splits off only the two header bytes — which are never
compressed — and hands the encoder the header plus everything after it. The encoder inflates, reads the count,
and decodes.

`Header` carries a `Version` enum. `Version1` is `0`, so it is not `required` and the default is the only version
this implementation writes; a decoder rejects codes 1-3 (§2.3).

**Three codecs, and they all stay.** `PROTOCOL.md` §6 names one — raw DEFLATE — and `ViPaqSerializer.ResolveCodec`
picks it from the header's `Compressed` bit, falling back to `NoOpCodec` when the bit is clear. Pinning it changed
that one method and **nothing else**:

- **The wire is not pluggable.** The spec fixes one codec per `Version` and puts no codec field in the header, so
  a shipped blob is inflatable by exactly one thing. Never build on the idea that a reader can choose.
- **`ICompressionCodec` is.** It is what makes `ProtocolEncoder` testable — a `NoOpCodec` forces the compressed
  path with the body still readable, which §6.1 forbids through a real codec. And the permanent harness measures
  deflate against gzip on every run, mirroring both onto protobuf, so both implementations must survive the choice.

Do not delete the losing codec. Do not collapse the interface.

**Reader and writer move one value, and nothing more.** `WriteValue(value, width)` and `ReadValue(width)` are the
whole new surface. They do not know what a dimension or a coordinate is, and there is no `WriteDimensions` or
`ReadCoordinates` — grouping three values into a triple is the caller's business. The layout codecs do it for the
items, because the order of those three *is* the layout. `ProtocolEncoder` does it for the bin, because the bin
is written the same way in both layouts (§3).

The new path does not range-check on the way in: an 8- or 16-bit field cannot hold an out-of-range value, which
is what §5 means by "a decoder has nothing to range-check". The old `EnsureWithinRange` guard exists only for the
64-bit tier that §4 deleted.

**The old-format methods are gone.** `Write32Bits`, `Write64Bits`, `Read32Bits`, `Read64Bits`, the `BitSize`
extension methods, `ExtensionMethods/`, and `ViPaqLimits.MaxInteger` / `.SixteenBitsMax` / `.ThirtyTwoBitsMax` /
`.CompressionThresholdBytes` were all deleted with the shim. The class is now `Limits`, and it keeps only
`EightBitsMax`, `MaxValue` and `MaxItemCount`.

**The layout codecs are a sanctioned abstraction.** Two implementations, one factory, and **two interfaces**:
`ILayoutEncoder` and `ILayoutDecoder`. Both are implemented by the same class, because the thing that knows how
to write the item fields is the thing that knows how to read them back. The split is not about independence —
it is that `TItem` means different things in the two directions. Encoding only ever *reads* an item, so it asks
for `IWithReadOnlyDimensions` / `IWithReadOnlyCoordinates` and a caller can encode a type it cannot mutate.
Decoding fills items in place, so it needs the settable pair. `TItem` is a method type parameter for that reason.

Only the items are laid out — the item count and the bin dimensions are the same in both layouts (§3), so
`ProtocolEncoder` handles those and hands over at the items.

**Encoder and decoder obey the header.** They validate that the header can hold the data (§8) and then write or
read exactly what it declares. No width is re-derived on decode (§4). No compression decision is made here.

## The chooser — `ViPaqSerializer`

A `public static class`. `ProtocolEncoder` is blind, so *something* has to decide the header, and this is where
that goes. `Header.Create` sizes the three widths from the input; `Serialize` layers the caller's options on top
with a `with` expression:

- **Widths** — the narrowest that holds each section, sized independently (§4). A big bin can hold small items
  at large coordinates, so the three sections genuinely disagree (Bischoff packs to `16/8/16`).
  With no items both item widths stay `Eight`, which is what §4 requires.
- **Layout** — the caller's choice through `ViPaqSerializationOptions`, default `RowMajor`. Both codecs ship and
  the header bit records which was used, so the default can change without a version bump.
- **Compressed** — the caller's choice too, default off. Not decided by the library: encoding both ways and
  keeping the shorter blob costs a second compression on every call, and that cost is unmeasured, so the call
  is handed to whoever knows their own trade-off.

`Deserialize` is the easy half: `Header.FromBytes` on the first two bytes — which already rejects a bad version,
a set reserved bit and a reserved width code (§7, steps 2-3) — then hand the encoder the header plus the rest.

That layer, and only that layer, is what the public API and the permanent benchmark harness call, so the harness
must not churn when the internals move.

**One codec rule, read the same way in both directions.** `ResolveCodec(header)` is the only place that maps the
`Compressed` bit onto a codec — raw DEFLATE when set, `NoOpCodec` when clear — and both `Serialize` and
`Deserialize` call it. A compressed blob reads back fine; there is no unsupported path.

**There are no typed wrappers.** `Serialize<TBin, TItem, T>` and `Deserialize<TBin, TItem, T>` are the whole public
surface. The old `SerializeInt32` / `DeserializeInt32` / `SerializeUInt16` / `DeserializeUInt16` are gone and did
not come back — every call site names its types on the generic method instead (`v3/Contracts/PackResponse.cs`,
`v4/Contracts/BinResponseBase.cs`, both `ViPaqExampleExtensions`, and the UIModule decoder).

## Public surface, and what tests can reach

- **Public:** the surface table in `$vipaq` is the list. Architecturally what matters is that it is the
  *choosing* layer plus the knobs that feed it — `ViPaqSerializer`, `ViPaqSerializationOptions`, `Layout` —
  and nothing that describes the wire.
- **Internal:** everything the blind layer is made of — `ProtocolReader<T>` / `ProtocolWriter<T>`, the layout
  codecs and their factory, `ProtocolEncoder`, `Header`, `Version`, `Width`, `HeaderNotation`, and the three
  codec implementations. **`ICompressionCodec` itself is public** while `DeflateCodec` / `GzipCodec` /
  `NoOpCodec` are not: the seam is visible, the choice of stream is not.
- `Binacle.ViPaq.csproj` grants `InternalsVisibleTo` to `.UnitTests`, `.VectorGenerators`, `.TestsKernel`,
  `.PerformanceTests` and `.Benchmarks` — the measurement harnesses drive the blind layer directly, which needs
  internals. `.PackedDataGenerator` is deliberately not on that list (`$vipaq/dependencies`, wall 3).
- **Racing the codecs needs internals**, and `TestsKernel` has them. The race is part of the permanent harness,
  so it belongs there rather than in a throwaway. No new grant is needed. The reports are in
  `results/vipaq/compression/`.

The public contract does not grow, yet tests can force any combination.

## What this makes testable

- **Forced-combo matrix.** Every `(width × layout × compressed)` through the blind encoder: assert the header
  round-trips, decode, assert it equals the input. The oracle is **decode-to-input, not byte-equality** (§6.1).
- **Byte-exact vectors.** Only with the header pinned *and* uncompressed. Compressed bytes must never be compared
  — two compressor builds can emit different, equally valid streams (§6.1).
- **The chooser is a checkable function** (phase 2). Enumerate the combinations through the blind layer and assert
  the choosing layer picked the smallest base64.
- **A header that cannot hold its data throws.** The blind layer trusts the header but rejects the impossible —
  1 byte per number forced on a value of 300 (§8, encode side).

## Cross-language

TypeScript may choose differently from C# for the same input, and both are conformant (§6.1). So the interop
vectors need **forced-width and forced-layout rows**, not just whatever each encoder picks naturally. Otherwise
the two could silently disagree in a mode neither would choose on its own.

## Open — do not assume

- **Does columnar actually pay?** Both layouts were raced against both codecs — the reports are in
  `results/vipaq/compression/` (`CodecCompressionCrossover.Row.md`, `.Columnar.md`). `RowMajor` remains the
  default; treat columnar as available and measured, not as the better choice.
- **Whether the library should choose `Compressed` for you.** It does not. Encoding both ways and keeping the
  shorter blob is still unmeasured for encode time, so the decision stays with the caller.
