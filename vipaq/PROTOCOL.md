# ViPaq Protocol Specification

> **Status: experimental.** ViPaq may change. This document defines the wire format, independent of any
> language. It is the authority on what the bytes mean. The C# library (`src/Binacle.ViPaq`) is the reference
> implementation — it produces the golden test bytes — but it does not outrank this spec. Where C# differs from
> this document (see the decisions log), C# has the bug to fix, not the spec.

ViPaq is a compact binary format for one packing result: a single **bin** (dimensions) plus a list of **items**
(dimensions and position coordinates). There are two implementations of this one format — the canonical C#
library (`src/Binacle.ViPaq`) and a hand-maintained TypeScript mirror (`binacle-vipaq`). This spec is what keeps
them on the same wire. See [README.md](README.md) for the index.

## Notation and conformance

The words **MUST**, **MUST NOT**, **SHOULD**, and **MAY** are used as in RFC 2119.

- A **byte** is 8 bits. Bit 7 is the most significant, bit 0 the least.
- All multi-byte integers are **unsigned** and **little-endian** (least significant byte first).
- Hex bytes are written `0x0A`. Header bit fields are written `0bAA_BB_CC_DD` (one 2-bit group per field).
- An **encoder** produces a blob from a bin + items. A **decoder** reads a blob back into a bin + items.

## 1. Top-level structure

A ViPaq blob is one header byte followed by a body:

```
[ EncodingInfo : 1 byte ][ Body ]
```

- The header byte is **never compressed**. The body is built first and compressed if it is large enough (§6);
  the header byte is added in front afterwards.
- So every blob is **self-describing**: a decoder reads byte 0 first, then reads or decompresses the body
  according to it. This is what makes the two implementations interoperate.
- Base64 is **not** part of the format. Encoders return raw bytes; callers (the API) may base64 the result.

## 2. EncodingInfo byte (header)

Four 2-bit fields, most significant first:

| Bits | Field | Values |
|---|---|---|
| 7-6 | `Version` | `0` Uncompressed, `1` CompressedGzip, `2` Reserved2, `3` Reserved3 |
| 5-4 | `BinDimensionsBitSize` | `BitSize` (see §4) |
| 3-2 | `ItemDimensionsBitSize` | `BitSize` (see §4) |
| 1-0 | `ItemCoordinatesBitSize` | `BitSize` (see §4) |

The byte is composed as:

```
byte = (Version << 6) | (BinDimensionsBitSize << 4) | (ItemDimensionsBitSize << 2) | ItemCoordinatesBitSize
```

The field at bits 7-6 is named `Version` in both implementations. Today it carries only the **compression**
state: `0` = body stored raw, `1` = body is a gzip stream. There is no separate format-version field. Values
`2` and `3` are reserved. A decoder **MUST** reject a blob whose `Version` is `Reserved2` or `Reserved3`
(see §7 and §8).

## 3. Body layout

The body, before any compression, is written in this exact order. Each integer's width is the `BitSize` named
for its section in the header (§2), encoded little-endian (§4).

| # | Field | Width | Notes |
|---|---|---|---|
| 1 | Item count | `uint16` (always 2 bytes) | `(ushort)items.Count`; **MUST** be ≤ 65535 |
| 2 | Bin dimensions | `BinDimensionsBitSize` | Length, Width, Height — in that order |
| 3 | Per item (× count) | `ItemDimensionsBitSize`, then `ItemCoordinatesBitSize` | dims L, W, H, then coords X, Y, Z |

The item count is **always** a 2-byte `uint16`, independent of the `BitSize` fields. The `BitSize` fields apply
only to bin dimensions, item dimensions, and item coordinates.

Items appear in the order the encoder received them. A decoder **MUST** return them in that same order.

## 4. Integer widths (`BitSize`)

Each dimension/coordinate section is stored at one of four byte-aligned widths:

| `BitSize` | Value | Bytes per integer |
|---|---|---|
| `Eight` | `0` | 1 |
| `Sixteen` | `1` | 2 |
| `ThirtyTwo` | `2` | 4 |
| `SixtyFour` | `3` | 8 |

**Width selection.** For a section, the width is the **smallest** that holds the section's **largest** value:

| Largest value in section | Width |
|---|---|
| ≤ 255 | `Eight` |
| ≤ 65,535 | `Sixteen` |
| ≤ 4,294,967,295 | `ThirtyTwo` |
| ≤ 9,007,199,254,740,991 (`2^53 − 1`) | `SixtyFour` |
| above that | rejected — see §5 |

- The bin dimensions are sized on their own (`BinDimensionsBitSize`).
- **All items share one** item-dimensions width and **one** item-coordinates width: the largest value across
  the whole item list drives each. An encoder **MUST** size by the maximum, not per item.
- When there are **no items**, both item widths default to `Eight`.

Note `SixtyFour` is a **storage width** (8 bytes), not a value ceiling. The largest value it may carry is
`2^53 − 1` (§5), so the top 11 bits of every 64-bit field are always zero. The width is still chosen because a
value above `4,294,967,295` does not fit 4 bytes.

## 5. Values and limits

This section is the heart of cross-language compatibility.

### 5.1 Interoperable integer range — `[0, 2^53 − 1]`

The two runtimes do not agree on how large an integer they hold exactly:

| Runtime | Largest exact integer |
|---|---|
| C# `ulong` | `18,446,744,073,709,551,615` (`2^64 − 1`) |
| JavaScript `number` | `9,007,199,254,740,991` (`2^53 − 1`) |

JavaScript is the limiting side: a value between `2^53` and `2^64` is fine in C# but JS rounds it silently. So
the format pins one ceiling both sides hold exactly.

> **Every dimension and coordinate MUST be in `[0, 2^53 − 1]`.** Call this constant `MaxInteger`
> (`9,007,199,254,740,991`, equal to JavaScript's `Number.MAX_SAFE_INTEGER`).

This is enforced in **both directions**:

- On **encode**, a value above `MaxInteger` **MUST** be rejected (it is not assigned a width — §4 stops at it).
- On **decode**, a 64-bit field whose stored value exceeds `MaxInteger` **MUST** be rejected, not returned. This
  stops a decoder from silently rounding a value some other encoder wrote above the ceiling.

C# `ulong` can physically hold more, but anything above `MaxInteger` is **outside ViPaq**.

### 5.2 Per-field rules

| Field | Rule |
|---|---|
| Bin dimensions L, W, H | **MUST** be ≥ 1. Zero or negative is rejected. |
| Item dimensions L, W, H | **MUST** be ≥ 1. Zero or negative is rejected. |
| Item coordinates X, Y, Z | **MUST** be ≥ 0. Negative is rejected; **zero is valid** (an item flush to the bin origin). |
| Item count | **MUST** be ≤ 65,535 (fits the `uint16` count field). |
| All of the above | **MUST** be ≤ `MaxInteger` (§5.1). |

## 6. Compression

After the body is built, an encoder decides whether to gzip it:

- **Canonical trigger:** if the **uncompressed body length is greater than 255 bytes**, the body **MUST** be
  gzipped and `Version` set to `CompressedGzip`; otherwise the body stays raw and `Version` is `Uncompressed`.
- The body length measured is the body **only** — it excludes the 1-byte header (which is not written yet).
- Gzip uses standard gzip (C# `GZipStream` at optimal level; JS/Web `CompressionStream('gzip')`). The header
  byte is prepended **after** compression, so it is never inside the gzip stream.

**Interop vs. byte-equality.** gzip output bytes are **not** identical across engines (different headers, OS
flag, deflate choices) — same input, same algorithm, different valid bytes. Therefore:

- For an **uncompressed** body, two conformant encoders produce **byte-identical** blobs. These can be compared
  exactly (golden vectors).
- For a **compressed** body, the blobs **differ**. They **MUST NOT** be byte-compared across implementations.
  The only contract is **cross-decode**: each side's compressed blob **MUST** decode on the other back to the
  original input.

A decoder **MUST** accept both an `Uncompressed` and a `CompressedGzip` body regardless of its own trigger.

## 7. Decoding order

A decoder **MUST**:

1. Reject input shorter than 1 byte.
2. Read byte 0 as the `EncodingInfo` (§2). Reject `Reserved2` / `Reserved3` `Version`.
3. If `Version == CompressedGzip`, wrap the rest of the input in a gzip decompressor; otherwise read it raw.
4. Read the `uint16` item count.
5. Read bin dimensions at `BinDimensionsBitSize`.
6. Repeat count times: read item dimensions at `ItemDimensionsBitSize`, then coordinates at
   `ItemCoordinatesBitSize`.

While reading steps 5 and 6, reject any 64-bit value the moment it reads above `MaxInteger` (§5.1) — do not
return it. Only a `SixtyFour` field can exceed the ceiling; narrower widths cannot.

## 8. Errors — what MUST be rejected

| Condition | Side |
|---|---|
| Bin or item dimension ≤ 0 | encode |
| Coordinate < 0 | encode |
| Any value > `MaxInteger` (`2^53 − 1`) | encode **and** decode |
| Item count > 65,535 | encode |
| Input shorter than 1 byte | decode |
| `Version` is `Reserved2` or `Reserved3` | decode |

(Each implementation maps these to its own exception type. The condition is normative; the exception type is not.)

## 9. Worked examples

### 9.1 Single 8-bit item (uncompressed)

Input: bin `10x20x30`, one item `1x2x3` at `(4,5,6)`.

- Bin max `30`, item-dim max `3`, item-coord max `6` — all ≤ 255 → every section is `Eight`.
- Header: `Version 0`, all sizes `Eight 0` → `0b00_00_00_00` = `0x00`.
- Body (11 bytes): count `01 00`, bin `0A 14 1E`, item dims `01 02 03`, item coords `04 05 06`.
- Body length 11 ≤ 255 → uncompressed.

```
00  01 00  0A 14 1E  01 02 03  04 05 06
^header ^count ^bin    ^dims    ^coords
```

### 9.2 Mixed widths (16-bit bin, 8-bit items)

Input: bin `1000x2x3`, one item `1x1x1` at `(0,0,0)`.

- Bin max `1000` → `Sixteen`. Item dims max `1` → `Eight`. Item coords max `0` → `Eight`.
- Header: `Version 0`, Bin `Sixteen 1`, ItemDim `Eight 0`, ItemCoord `Eight 0` → `0b00_01_00_00` = `0x10`.
- Bin dims as little-endian `uint16`: `1000` = `E8 03`, `2` = `02 00`, `3` = `03 00`.

```
10  01 00  E8 03 02 00 03 00  01 01 01  00 00 00
^header ^count ^bin (16-bit LE) ^dims    ^coords
```

This shows both the little-endian order and a `0` coordinate being valid.

## 10. Decisions log

Protocol decisions, newest first. Record date and rationale for anything that changes the wire or its rules.

- **2026-06-27 — Canonical compression trigger is body length > 255 bytes.** The C# library is the reference, so
  its rule defines the byte-exact golden vectors (`memoryStream.Length > byte.MaxValue`, body only — the header is
  prepended afterward). The TS mirror matches: `(bufferSize - 1) > 255`, where `getBufferSize` counts the 1-byte
  header, so `bufferSize - 1` is the body. (TS once triggered on the full buffer length — an off-by-one — now
  aligned, 2026-06-29.) Compressed blobs are still never byte-compared across implementations; only cross-decode
  is guaranteed (§6).
- **2026-06-27 — `PROTOCOL.md` is the normative spec; `README.md` is a short index.** The `.agents/docs/vipaq/`
  files stay as agent notes and link here.
- **2026-06-27 — Bits 7-6 keep the name `Version`; it currently encodes compression only.** There is no
  dedicated format-version field. `Reserved2`/`Reserved3` are unused and decoders reject them. A future format
  revision has no reserved version slot yet — that is a known limitation to revisit if the wire ever changes.
- **2026-06-26 — Interoperable integer range is `[0, 2^53 − 1]` (`MaxInteger`).** `2^53 − 1` is the largest
  integer all target runtimes hold exactly (JS `number`). Values above it are rejected on encode and decode, so
  no implementation can silently round. C# `ulong` can hold more, but that is outside ViPaq. The `SixtyFour`
  bucket stays an 8-byte storage width; only its accepted value range is capped at `MaxInteger`.
  Both C# and TypeScript enforce this ceiling on encode and decode as of 2026-06-30
  (`.agents/plans/vipaq-integer-range-spec.md`, Deliverable 4).

## 11. References

- C# canonical implementation — `src/Binacle.ViPaq/`
- TypeScript mirror — `binacle-vipaq/` (see `../.agents/docs/vipaq/typescript.md` for divergences)
- Shared cross-language test vectors — `test-vectors/` (and its `README.md`)
- Agent notes — `../.agents/docs/vipaq/README.md`
- Plans — `../.agents/plans/vipaq-integer-range-spec.md`, `../.agents/plans/vipaq-cross-language-testing.md`
</content>
</invoke>
