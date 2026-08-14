---
id: vipaq
description: Binacle.ViPaq — compact binary format for packing results. The wire is defined in PROTOCOL.md; this covers the C# API surface, repo layout, and tests.
verified: 2026-08-07
check: Public API surface (ViPaqSerializer, ViPaqSerializationOptions, Layout, Limits) and repo layout match vipaq/src/Binacle.ViPaq/
also_update:
  - vipaq/typescript
  - vipaq/cross-language-testing
paths:
  - "vipaq/**"
---

# ViPaq

> **Stable as of Binacle.Net v3.0.0.** A future format change takes the next `Version` code rather than altering
> `Version = 0` — see "Room to grow" (§2.3) in `vipaq/PROTOCOL.md`.

`Binacle.ViPaq` is a compact binary format for one packing result: a single bin plus a list of placed items,
stored and moved as a short base64 token. Used in v3 and v4 API responses when `IncludeViPaqData: true`.

**The wire format is defined in `vipaq/PROTOCOL.md`, which stands alone** — everything about the bytes is there,
with no dependency on any other file. Do not restate the byte layout here; read it there. This document covers the
C# side: the public API, the repo layout, and the tests. The *why* behind the format is in
`$vipaq/architecture` and the ViPaq design records.

## C# public surface

Everything else in the library is `internal` (tests and tools reach it via `InternalsVisibleTo`).

| Type | What it is |
|---|---|
| `ViPaqSerializer` | `Serialize<TBin,TItem,T>(bin, items, Action<ViPaqSerializationOptions>?)` and `Deserialize<TBin,TItem,T>(byte[])`. The only entry point. |
| `ViPaqSerializationOptions` | `Compress` (bool, default off) and `Layout` (default `RowMajor`). Sets the header; the codec follows from it. |
| `Layout` | `RowMajor` / `Columnar` — item order in the body. Public so a caller can pick it. |
| `ViPaqBase64Extensions` | `byte[].ToBase64()` / `string.FromBase64()`. The serializer returns raw `byte[]`; base64 is not part of the format. |
| `Limits` | `MaxValue` = 65,535 (every dimension and coordinate is in `[0, 65535]`), `MaxItemCount` = 65,535. |
| `ViPaqFormatException` | Thrown on a malformed blob (bad version, a set reserved bit or width, wrong length, a body that is not a valid DEFLATE stream). |
| `ICompressionCodec` | The codec seam. `DeflateCodec` / `GzipCodec` / `NoOpCodec` are `internal`; only the internal `ProtocolEncoder` takes one. |

The generic constraints use the shared **`Binacle.Geometry`** interfaces (`IWith[ReadOnly]Dimensions<T>` /
`IWith[ReadOnly]Coordinates<T>`, `where T : struct, IBinaryInteger<T>`) — ViPaq defines no `IWith*` family of its
own. `Serialize` takes read-only items (it only reads them); `Deserialize` needs the settable interfaces and
`new()`. The ready-made concrete types `Dimensions<T>` / `Item<T>` live in `Binacle.Geometry`
(`shared/src/Binacle.Geometry/Models/`), not here; any type that implements the leaf interfaces — for example the
v4 `PackedBox` — can be serialized directly.

**Compression** is off by default and is a straight on/off (no "keep the shorter" try-both in the serializer). The
codec is **resolved from the header** — raw DEFLATE when `Compress` is set, a pass-through `NoOpCodec` when not.

### Header notation (internal)

`HeaderNotation` (internal) is the header's text form for the test vectors:
`v{N}_{raw|comp}_{row|col}_{binW}_{itemDimW}_{itemCoordW}` — e.g. `v1_comp_col_16_8_16`. The geometry text
notation (`"10x10x10 (0,0,0)"`) is not here; it lives in the shared `Binacle.CompactNotation`. TS mirror:
`src/headerNotation.ts`.

## Repo layout

| Path | What it is |
|---|---|
| `vipaq/PROTOCOL.md` | Normative, standalone wire spec |
| `vipaq/src/Binacle.ViPaq/` | C# reference implementation |
| `vipaq/packages/binacle-vipaq/` | TypeScript mirror (`$vipaq/typescript`) |
| `vipaq/test-vectors/` | Language-neutral vectors read by both suites |
| `vipaq/test/` | C# unit tests, benchmarks, performance tests |

## Tests

| Project | Covers |
|---|---|
| `vipaq/test/Binacle.ViPaq.UnitTests` | serializer round-trips, exact-byte golden vectors, the forced width/layout/compression matrix, every rejection; internal `Header` / `ProtocolEncoder` / codecs via `InternalsVisibleTo` |
| `vipaq/test/Binacle.ViPaq.PerformanceTests` | packed-data conformance gate (`RoundTripCheck`) — all 716 real packs × every codec × both layouts × natural/forced-16-bit widths, header + decode-to-input, run before the size reports |
| `vipaq/packages/binacle-vipaq` | TypeScript mirror — `just test vipaq-ts-unit` (jest) |

How the two languages are held to one wire — the shared vectors, the generators, and the decode-to-input contract
for compressed payloads — is in `$vipaq/cross-language-testing`.

How the projects reference each other, who can see internals, and the walls between them (UnitTests never touches
the real-data kernel) are in `$vipaq/dependencies`.
