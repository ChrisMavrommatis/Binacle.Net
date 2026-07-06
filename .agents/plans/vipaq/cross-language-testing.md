# ViPaq — Cross-Language Wire Testing (record)

**What it guarantees:** the C# `Binacle.ViPaq` library and its hand-maintained TypeScript mirror
(`vipaq/packages/binacle-vipaq`) stay **wire-compatible** — bytes written by one are readable by the other.
The mechanism is **shared reference vectors** in `vipaq/test-vectors/`: one set of JSON inputs+answers read by
*both* test suites, so each is graded against the same answer key and the two can't silently drift.

**Status: DONE and committed.** Both suites green, `tsc` clean, full solution builds. No active work left —
this file is the durable inventory. The optional coverage at the bottom is the only open item.

---

## Inventory

### Generators (regenerate the committed vectors)
- **C#** — `vipaq/tools/Binacle.ViPaq.Generators/`: `Program.cs` (no-arg runner over a list of `IVectorGenerator`),
  `InteropArtifactGenerator.cs` (serializes each `input.json` case → `artifact-cs.json`),
  `EncodingInfoBytesGenerator.cs` (all 256 header combos → `encoding-info-bytes.json`), `CompactJson.cs` (one-object-
  per-line writer for the 256-row file), plus `IVectorGenerator.cs`, `Contracts.cs`, `RepoLocator.cs`. Geometry comes
  from the shared `Binacle.CompactNotation` + `Binacle.Geometry`; the tool only depends on the lib (+ those shared libs).
- **TS** — `vipaq/packages/binacle-vipaq/tools/`: `generateVectors.ts` (no-arg runner, mirrors `Program.cs`),
  `interopArtifactGenerator.ts` (same input → `artifact-ts.json`), `Artifact.ts` (output shape as a class). TS does
  **not** generate `encoding-info-bytes.json` — that is C#-only.

**Regenerate:** `npm run regen:interop` in `binacle-vipaq` runs the C# generator then the TS one, so the interop
halves can't drift. Output is deterministic — a no-change re-run is byte-identical (no git noise).

### Interop tests (the gzip cross-decode matrix)
Contract is **decode-to-input, never byte-equality** for compressed blobs: gzip bytes are not reproducible across
engines/runtimes. The *why* is recorded permanently in `PROTOCOL.md §6` + `test-vectors/README.md` — keep it there.
- **C#** `vipaq/test/Binacle.ViPaq.UnitTests/Tests/Interop/`:
  - `InteropDecodeTests` — decodes both `artifact-cs` and `artifact-ts` back to input; pins byte 0 first.
  - `InteropByteIdentityTests` — the **uncompressed** blob must be byte-identical across producers (the one safe byte
    comparison; language-agnostic, so it lives on one side only).
  - `InteropIntegrityTests` — each artifact file's `Name` set must equal `input.json`'s (catches a stale/forgotten regen).
- **TS** `vipaq/packages/binacle-vipaq/tests/`: `interop.test.ts` (decodes both artifacts) and
  `interopIntegrity.test.ts` (the same Name-set guard).

### Shared vectors in `vipaq/test-vectors/` — the answer key both suites read
| File | Cases | Consumed by |
|---|---|---|
| `exact-bytes.json` | 8 | serialize golden (both) + deserialize (C#) |
| `encoding-info-bytes.json` | 256 | header pack/unpack, both directions (both) |
| `little-endian/uint8..uint64.json` | 4 files | protocol reader+writer, all four widths (both) — two wide 64-bit rows are C#-only |
| `bit-size-selection.json` | 20 | both width pickers, routed by `Kind` |
| `bit-size-invalid.json` | 15 | both pickers reject; assert offending field |
| `round-trip-scenarios.json` | 10 | serialize → pin byte 0 → deserialize (both) |
| `decode-invalid.json` | 7 | deserialize must throw (both) |
| `encode-invalid.json` | 7 | serialize must throw (both) |
| `interop/input.json` | 14 | interop matrix — the shared answer key for the two artifact files |

`interop/input.json` (14 cases): the two threshold-straddling `_8_8_8` cases (one under / one over the 255-byte body
threshold), mid-bucket `16/32/64` uncompressed, a mixed-width `8_16_32`, a `Compressed_16_16_16`, six item-dim boundary
cases (255, 256, 65535, 65536, 4294967295, 4294967296) proving both languages flip width at the exact same value, and a
`MaxInteger` (`2^53-1`) case in all three sections. Each case carries `ExpectedEncodingInfo` — a spec-determined byte-0
oracle. Artifacts are `{Name, Producer, Base64}`, joined to input by `Name`.

**Invariant to protect:** the same shared scenarios are consumed on both sides. The two suites' *totals* differ (C# has
more language-local tests) and that is expected — see "Out of scope" below.

### EncodingInfoNotation
`vipaq/src/Binacle.ViPaq/EncodingInfoNotation.cs` — `internal static`. It parses/formats only the header string
(`"Version_Bin_ItemDim_ItemCoord"`, e.g. `"Uncompressed_8_8_8"`); the geometry grammar lives in the shared
`Binacle.CompactNotation`. Both members are in use via `InternalsVisibleTo`: `FormatEncodingInfo` by the C# generator,
`ParseEncodingInfo` by the test `VectorParser`.

---

## Reference docs (canonical — keep, not plans)
- `vipaq/PROTOCOL.md` — the normative spec (wire layout, `[0, 2^53−1]` range, error table, decisions log).
- `vipaq/test-vectors/README.md` — shared-vector conventions (PascalCase keys, compact strings, `Name` join key).
- `.agents/docs/vipaq/README.md`, `.agents/docs/vipaq/typescript.md` — agent notes; link to PROTOCOL.md.

---

## Optional coverage (not built — low value, add only if a real need appears)
Each is just new `input.json` rows + a regen; the matrix fans them across both suites automatically.
- Mirror the width-boundary flips in a *coordinate* (separate encoder from dims, shared picker).
- Empty items list.
- Many *distinct* items (varied dims and coords, not `:Q` repeats).
- Compressed payloads at 32/64-bit widths.

Cross-runtime coverage (foreign-runtime gzip blobs, .NET-8/9 rows) was built then dropped: a gzip decoder reads any
valid gzip, so it was low-value belt-and-suspenders and needed hand-captured Docker bytes outside the "one generator,
committed output" discipline. The finding it demonstrated is preserved in `PROTOCOL.md §6`.

## Out of scope (stays language-local on purpose)
C# generic-`T` matrices, saturation-by-type, dispose idempotency; TS buffer pre-sizing + Web-Streams gzip mechanics
(`getByteSize`, `getBufferSize`, `compressBuffer`, `getDecodingDataStream`). These are language mechanics, not wire
data. Only *shared-vector* coverage matches across suites (same JSON, same `Name`s) — totals differ by design.
