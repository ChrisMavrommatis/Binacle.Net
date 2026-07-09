---
description: Binacle.ViPaq — compact binary format for encoding packing results. Wire layout, encoding-info header, C# API surface, and limits.
verified: 2026-07-05
check: Wire layout, EncodingInfo bit packing, enums, and public method signatures match vipaq/src/Binacle.ViPaq/
also_update:
  - vipaq/typescript.md
  - vipaq/cross-language-testing.md
---

# ViPaq

> **Warning: ViPaq is experimental and may change.**

> ⚠️ **The spec has moved ahead of the code.** `vipaq/PROTOCOL.md` now describes a **rebuilt, breaking** wire
> format: a 2-byte header, `Compressed` and `Layout` as per-blob flags, 8/16-bit widths only, and values capped
> at 65,535. **This document still describes the shipped library, which has not been rebuilt yet.** It is accurate
> to the code and wrong about the format. It gets rewritten when the C# library lands the new wire — see
> `.agents/plans/vipaq/README.md`. The same is true of [typescript.md](typescript.md) and
> [cross-language-testing.md](cross-language-testing.md).

`Binacle.ViPaq` is a compact binary format for encoding a packing result: one `Bin` (dimensions) plus a list of
items (dimensions + position coordinates). The C# library is the canonical implementation. A hand-maintained
TypeScript mirror lives at `vipaq/packages/binacle-vipaq` — see [typescript.md](typescript.md) (the two differ in real ways).

Used in v3 and v4 API responses when `IncludeViPaqData: true`. The serializer returns **raw `byte[]`** — the API
layer base64-encodes it (`v4/Contracts/BinResponseBase.cs`, `v3/Contracts/PackResponse.cs`). Base64 is **not**
part of the format.

## C# API surface

All entry points are static on `ViPaqSerializer`. The typed methods are thin wrappers over the generic ones.

| Method | Signature | Notes |
|---|---|---|
| `SerializeInt32` | `byte[] SerializeInt32<TBin,TItem>(TBin bin, IList<TItem> items)` | `T = int` |
| `SerializeUInt16` | `byte[] SerializeUInt16<TBin,TItem>(TBin bin, IList<TItem> items)` | `T = ushort` |
| `DeserializeInt32` | `(TBin, IList<TItem>) DeserializeInt32<TBin,TItem>(byte[] data)` | `TBin`/`TItem` need `new()` |
| `DeserializeUInt16` | `(TBin, IList<TItem>) DeserializeUInt16<TBin,TItem>(byte[] data)` | `T = ushort` |
| `Serialize` (generic) | `byte[] Serialize<TBin,TItem,T>(TBin bin, IList<TItem> items)` | `T : struct, IBinaryInteger<T>` (which already implies `INumber<T>` / `IComparable<T>`) |
| `Deserialize` (generic) | `(TBin, IList<TItem>) Deserialize<TBin,TItem,T>(byte[] data)` | plus `new()` on `TBin`/`TItem` |

Type constraints use the shared `Binacle.Geometry` interfaces (namespace `Binacle.Geometry`,
`shared/src/Binacle.Geometry/`) — ViPaq no longer defines its own `IWith*` family; it deleted the old
`Binacle.ViPaq.Abstractions` copies and points at the leaf. `TBin : IWithDimensions<T>`;
`TItem : IWithDimensions<T>, IWithCoordinates<T>` (all constrained `where T : struct, IBinaryInteger<T>`, matching the
serializer). A type that gets serialized directly (e.g. v4 `PackedBox`) implements the leaf's `IWithDimensions<int>` /
`IWithCoordinates<int>`.

The lib ships two **public** concrete models — `Bin<T>` (dimensions) and `Item<T>` (dimensions + coordinates), both
in `Binacle.ViPaq` and implementing the leaf interfaces — as ready-made types a caller can serialize without defining
their own. (`Dimensions<T>` / `Coordinates<T>` used to ship here too, but nothing in the format serializes a standalone
measurement or point; the concrete generic `Dimensions<T>` / `Coordinates<T>` now live in the `Binacle.Geometry` leaf,
and vipaq's tests keep their own copies as fixtures in `vipaq/test/Binacle.ViPaq.UnitTests/Models/`.)

Everything else is `internal` — implementation detail, not consumer API: `BitSizeHelper`, `EncodingInfoHelper`,
`ProtocolReader<T>` / `ProtocolWriter<T>` (+ their extension methods), and `EncodingInfoNotation`. The test and
tools assemblies reach them via `InternalsVisibleTo`. So the whole **public surface** is: `ViPaqSerializer`,
`Bin<T>` / `Item<T>`, the wire types `EncodingInfo` / `BitSize` / `Version`, and `ViPaqLimits` — the `IWith*`
constraint interfaces come from the shared `Binacle.Geometry` leaf, not from vipaq.

### Encoding-info notation (internal)

The geometry text notation (dimensions / coordinates / items, e.g. `"10x10x10 (0,0,0)"`) is **not** in vipaq — it
lives in the shared `Binacle.CompactNotation` project, used by the test vectors and interop generators so the
grammar sits in one place. vipaq keeps only `EncodingInfoNotation` — an `internal` helper that parses/formats the
**header** string via `ParseEncodingInfo` / `FormatEncodingInfo`:

| Methods | Text form |
|---|---|
| `ParseEncodingInfo` · `FormatEncodingInfo` | `"Uncompressed_8_8_8"` (`"Compressed"` = gzip) |

It stays in the library because it's wire-specific — it names `EncodingInfo` / `BitSize` / `Version`, which the
leaf `Binacle.CompactNotation` can't hold. TS mirror: `src/encodingInfoNotation.ts` (see
[typescript.md](typescript.md)).

## Wire layout

Field order (all multi-byte integers are **little-endian**):

| # | Field | Width | Notes |
|---|---|---|---|
| 1 | EncodingInfo header | 1 byte | always at index 0; never compressed |
| 2 | Item count | `ushort` (2 bytes) | `(ushort)items.Count` |
| 3 | Bin dimensions | L, W, H — each at `BinDimsBitSize` | in L, W, H order |
| 4 | Per item (× count) | dims L, W, H at `ItemDimsBitSize`, then coords X, Y, Z at `ItemCoordsBitSize` | dims first, then coords |

Each section's integer width is chosen per-payload and recorded in the header (see below). Width → bytes:
`Eight`→1, `Sixteen`→2, `ThirtyTwo`→4, `SixtyFour`→8.

## EncodingInfo header (1 byte)

Four 2-bit fields, MSB → LSB:

```
bits 7-6 : Version
bits 5-4 : BinDimensionsBitSize
bits 3-2 : ItemDimensionsBitSize
bits 1-0 : ItemCoordinatesBitSize
```

`BitSize` enum: `Eight = 0`, `Sixteen = 1`, `ThirtyTwo = 2`, `SixtyFour = 3` (8/16/32/64-bit storage).
The width for each section is the **smallest that fits the largest value** in that section: all values ≤ 255 →
`Eight`; ≤ 65535 → `Sixteen`; ≤ uint.MaxValue → `ThirtyTwo`; else `SixtyFour`. Bin dims are sized independently;
item dims share one width across all items, item coords share another.

`Version` enum: `Uncompressed = 0`, `CompressedGzip = 1`, `Reserved2 = 2`, `Reserved3 = 3`.

## Compression

After the body is built, if the **uncompressed body length > 255 bytes** (`byte.MaxValue`), the body is gzipped
(`CompressionLevel.Optimal`) and `Version` is set to `CompressedGzip`. Otherwise it stays `Uncompressed`. The
1-byte header is prepended **after** compression, so the header is never inside the gzip stream. (This is an
adaptive bit-width + optional-gzip scheme — there is no per-value variable-length encoding.)

## Limits and throws

- Item count > `ushort.MaxValue` (65535) → `ArgumentOutOfRangeException`.
- Any dimension ≤ 0 → throws ("must be greater than zero"). Coordinates may be 0 but not negative.
- Values exceeding 64-bit → throws ("too large").
- `Deserialize` with null or `< 1` byte → `ArgumentException`.
- `Deserialize` where the stored bit size is wider than the chosen `T` can hold → `ArgumentOutOfRangeException`
  (`ThrowOnInvalidEncodingInfo<T>`). So pick `T` to match how the data was written (`Int32` is the safe default).

## Related Tests

| Project | Alias | Covers |
|---|---|---|
| `vipaq/test/Binacle.ViPaq.UnitTests` | `vipaq` | serializer round-trips + exact-byte golden vectors; the internal `BitSizeHelper` / `EncodingInfoHelper` / protocol read-write (reached via `InternalsVisibleTo`); curated data, not Bogus |
| `vipaq/packages/binacle-vipaq` | — | TypeScript mirror — `npm test` (jest); see [typescript.md](typescript.md) |

How the two languages are held to one wire format — the shared vectors, the generators, and the
decode-to-input contract for compressed payloads — is in
[cross-language-testing.md](cross-language-testing.md).
