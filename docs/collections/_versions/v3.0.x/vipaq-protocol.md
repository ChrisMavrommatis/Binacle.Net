---
title: ViPaq Protocol
nav:
  order: 7
  icon: 🗜️
---

This page describes the ViPaq wire format as produced and read by Binacle.Net {{ page.version }}.
For what ViPaq is and why it exists, see [ViPaq Protocol]({% link _common_pages/vipaq-protocol.md %}).

> 🚨 **The format changed in v3.0.0.** Strings produced by v2.1.1 and earlier do not decode here, and strings
> produced here do not decode there. There is no fallback reader. An old string is rejected with a format error
> rather than misread, so the failure is visible - but stored strings are worth regenerating after the upgrade.
{: .block-warning}

## 📌 Structure

A ViPaq blob is a two byte header followed by a body:

```text
[ Header: 2 bytes ][ Body ]
```

- The header is **never compressed**. The body is built first, compressed if that makes it smaller, and the
  header is put in front afterwards.
- Every blob is therefore self-describing: a decoder reads the two header bytes, then reads the body exactly as
  they say.
- All multi-byte integers are **unsigned** and **little-endian**.

## 🧾 Header

Two bytes, one job each. Byte 0 says **how to read** the body, byte 1 says **how wide** its integers are.

##### Byte 0 - form

| Bits | Field | Values |
|---|---|---|
| 7-6 | `Version` | `0` for this format. `1`-`3` are reserved. |
| 5 | `Compressed` | `0` the body is raw, `1` the body is compressed |
| 4 | `Layout` | `0` row-major, `1` columnar |
| 3-0 | reserved | always `0`, a decoder rejects anything else |

##### Byte 1 - integer widths

| Bits | Field |
|---|---|
| 7-6 | `BinDimensionsWidth` |
| 5-4 | `ItemDimensionsWidth` |
| 3-2 | `ItemCoordinatesWidth` |
| 1-0 | reserved, always `0` |

`Compressed` and `Layout` describe that one blob, not the format. An encoder picks them per blob and a decoder
obeys them, so two blobs from the same version can differ in both.

## 📦 Body

The body carries the item count, the bin dimensions, then the items:

```text
[ Item count: uint16 ][ Bin L, W, H ][ Items ]
```

- **Item count** is always a 2 byte `uint16`, whatever the width fields say.
- **Bin dimensions** are Length, Width, Height, in that order.
- Items come back in the order they were sent.

`Layout` decides only how the items are arranged. The count and the bin dimensions are the same either way.

##### Row-major (`Layout = 0`)

Each item is written whole - its three dimensions, then its three coordinates - before the next item.

```text
L W H X Y Z | L W H X Y Z | L W H X Y Z | ...
```

##### Columnar (`Layout = 1`)

Each field is written for every item before moving on to the next field. Six runs, each as long as the item count.

```text
L L L ... | W W W ... | H H H ... | X X X ... | Y Y Y ... | Z Z Z ...
```

Columnar puts values of similar size next to each other, which usually compresses better. Row-major is easier to
read in a hex dump. Neither is required - the header bit says which was used.

## 🔢 Integer Widths

Each of the three sections - bin dimensions, item dimensions, item coordinates - is stored at one width, chosen
independently:

| Code | Bytes per integer | Largest value |
|---|---|---|
| `0` | 1 | 255 |
| `1` | 2 | 65,535 |
| `2`, `3` | reserved | a decoder rejects these |

Widths are fixed per section, not per value: every item shares one dimensions width and one coordinates width.
An encoder picks the smallest width that holds the largest value in the section.

The three sections are sized separately because real data needs them to be. A bin of `5000x2000x2000` holding
items no larger than `200` gives 16 bit bin dimensions, 8 bit item dimensions and 16 bit coordinates.

## 🗜️ Compression

The `Compressed` header bit records what the encoder did. It is not a hint to guess at.

- `Compressed = 0` - the bytes after the header are the body.
- `Compressed = 1` - the bytes after the header are a compressed stream holding the body.

The codec is **raw DEFLATE** ([RFC 1951](https://www.rfc-editor.org/rfc/rfc1951)), with **no** zlib or gzip
wrapper. The compressed stream is the DEFLATE bit stream and nothing else.

Compression is not applied by size. The encoder compresses the body, keeps whichever of the two is shorter, and
sets the bit to say which it kept - so a blob is never made larger by compressing it. A decoder accepts both.

## 🔤 Text Form

The stored and shared artifact is **base64**, and the format is optimized for it.

- Standard base64 ([RFC 4648 §4](https://www.rfc-editor.org/rfc/rfc4648#section-4)), alphabet `A-Z a-z 0-9 + /`,
  padded with `=`. Not the URL-safe alphabet.
- No line wrapping and no whitespace.

## 📏 Limits

| Field | Rule |
|---|---|
| Bin dimensions L, W, H | 1 to 65,535 |
| Item dimensions L, W, H | 1 to 65,535 |
| Item coordinates X, Y, Z | 0 to 65,535. Zero is valid - an item flush to the bin origin. |
| Item count | up to 65,535 |

A value outside these ranges is an error when encoding. Nothing is clamped or widened silently.

## 🧪 Example

A bin of `10x20x30` holding one item `1x2x3` at `(4,5,6)`.

Every value fits in a byte, so all three sections use the 1 byte width, and the encoder wrote it row-major and
uncompressed. Both header bytes are therefore `0x00`:

```text
00 00  01 00  0A 14 1E  01 02 03  04 05 06
^byte0 ^count ^bin      ^dims     ^coords
   ^byte1
```

## 📖 Full Specification

The normative wire specification, including the decoding order, everything a decoder must reject, and further
worked examples, lives with the source:

- 📖 [ViPaq Protocol Specification](https://github.com/binacle-labs/Binacle.Net/blob/v3.0.0/vipaq/PROTOCOL.md)
