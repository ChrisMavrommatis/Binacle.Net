# ViPaq Protocol Specification

> **Status: experimental.** ViPaq may change. This document defines the wire format and is the authority on what
> the bytes mean. Where an implementation differs from this document, the implementation has the bug, not the
> spec. This document stands alone: everything needed to encode or decode a ViPaq blob is here, with no
> dependency on any other file.

ViPaq is a compact binary format for one packing result: a single **bin** (dimensions) plus a list of **items**
(dimensions and position coordinates). It is designed to be stored and moved as a short base64 text token (§9).

An **implementation** is any encoder/decoder pair conforming to this document. Nothing here depends on a
programming language, a runtime, or a compression library.

## Notation and conformance

The words **MUST**, **MUST NOT**, **SHOULD**, and **MAY** are used as in RFC 2119.

- A **byte** is 8 bits. Bit 7 is the most significant, bit 0 the least.
- All multi-byte integers are **unsigned** and **little-endian** (least significant byte first).
- Hex bytes are written `0x0A`. Header bit fields are written `0bAA_B_C_DDDD` (one group per field).
- An **encoder** produces a blob from a bin + items. A **decoder** reads a blob back into a bin + items.

## 1. Top-level structure

A ViPaq blob is a two-byte header followed by a body:

```
[ Header : 2 bytes ][ Body ]
```

- The header is **never compressed**. The body is built first and compressed if that makes it smaller (§6); the
  header is added in front afterwards.
- So every blob is **self-describing**: a decoder reads the two header bytes first, then reads or decompresses
  the body according to them.
- Base64 is **not** part of the binary format, but it is the stored form (§9). Encoders return raw bytes.

## 2. Header (2 bytes)

Each byte has one job. Byte 0 says **how to read** the body; byte 1 says **how wide** its integers are.

### 2.1 Byte 0 — form

| Bits | Field | Values |
|---|---|---|
| 7-6 | `Version` | `0` = this spec. `1`-`3` reserved. |
| 5 | `Compressed` | `0` body is raw, `1` body is a compressed stream (§6) |
| 4 | `Layout` | `0` row-major, `1` columnar (§3) |
| 3-0 | reserved | **MUST** be written `0`; a decoder **MUST** reject a non-zero value |

```
byte0 = (Version << 6) | (Compressed << 5) | (Layout << 4)
```

### 2.2 Byte 1 — widths

| Bits | Field | Values |
|---|---|---|
| 7-6 | `BinDimensionsWidth` | `Width` (§4) |
| 5-4 | `ItemDimensionsWidth` | `Width` (§4) |
| 3-2 | `ItemCoordinatesWidth` | `Width` (§4) |
| 1-0 | reserved | **MUST** be written `0`; a decoder **MUST** reject a non-zero value |

```
byte1 = (BinDimensionsWidth << 6) | (ItemDimensionsWidth << 4) | (ItemCoordinatesWidth << 2)
```

### 2.3 Room to grow

- `Version` claims code `0`; three codes remain. A change the flags and width codes cannot express — a different
  compression codec, a new body shape — takes the next `Version`.
- `Compressed` and `Layout` describe *this* blob, not the format. An encoder chooses them per blob; a decoder
  obeys them. Both may vary between two blobs of the same `Version`.
- Each width field has **two spare codes**, leaving room for a variable-length encoding (§4) with one code still
  in hand.

## 3. Body layout

The body carries, in order: the item count, the bin dimensions, then the items. `Layout` (§2.1) decides only how
the **items** are arranged. Count and bin dimensions are the same in both layouts.

```
[ Item count : uint16 ][ Bin L, W, H ][ Items ]
```

- **Item count** is **always** a 2-byte `uint16`, independent of every `Width` field. It **MUST** be ≤ 65,535.
- **Bin dimensions** are Length, Width, Height, in that order, each at `BinDimensionsWidth`.
- Items appear in the order the encoder received them. A decoder **MUST** return them in that same order.

### 3.1 Row-major (`Layout = 0`)

Each item is written whole: its three dimensions, then its three coordinates. Then the next item.

```
L W H X Y Z | L W H X Y Z | L W H X Y Z | ...
```

### 3.2 Columnar (`Layout = 1`)

Each field is written for every item before moving to the next field. Six runs, each `count` values long.

```
L L L ... | W W W ... | H H H ... | X X X ... | Y Y Y ... | Z Z Z ...
```

Dimensions are at `ItemDimensionsWidth`, coordinates at `ItemCoordinatesWidth`, in both layouts. Columnar puts
like magnitudes next to each other, which usually compresses better; row-major is easier to read in a hex dump.
Neither is normative — the bit says which was used.

## 4. Integer widths (`Width`)

Each of the three sections is stored at one width, chosen independently:

| `Width` | Value | Bytes per integer | Largest value |
|---|---|---|---|
| `Eight` | `0` | 1 | 255 |
| `Sixteen` | `1` | 2 | 65,535 |
| reserved | `2` | — | — |
| reserved | `3` | — | — |

A decoder **MUST** reject width codes `2` and `3`.

**Structure (normative).**

- Each section has **exactly one** width. All items share one item-dimensions width and one item-coordinates
  width. An encoder **MUST NOT** vary width per item.
- A chosen width **MUST** hold every value in its section. Writing `Eight` for a section containing `300` is an
  error.
- When there are **no items**, both item widths **MUST** be written `Eight`.
- A decoder **MUST** read each section at the width the header declares. It **MUST NOT** re-derive widths from
  the values it reads.

**Selection (policy).** An encoder **SHOULD** pick the **smallest** width that holds the section's largest value:
`Eight` if that value is ≤ 255, otherwise `Sixteen`. It **MAY** pick a wider one — a blob whose sections are
wider than they need to be is larger, but conformant, and decodes to the same input. This is what makes every
width combination forceable for testing.

The three sections are sized separately because real data needs them to be. A bin of `5000x2000x2000` holding
items no larger than `200` packs to a 16-bit bin, 8-bit item dimensions, and 16-bit coordinates.

## 5. Values and limits

Every dimension and coordinate is an unsigned integer in **`[0, 65,535]`**.

| Field | Rule |
|---|---|
| Bin dimensions L, W, H | **MUST** be ≥ 1 |
| Item dimensions L, W, H | **MUST** be ≥ 1 |
| Item coordinates X, Y, Z | **MUST** be ≥ 0. **Zero is valid** — an item flush to the bin origin. |
| All of the above | **MUST** be ≤ 65,535 |
| Item count | **MUST** be ≤ 65,535 |

Encoding a value above 65,535 **MUST** be an error. There is no saturation and no widening.

A decoder has **nothing to range-check**: a value read from an 8- or 16-bit field is in range by construction.

## 6. Compression

`Compressed` (§2.1) records what the encoder did. It does not tell a decoder to guess.

- `Compressed = 0` — the bytes after the header are the body.
- `Compressed = 1` — the bytes after the header are a **compressed stream** whose contents are the body.

The codec is fixed by `Version`. There is no codec field, so changing the codec takes the next `Version`.

For `Version = 0` the codec is **raw DEFLATE** (RFC 1951), with **no** zlib (RFC 1950) or gzip (RFC 1952)
wrapper. The compressed stream is exactly the DEFLATE bit stream and nothing else: the header already says the
body is compressed and the body already carries its own length, so a wrapper's framing and checksum would be dead
weight. A decoder inflates the bytes after the header as a raw DEFLATE stream; bytes that are not a valid DEFLATE
stream are a malformed blob.

**Choosing.** An encoder **SHOULD** compress the body, keep whichever of the two is shorter, and set `Compressed`
to say which it kept. This never inflates a blob and has no threshold to get wrong. An encoder **MAY** compress
unconditionally or never — for measurement, or because it knows its data. Any such blob is still conformant: the
bit is normative, the policy is not.

A decoder **MUST** accept both values regardless of what it would have chosen itself.

### 6.1 Determinism — when bytes may be compared

Three things are the encoder's choice: the widths (§4), `Layout`, and `Compressed`. All three are recorded in the
header, so a decoder never has to guess — but it means **two conformant encoders given the same input may emit
different blobs**, and both are right. Byte-comparison is only meaningful once the header is pinned.

- **Same header, uncompressed** → the body is **byte-identical**, always, in every implementation. This is the
  only exact-comparison contract, and it is what golden test vectors rest on. A vector that expects exact bytes
  **MUST** state the full header it expects them under.
- **Compressed** → bytes **MUST NOT** be compared at all. The same body, the same codec, and two different
  compressor builds can each emit different, equally valid streams. Never rely on compressed bytes being equal
  *or* unequal.
- **Any blob, either way** → the contract is **decode-to-input**: any conformant blob **MUST** decode back to the
  original bin and items, in the original order, in every implementation.

## 7. Decoding order

A decoder **MUST**:

1. Reject input shorter than 2 bytes.
2. Read byte 0. Reject a `Version` other than `0`. Reject non-zero reserved bits.
3. Read byte 1. Reject width code `2` or `3` in any section. Reject non-zero reserved bits.
4. If `Compressed = 1`, decompress the rest of the input; otherwise read it raw.
5. Read the `uint16` item count.
6. Read bin dimensions at `BinDimensionsWidth`.
7. Read the items according to `Layout` (§3), dimensions at `ItemDimensionsWidth`, coordinates at
   `ItemCoordinatesWidth`.
8. Reject the blob if any body bytes remain unread.

No value check is needed while reading. An 8- or 16-bit field cannot hold an out-of-range value.

## 8. Errors — what MUST be rejected

| Condition | Side |
|---|---|
| Bin or item dimension < 1 | encode |
| Coordinate < 0 | encode |
| Any value > 65,535 | encode |
| Item count > 65,535 | encode |
| A section's declared width cannot hold one of its values | encode |
| Input shorter than 2 bytes | decode |
| `Version` is not `0` | decode |
| Width code is `2` or `3` | decode |
| Any reserved bit is non-zero | decode |
| Body ends before the declared item count is read | decode |
| Body has bytes left over after the last item is read | decode |

(Each implementation signals these in whatever way suits its language. The condition is normative; how it is
raised is not.)

## 9. Text form

The stored artifact is **base64**, and it is what the format is optimised for. Encoders return bytes; the caller
encodes them.

- Standard base64 (RFC 4648 §4), alphabet `A-Z a-z 0-9 + /`, padded with `=`. Not the URL-safe alphabet.
- No line wrapping, no whitespace.
- Three bytes become four characters. Blob length therefore matters in **steps of three**: growing a blob from 13
  to 15 bytes costs no extra characters.

## 10. Worked examples

### 10.1 Single 8-bit item, row-major, uncompressed

Input: bin `10x20x30`, one item `1x2x3` at `(4,5,6)`.

- Every value ≤ 255 → all three sections are `Eight` (`0`).
- Byte 0: `Version 0`, `Compressed 0`, `Layout 0` → `0b00_0_0_0000` = `0x00`.
- Byte 1: all widths `0` → `0x00`.

```
00 00  01 00  0A 14 1E  01 02 03  04 05 06
^byte0 ^count ^bin      ^dims     ^coords
   ^byte1
```

### 10.2 Mixed widths (16-bit bin, 8-bit items)

Input: bin `1000x2x3`, one item `1x1x1` at `(0,0,0)`.

- Bin max `1000` → `Sixteen` (`1`). Item dims max `1` → `Eight`. Item coords max `0` → `Eight`.
- Byte 0: `0x00`. Byte 1: `0b01_00_00_00` = `0x40`.
- Bin dims as little-endian `uint16`: `1000` = `E8 03`, `2` = `02 00`, `3` = `03 00`.

```
00 40  01 00  E8 03 02 00 03 00  01 01 01  00 00 00
                ^bin (16-bit LE)  ^dims     ^coords
```

This shows both the little-endian order and a `0` coordinate being valid.

### 10.3 Columnar, two items

Input: bin `10x20x30`; items `1x2x3` at `(0,0,0)` and `4x5x6` at `(1,2,3)`.

- All sections `Eight`. Byte 0: `Layout 1` → `0b00_0_1_0000` = `0x10`. Byte 1: `0x00`.

```
10 00  02 00  0A 14 1E  01 04  02 05  03 06  00 01  00 02  00 03
              ^bin      ^L     ^W     ^H     ^X     ^Y     ^Z
```

The same items row-major would be `01 02 03 00 00 00  04 05 06 01 02 03`.

## 11. Conformance

An implementation conforms if it:

1. Encodes and decodes every structure in §1–§6 exactly as written.
2. Rejects every condition in §8.
3. Round-trips: decoding an encoded blob returns the original bin and items, in the original order.
4. Reproduces the worked examples in §10 byte for byte (they pin their full header).
5. **Decodes** any blob produced by any other conforming encoder, compressed or not.
6. Given the same input **and the same header**, emits the same uncompressed bytes as any other conforming
   encoder (§6.1).

Points 5 and 6 are what make the format portable. Note what is **not** required: two encoders need not choose the
same header for the same input, so they need not emit the same blob. An implementation may be written in any
language.

## 12. Open questions

None. The one that stood here — the compression codec for `Version = 0` — is settled: raw DEFLATE (§6).
