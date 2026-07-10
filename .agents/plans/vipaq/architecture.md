---
description: ViPaq architecture — the blind encode/decode layer, the layout codecs, and the serializer that chooses. Phase 1 is the base structure.
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

There is one type, `ViPaqHeader`, and it is both the thing the encoder is told and the thing that lands on the
wire. It carries the six fields of `PROTOCOL.md` §2: `Version`, `Compressed`, `Layout`, and the three `Width`
values (bin dimensions, item dimensions, item coordinates).

No separate directive type. The spec already forces the header to describe the blob completely — a decoder reads
it and never re-derives anything (§4) — so a second type would carry the same fields under a different name.

**`Compressed` is the odd one.** The other five are decided by looking at the input. `Compressed` cannot be:
you only know whether compressing paid after you compress and compare lengths (§6, `decisions.md` D7). So the
blind encoder takes it as an instruction and obeys it — "compress this" or "don't" — and the *choosing* layer is
what runs both and keeps the shorter one. The blind layer never decides; it just does what the bit says.

## Phase 1 — the base structure — **landed 2026-07-10**

Only the blind layer. No choosing. In `vipaq/src/Binacle.ViPaq/`:

| Piece | What it does |
|---|---|
| `ProtocolWriter<T>` / `ProtocolReader<T>` | Read and write **one** value at a given `Width`, little-endian |
| `Layouts/ILayoutCodec` + `RowMajorCodec` + `ColumnarCodec` | Write and read the *items*, in the order `Layout` names |
| `Layouts/LayoutCodecFactory` | Hands back the codec for a `Layout` |
| `Compression/ICompressionCodec` + `DeflateCodec` + `GzipCodec` + `NoOpCodec` | Squeeze the body, and unsqueeze it |
| `ProtocolEncoder` | `Encode` and `Decode`. Handed a header, obeys it — widths, layout, and whether to compress. |
| `ViPaqSerializer` | **Stub.** The chooser: works the header out from the bin and items, then calls the encoder. |
| `HeaderNotation` | **Stub.** The header's text form, for the test vectors. |

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

**Two codecs, on purpose, for now.** `PROTOCOL.md` §6 still does not name one, so both are built and the encoder
is handed the one to use. The next step is to race them on real packs and pick. `ICompressionCodec` dies with
that choice: the spec fixes one codec per `Version` and puts no codec field on the wire, so nothing may be built
on the idea that the codec is pluggable. Once picked, name it in §6, strike it from §12, and collapse the
interface.

**Reader and writer move one value, and nothing more.** `WriteValue(value, width)` and `ReadValue(width)` are the
whole new surface. They do not know what a dimension or a coordinate is, and there is no `WriteDimensions` or
`ReadCoordinates` — grouping three values into a triple is the caller's business. The layout codecs do it for the
items, because the order of those three *is* the layout. `ProtocolEncoder` and `ProtocolDecoder` do it for the
bin, because the bin is written the same way in both layouts (§3).

The new path does not range-check on the way in: an 8- or 16-bit field cannot hold an out-of-range value, which
is what §5 means by "a decoder has nothing to range-check". The old `EnsureWithinRange` guard exists only for the
64-bit tier that §4 deleted.

**The old-format methods are gone.** `Write32Bits`, `Write64Bits`, `Read32Bits`, `Read64Bits`, the `BitSize`
extension methods, `ExtensionMethods/`, and `ViPaqLimits.MaxInteger` / `.SixteenBitsMax` / `.ThirtyTwoBitsMax` /
`.CompressionThresholdBytes` were all deleted with the shim. `ViPaqLimits` keeps only `EightBitsMax`, `MaxValue`
and `MaxItemCount`.

**The layout codecs are a sanctioned abstraction.** One interface, two implementations, one factory. The pair
that writes the items and the pair that reads them have to agree, so they live behind one type. Only the items
are laid out — the item count and the bin dimensions are the same in both layouts (§3), so the encoder and
decoder handle those and hand over at the items.

**Encoder and decoder obey the header.** They validate that the header can hold the data (§8) and then write or
read exactly what it declares. No width is re-derived on decode (§4). No compression decision is made here.

## The chooser — `ViPaqSerializer` — **STUB, not written**

`Serialize` and `Deserialize` throw. The type exists to hold the shape and the open questions; the file lists
them. `ProtocolEncoder` is blind, so *something* has to decide the header, and this is where that goes:

- **Widths** — the narrowest that holds each section, sized independently (§4). A big bin can hold small items
  at large coordinates, so the three sections genuinely disagree (`findings.md`: Bischoff packs to `16/8/16`).
- **Layout** — unmeasured. `findings.md` suggests a good codec already exploits the structure columnar exposes,
  so it may buy nothing. Both ship and the header bit records which was used, so it can change without a version
  bump.
- **Compressed** — encode both ways, keep the shorter blob (§6, D7). Never inflates, no threshold to tune wrong.

`Deserialize` is the easy half: split off the two header bytes, hand the encoder the header plus the rest.

That layer, and only that layer, is what the public API and the permanent benchmark harness call, so the harness
must not churn when the internals move (`decisions.md` D4).

**Open.** The codec must be pinned before this can be `public` with a parameterless constructor. And the shape of
this type is itself unsettled — a working chooser was written and pulled back out, because whether the choosing
belongs here, on `Header`, or somewhere else is not decided.

> **Sequencing — settled: no shim.** The old `ViPaqSerializer`, `EncodingInfo`, `BitSize`, `EncodingInfoHelper`,
> `BitSizeHelper`, `EncodingInfoNotation` and the `ExtensionMethods/` folder are **deleted**. `Binacle.ViPaq`
> itself builds clean, but five projects that used the old API do not, and stay red until phase 2 lands
> `ViPaqSerializer` on the new wire: `Binacle.ViPaq.UnitTests`, `Binacle.ViPaq.TestsKernel`,
> `Binacle.ViPaq.VectorGenerators`, `Binacle.ViPaq.PackedDataGenerator`, and `Binacle.Net.UIModule`.
>
> This is a deliberate, temporary red. It is the cost of not carrying two formats at once (`decisions.md` D11:
> breaking rebuild, no compatibility, no migration). Phase 2 closes it.

## Public surface, and what tests can reach

- **Public:** `ViPaqSerializer` (phase 2), `Bin<T>` / `Item<T>`, `ViPaqLimits`. The constraint interfaces come
  from the `Binacle.Geometry` leaf.
- **Internal:** everything in phase 1 — reader, writer, layout codecs, encoder, decoder, and `ViPaqHeader`.
- `Binacle.ViPaq.csproj` grants `InternalsVisibleTo` to **only** `.UnitTests` and `.VectorGenerators` — not
  `.Benchmarks`, `.PerformanceTests`, or `.TestsKernel`. That is deliberate: the permanent harness lives on the
  public API (D4). Granting more would weaken it.
- **Racing the two codecs needs internals**, and the benchmark projects cannot reach them today. Per D5 that
  race is a *one-off experiment*, not the permanent ruler — so run it as a throwaway rather than adding an
  `InternalsVisibleTo` entry for `.Benchmarks`. Record the answer in `findings.md` and lock it in `decisions.md`.

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

- **Does columnar actually pay?** Unmeasured. `findings.md` suggests a good codec already exploits the structure
  columnar exposes. Treat it as unproven until raced on real packs.
- **The codec for `Version = 0`.** Still unnamed in `PROTOCOL.md` §6 and §12. See `decisions.md` O2.
- **The header's text notation.** `HeaderNotation` is a **stub** — `Parse` and `Format` throw. The old grammar
  (`Uncompressed_8_8_8`) does not survive: its `Version` word conflated the version with the compression flag,
  which are now separate header fields, and it has no place for `Layout`. A new grammar must carry `Compressed`,
  `Layout`, and the three widths; whether it also names `Version`, and whether it is positional or named, is
  undecided. `PROTOCOL.md` defines no text form, but §6.1 requires a vector to state the full header its bytes
  were produced under. Settle it when the vectors are regenerated — the file lists the open questions.
