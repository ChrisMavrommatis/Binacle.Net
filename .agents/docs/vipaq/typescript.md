---
description: Binacle.ViPaq TypeScript mirror (vipaq/binacle-vipaq) — public API and how it differs from the C# library
verified: 2026-07-02
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

Header bit packing, `BitSize` (0–3) and `Version` (0–3, gzip = 1) enum values, the 8/16/32-bit selection
thresholds (255 / 65535 / 4.29e9), little-endian byte order, the field order (header, `ushort` count, bin L/W/H,
per-item dims then coords), the 65535 item-count cap, and the gzip algorithm. (The 64-bit bucket diverges — see
below.)

## How it differs from C# — read before assuming parity

| Aspect | C# | TypeScript |
|---|---|---|
| API shape | `SerializeInt32` / `SerializeUInt16` + generic `Serialize<TBin,TItem,T>` | single `serialize` / `deserialize`, JS `number` only — no width-typed or generic variants |
| Sync | synchronous, returns `byte[]` | **async**, returns `Promise<Uint8Array>` |
| Width validation | `ThrowOnInvalidEncodingInfo<T>` checks `T` can hold the stored bit sizes | range-checks every write and rejects a decoded 64-bit value above `MaxInteger` (2^53−1) |
| 64-bit values | `ulong` storage, value capped at `MaxInteger` (2^53−1) | float math (`left + 2**32*right`), same `MaxInteger` cap (`Number.MAX_SAFE_INTEGER`) — both enforce the ceiling (PROTOCOL.md §5) |
| Compression trigger | uncompressed **body** length > 255 | matches: `(bufferSize − 1) > 255` (body only) |

### Integer range — `[0, 2^53 − 1]`

Both implementations enforce the protocol's interoperable ceiling (`MaxInteger`, 2^53−1): every dimension/coordinate
is range-checked on encode, and a decoded 64-bit value above it is rejected rather than silently rounded. C# was
brought to this ceiling on 2026-06-30 — see PROTOCOL.md §5 and `.agents/plans/vipaq-integer-range-spec.md`.

(The old `getByteSize` under-allocation bug — `ThirtyTwo → 3`, `SixtyFour → 4` — is fixed; widths are now 4 and 8.)

## Tests

`npm test` (jest, from `vipaq/binacle-vipaq`; run `npm install` first — needs `@types/node`). 18 suites,
949 tests — unit tests on the utils (`createEncodingInfo`, `getDimensionsBitSize`, `getCoordinatesBitSize`,
`getByteSize`, `getBufferSize`, …), the `ProtocolReader` / `ProtocolWriter` little-endian and range-limit
guards, and `ViPaqSerializer` round-trips.

The suite now reads the **shared cross-language vectors** in `vipaq/test-vectors/` — the same files the C#
suite reads — via `tests/support/vectorReader.ts` (`readVectors`, `fs`-based) and `tests/support/vectorParser.ts`
(free functions mirroring C# `VectorParser`: `parseDimensions` splits `x`, `parseCoordinates` splits `,`,
`parseItems` composes both, `parseEncodingInfo`, `parseBitSize`). Providers in `tests/providers/` parse each
file into arrays consumed by `test.each`. So both implementations grade against one answer key and can't
silently drift on the wire format. (Still out of scope: the gzip cross-decode matrix — see
`.agents/plans/vipaq-cross-language-testing.md`.)
