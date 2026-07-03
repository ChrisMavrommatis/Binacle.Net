---
description: Binacle.ViPaq — compact binary format for encoding packing results. Wire layout, encoding-info header, C# API surface, and limits.
verified: 2026-07-03
check: Wire layout, EncodingInfo bit packing, enums, and public method signatures match vipaq/src/Binacle.ViPaq/
also_update:
  - vipaq/typescript.md
---

# ViPaq

> **Warning: ViPaq is experimental and may change.**

`Binacle.ViPaq` is a compact binary format for encoding a packing result: one `Bin` (dimensions) plus a list of
items (dimensions + position coordinates). The C# library is the canonical implementation. A hand-maintained
TypeScript mirror lives at `vipaq/binacle-vipaq` — see [typescript.md](typescript.md) (the two differ in real ways).

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
| `Serialize` (generic) | `byte[] Serialize<TBin,TItem,T>(TBin bin, IList<TItem> items)` | `T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>` |
| `Deserialize` (generic) | `(TBin, IList<TItem>) Deserialize<TBin,TItem,T>(byte[] data)` | plus `new()` on `TBin`/`TItem` |

Type constraints use ViPaq's **own** interfaces in `Binacle.ViPaq.Abstractions` — separate from the Lib `IWith*`
interfaces. `TBin : IWithDimensions<T>`; `TItem : IWithDimensions<T>, IWithCoordinates<T>`. A type that gets
serialized directly (e.g. v4 `PackedBox`) implements `IWithDimensions<int>` / `IWithCoordinates<int>`.

The lib also ships canonical concrete models — `Bin<T>` (dimensions), `Item<T>` (dimensions + coordinates),
`Dimensions<T>`, and `Coordinates<T>` (with `Dimensions.Create` / `Coordinates.Create` factories for inferred
`T`) — so callers don't define their own. (`Bin<T>` and `Dimensions<T>` share a shape but are distinct roles.)

### Compact notation (experimental)

`CompactNotation` — marked `[Experimental("BINACLE_VIPAQ_COMPACT")]` — is a **text** companion to the binary
format. It parses and formats the human-readable shorthand used by the shared test vectors and the interop
generators, so that grammar lives in one place instead of a copy per project:

| Methods | Text form |
|---|---|
| `ParseBin<T>` · `ParseDimensions<T>` · `FormatDimensions<T>` | `"100x100x100"` |
| `ParseCoordinates<T>` · `FormatCoordinates<T>` | `"0,0,0"` |
| `ParseItem<T>` · `ParseItems<T>` · `FormatItem<T,TItem>` | `"10x10x10 (0,0,0)"` (`ParseItems` also expands a `":Q"` repeat) |
| `ParseEncodingInfo` · `FormatEncodingInfo` | `"Uncompressed_8_8_8"` (`"Compressed"` = gzip) |

Parse is lenient about range (it just reads the integers); `Serialize` still enforces `[0, MaxInteger]`. Parse
returns the concrete `Bin<T>` / `Item<T>`. Consumers opt into the experimental API with
`<NoWarn>BINACLE_VIPAQ_COMPACT</NoWarn>`. TS mirror: `src/compactNotation.ts` (see [typescript.md](typescript.md)).

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
| `vipaq/test/Binacle.ViPaq.UnitTests` | `vipaq` | `BitSizeHelper`, `EncodingInfoHelper` (encode/decode, throws) — uses Bogus fakers |
| `vipaq/binacle-vipaq` | — | TypeScript mirror — `npm test` (jest); see [typescript.md](typescript.md) |
