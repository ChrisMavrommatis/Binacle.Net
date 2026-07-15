---
id: vipaq/cross-language-testing
description: ViPaq cross-language wire testing — the C#/TS shared-vector apparatus, its inventory, and the decode-to-input contract
verified: 2026-07-14
check: Vector file list, generator paths, and interop test names match vipaq/test-vectors/ and the two suites
also_update:
  - vipaq/typescript
---

# ViPaq — Cross-Language Wire Testing

**What it guarantees:** the C# `Binacle.ViPaq` library and its hand-maintained TypeScript mirror
(`vipaq/packages/binacle-vipaq`) stay **wire-compatible** — bytes written by one are readable by the other. The
mechanism is **shared reference vectors** in `vipaq/test-vectors/`: one set of JSON inputs+answers read by *both*
test suites, so each is graded against the same answer key and the two can't silently drift.

This is the durable inventory of the apparatus — a doc, not a plan. The full per-file shapes and conventions live
in [`vipaq/test-vectors/README.md`](../../../vipaq/test-vectors/README.md); this summarizes the moving parts.

## Vectors

`vipaq/test-vectors/`, grouped by area (see that folder's README for each shape):

- `serialization/` — `exact-bytes`, `round-trip-scenarios`, `decode-invalid`, `encode-invalid`.
- `header/header-bytes.json` — the 32 valid header combos as `{Header notation, two bytes}`.
- `width/` — `width-selection`, `width-invalid` (the two width pickers, `Eight`/`Sixteen` only).
- `protocol/little-endian/` — `uint8`, `uint16` (the reader/writer, the only two widths).
- `interop/` — `input.json` plus `{cs,ts}/{raw,deflate,gzip}.json`.

Everything names a full header with the **HeaderNotation** string `v{N}_{raw|comp}_{row|col}_{binW}_{itemDimW}_{itemCoordW}`
(e.g. `v1_comp_col_16_8_16`), so a round-trip vector pins the layout and all three widths, not just the geometry.

## Generators (regenerate the derived vectors)

Only two kinds of vector are generated — `header/header-bytes.json` and the interop artifacts
(`$vipaq#D15`); every other file is hand-authored.

- **C#** — `vipaq/tools/Binacle.ViPaq.VectorGenerators/`: `Program.cs` (no-arg runner over `IVectorGenerator`),
  `HeaderBytesGenerator.cs` (the 32 header combos), `InteropArtifactGenerator.cs` (serializes each `input.json`
  case under each codec → `interop/cs/{raw,deflate,gzip}.json`), `CompactJson.cs` (one-object-per-line writer),
  plus `IVectorGenerator.cs` / `Contracts.cs`. Geometry comes from the shared `Binacle.CompactNotation` +
  `Binacle.Geometry`.
- **TS** — `vipaq/packages/binacle-vipaq/tools/`: `generateVectors.ts` (mirrors `Program.cs`),
  `interopArtifactGenerator.ts` (same inputs → `interop/ts/{raw,deflate,gzip}.json`), `Artifact.ts`.

Run the C# generator then the TS one so the two interop halves can't drift. Output is deterministic — a no-change
re-run is byte-identical (no git noise).

## Interop tests — the cross-decode matrix

The contract is **decode-to-input, never byte-equality** for compressed blobs: the same body, the same codec, and
two different compressor engines (`DeflateStream` vs `CompressionStream('deflate-raw')`) can each emit a different
valid DEFLATE stream. Raw artifacts are byte-identical and can be compared directly. The *why* is normative in
`PROTOCOL.md §6.1` — keep it there.

- **C#** `vipaq/test/Binacle.ViPaq.UnitTests/Tests/Interop/`:
  - `InteropDecodeTests` — decodes each language's `{raw,deflate,gzip}` artifacts back to the `input.json` case.
  - `InteropIntegrityTests` — each artifact file's `Name` set must equal `input.json`'s (catches a stale regen).
- **TS** `vipaq/packages/binacle-vipaq/tests/`: `interop.test.ts` (decodes the artifacts) and
  `interopIntegrity.test.ts` (the same `Name`-set guard).

## Reference docs (canonical)

- `vipaq/PROTOCOL.md` — the normative, standalone spec.
- [`vipaq/test-vectors/README.md`](../../../vipaq/test-vectors/README.md) — shared-vector conventions and per-file shapes.
- `$vipaq`, `$vipaq/typescript` — the C# and TS sides.

## Out of scope (stays language-local on purpose)

C# generic-`T` matrices and typed exceptions; TS buffer pre-sizing and Web-Streams codec mechanics. These are
language mechanics, not wire data. Only shared-vector coverage matches across suites (same JSON, same `Name`s) —
totals differ by design.
