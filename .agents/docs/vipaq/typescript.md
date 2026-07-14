---
description: Binacle.ViPaq TypeScript mirror (vipaq/packages/binacle-vipaq) — public API and how it differs from the C# library
verified: 2026-07-14
check: TS API signatures and divergences match vipaq/packages/binacle-vipaq/src/
also_update:
  - vipaq/README.md
---

# ViPaq — TypeScript Mirror

`vipaq/packages/binacle-vipaq` is a **hand-maintained** TypeScript reimplementation of the ViPaq format — no
codegen, no shared schema. It mirrors the C# file structure by hand, and the two are kept byte-compatible by hand.
Any change to the C# wire must be replicated here. The normative format is `vipaq/PROTOCOL.md`; see
[README.md](README.md) for the C# side.

## Public API

`ViPaqSerializer` (default export) exposes two **async** static methods:

```ts
ViPaqSerializer.serialize(
  bin: Dimensions,
  items: (Dimensions & Coordinates)[],
  options?: ViPaqSerializationOptions,   // { compress?: boolean; layout?: Layout }
): Promise<Uint8Array>

ViPaqSerializer.deserialize(data: Uint8Array): Promise<DeserializedResult>   // { bin, items }
```

Both work in raw bytes (no base64). They are **async** because the codec uses the Web Streams
`CompressionStream('deflate-raw')` / `DecompressionStream`. `options` mirrors C# `ViPaqSerializationOptions`:
`compress` (default `false`) and `layout` (default `RowMajor`). `index.ts` re-exports `Dimensions`, `Coordinates`,
`Layout`, and the `ViPaqSerializationOptions` type.

`src/headerNotation.ts` mirrors C# `HeaderNotation` — the header's text form for the test vectors,
`v{N}_{raw|comp}_{row|col}_{binW}_{itemDimW}_{itemCoordW}` (e.g. `v1_comp_col_16_8_16`). The **geometry** text
notation (`"LxWxH (X,Y,Z) [Q]"`) is **not** here — it lives in the shared `binacle-compact-notation` package (the
TS mirror of C# `Binacle.CompactNotation`), which both the vector parser and the interop generator import.

## Identical to C# (by design)

The two header bytes and their bit packing, `Width` (`Eight = 0`, `Sixteen = 1`; codes 2–3 reserved), `Layout`
(`RowMajor = 0`, `Columnar = 1`), `Version`, the per-section width choice (narrowest that fits, both item widths
`Eight` when there are no items), little-endian order, the field order (2-byte header, then the body: `uint16`
count, bin L/W/H, per-item dims then coords in the layout's order), the `[0, 65535]` value cap, the 65,535
item-count cap, and the codec (**raw DEFLATE**, decisions.md D16). The codec is resolved from the header: DEFLATE
when the compressed bit is set, a pass-through NoOp when not.

## How it differs from C# — read before assuming parity

| Aspect | C# | TypeScript |
|---|---|---|
| API shape | generic `Serialize<TBin,TItem,T>` + `Action<ViPaqSerializationOptions>` | single `serialize` / `deserialize`, JS `number` only; options is an optional object |
| Sync | synchronous, returns `byte[]` | **async**, returns `Promise<Uint8Array>` (the browser codec is stream-based) |
| Codec impl | `DeflateStream` (raw DEFLATE) | `CompressionStream('deflate-raw')` — the `-raw` variant, so no zlib header |

Compressed bytes are **not** byte-identical across the two engines; the guarantee is decode-to-input (PROTOCOL.md
§6.1), which the interop matrix proves.

## Tests

`npm test` (jest, from `vipaq/packages/binacle-vipaq`; run `npm install` first). 20 suites, 334 tests — unit tests
on the utils (`createHeader`, `getDimensionsWidth`, `getCoordinatesWidth`, `getBodyLength`, header pack/parse),
the `ProtocolReader` / `ProtocolWriter` little-endian and range guards, `ViPaqSerializer` round-trips and the
`compress` / `layout` options, and the interop cross-decode matrix.

The suite reads the **shared cross-language vectors** in `vipaq/test-vectors/` — the same files the C# suite reads
— so both implementations grade against one answer key and can't silently drift. The `{raw, deflate, gzip}`
cross-decode matrix is done; see [cross-language-testing.md](cross-language-testing.md).
