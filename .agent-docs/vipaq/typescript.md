---
description: Binacle.ViPaq TypeScript mirror (vipaq/binacle-vipaq) — public API, how it differs from the C# library, and the known buffer-size bug
verified: 2026-06-10
check: TS API signatures and divergences match vipaq/binacle-vipaq/src/
also_update:
  - vipaq/README.md
---

# ViPaq — TypeScript Mirror

`vipaq/binacle-vipaq` is a **hand-maintained** TypeScript reimplementation of the C# ViPaq format — no codegen,
no shared schema. It mirrors the C# file structure 1:1 by hand. Any change to the C# wire format must be
replicated here manually, and the two must be kept byte-compatible by hand. See [README.md](README.md) for the
canonical format.

## Public API

`ViPaqSerializer` (default export) exposes two **async** static methods:

```ts
ViPaqSerializer.serialize(bin: Dimensions, items: (Dimensions & Coordinates)[]): Promise<Uint8Array>
ViPaqSerializer.deserialize(data: Uint8Array): Promise<DeserializedResult>   // { bin, items }
```

Both return raw bytes (no base64). They are **async** because gzip uses the Web Streams `CompressionStream` /
`DecompressionStream` API. `index.ts` also re-exports the `Dimensions` and `Coordinates` types.

## Identical to C# (by design)

Header bit packing, `BitSize` (0–3) and `Version` (0–3, gzip = 1) enum values, the bit-size selection thresholds
(255 / 65535 / 4.29e9 / 9.22e18), little-endian byte order, the field order (header, `ushort` count, bin L/W/H,
per-item dims then coords), the 65535 item-count cap, and the gzip algorithm.

## How it differs from C# — read before assuming parity

| Aspect | C# | TypeScript |
|---|---|---|
| API shape | `SerializeInt32` / `SerializeUInt16` + generic `Serialize<TBin,TItem,T>` | single `serialize` / `deserialize`, JS `number` only — no width-typed or generic variants |
| Sync | synchronous, returns `byte[]` | **async**, returns `Promise<Uint8Array>` |
| Width validation | `ThrowOnInvalidEncodingInfo<T>` checks `T` can hold the stored bit sizes | no analog (JS uses `number`) |
| 64-bit values | exact `ulong` | float math (`left + 2**32*right`), capped at `Number.MAX_SAFE_INTEGER` (2^53) |
| Compression trigger | uncompressed **body** length > 255 | **total** buffer (incl. the 3 header/count bytes) > 255 — boundary can differ |

### Known bug — buffer under-allocation for ≥32-bit data

`src/utils/getByteSize.ts` returns `ThirtyTwo → 3` and `SixtyFour → 4` (should be **4** and **8**). It is only
used by `getBufferSize` to pre-allocate the write buffer, so:

- For 8/16-bit data (the normal case — dims/coords ≤ 65535) the TS output is byte-identical to C#.
- For data needing ≥32-bit widths, `serialize` under-allocates the buffer and the output is corrupt.

The read/write protocol code itself uses correct byte counts; only the pre-allocation is wrong. Treat the TS
mirror as reliable for ≤16-bit values only until this is fixed.

## Tests

`npm test` (jest, from `vipaq/binacle-vipaq`). Tests are unit-level on the utils (`createEncodingInfo`,
`encodingUtils`) plus a placeholder sanity test. There is **no** round-trip cross-check against C# fixtures, so
wire compatibility is not currently enforced by an automated test — verify by hand when changing either side.
