# ViPaq — Cross-Language Wire Testing (master plan)

**Goal:** guarantee the C# `Binacle.ViPaq` library and its hand-maintained TypeScript mirror
(`vipaq/binacle-vipaq`) stay **wire-compatible** — bytes written by one are readable by the other. The
mechanism is **shared reference vectors** in `vipaq/test-vectors/`: one set of JSON inputs+answers read by
*both* test suites, so each is graded against the same answer key and the two can't silently drift.

**Status (2026-07-02):** unit suites and shared vectors **DONE and green** — C# **1329**, TS **954** (18
suites), `tsc` clean. The three-pass review passed; its actionable findings (F1 uint8 symmetry, F2 stale
README note, F3 pin `typescript`) are **fixed** — folded into "Done" below. See
[vipaq-review-findings.md](vipaq-review-findings.md) for the one remaining note-only item. **The one remaining
build goal is the gzip cross-decode matrix.**

Reference docs (canonical, keep these — not plans):
- `vipaq/PROTOCOL.md` — the normative, language-agnostic spec (wire layout, `[0, 2^53−1]` integer range,
  error table, decisions log). The thing both implementations conform to.
- `vipaq/test-vectors/README.md` — the shared-vector conventions (PascalCase keys, compact strings, `Name`
  join key, byte notation).
- `.agents/docs/vipaq/README.md`, `.agents/docs/vipaq/typescript.md` — agent notes; link to PROTOCOL.md.

---

## Done — do not redo

### Integer-range spec + conformance (was `vipaq-integer-range-spec.md`)
ViPaq's interoperable integer range is **`[0, 2^53 − 1]` (`MaxInteger`)** — the largest integer both C# and JS
hold exactly. Both sides reject outside it, **on encode and on decode**. Landed:
- `vipaq/PROTOCOL.md` written (§5 range, §8 error table, §10 decisions log — the durable record).
- C#: `ViPaqLimits.cs` (spec constants incl. `CompressionThresholdBytes`, kept separate from `EightBitsMax`);
  `BitSizeHelper` caps the 64-bit bucket at `MaxInteger` with per-field throws; `ProtocolReaderExtensions`
  guards the decode path; serialize uses the compression-threshold constant.
- TS: `sizes.ts` renamed to the same spec constants (`eightBitsMax`/`sixteenBitsMax`/`thirtyTwoBitsMax`/
  `maxInteger`/`compressionThresholdBytes`); write primitives range-check; `read64Bits` guards the ceiling.
- All prototyping bugs (#1–#7) fixed in-tree; the compression-threshold off-by-one (#5) is fixed — **both**
  sides now compress when the **body** (excluding the header byte) is `> 255`.

### C# unit suite (canonical)
Fully restructured to theory+provider, or one-helper+thin-callers where the generic `T` differs per row. Reads
every `test-vectors/*.json` via embedded resources (`VectorReader` + `VectorParser`). Language-local matrices
(generic-`T` dispatch, saturation-by-type, dispose idempotency, `Read8Bits` per type) stay C#-only on purpose.

### TS mirror + shared-vector wiring (was `vipaq-vectors-ts-port.md`)
TS reads the **same** JSON files from disk (`tests/support/vectorReader.ts` via `fs`, **not**
`resolveJsonModule`) and parses them with the **same** grammar (`tests/support/vectorParser/` — free
functions, one per file + `index.ts` barrel, mirroring the C# `VectorParser`). Providers are 1:1 with the C#
ones. Locked design decisions (still binding):
1. **JSON loading = file read** (`readVectors<T>(fileName)`), mirroring C# `VectorReader`.
2. **Free functions over static-only classes**; PascalCase filename only when the file exports a PascalCase type.
3. **One provider per set, exporting a parsed array** — jest `test.each(array)("$name", …)`; tests are pure asserts.
4. **Two honest parsers, no generic "triple":** `parseDimensions`/`parseBin` split `x`; `parseCoordinates`
   splits `,`; `parseItems` composes them and expands `:Q`. A coordinate is comma-separated everywhere.
5. **Bit-size files split by `Kind`** into a dimensions set and a coordinates set; **each row runs through only
   its own picker.** The two sets together cover every width bucket — that is what pins the pickers together.

### Shared vectors in `vipaq/test-vectors/` — the answer key both suites read
| File | Consumed by |
|---|---|
| `exact-bytes.json` (8) | serialize golden (both) + deserialize (C#) |
| `encoding-info-bytes.json` (256) | header pack/unpack, both directions (both) |
| `little-endian/uint8..uint64.json` | protocol reader+writer, all four widths (both) — the two wide 64-bit rows stay C#-only |
| `bit-size-selection.json` (20) | both width pickers, routed by `Kind` |
| `bit-size-invalid.json` (15) | both pickers reject; assert offending field |
| `round-trip-scenarios.json` (10) | serialize → pin byte 0 → deserialize (both) |
| `decode-invalid.json` (7) | deserialize must throw (both) |
| `encode-invalid.json` (7) | serialize must throw (both) |

**Invariant to protect:** the same shared scenarios are consumed on both sides. The two suites' *totals* differ
(C# has more language-local tests) and that is expected — see the note in the findings file.

---

## Remaining — the gzip cross-decode matrix (the one real TODO)

The real requirement: **a payload serialized by one side deserializes on the other**, *including compressed
payloads*. Uncompressed bytes are already covered by the exact-byte goldens (identical across languages).
Compressed bytes are **not** byte-identical — `GZipStream` (C#) and `CompressionStream('gzip')` (Node) emit
different valid gzip for the same input. So the compressed contract is **decode-to-input, never byte-equality.**

**The 2×2 matrix** (same input, compressed once per language → two artifacts):

|                                | decode in **C#** | decode in **TS** |
|--------------------------------|------------------|------------------|
| artifact compressed **by C#**  | own round-trip   | C# → TS interop  |
| artifact compressed **by TS**  | TS → C# interop  | own round-trip   |

- All four cells must recover the **identical original input**.
- Assert `artifact-cs != artifact-ts` (documents that the encoders differ on the wire); never use byte-equality
  *between* artifacts as a pass condition.
- Pick an input comfortably over the threshold so both definitely compress (~60 small items), off the 255 edge.

**Generate once, commit, consume read-only** (the TestsKernel pattern — no CI, no run-time serialization):
- Suggested home: `vipaq/test-vectors/compressed/` — a shared input definition + `artifact-cs` + `artifact-ts`.
- A C# generator writes `artifact-cs`; a TS generator writes `artifact-ts`; both read the shared input. Wire to
  **one** `regen` command so the two producers can't drift. Commit the output.
- Each suite's cross-decode test reads **both** artifacts (its own runtime only) and runs the matrix.

**Generators / `vipaq/tools/`:** none exist yet. `encoding-info-bytes.json` is marked "generated" but nothing
regenerates it — build the canonical C# generator when you build the compressed pipeline, off the shared inputs.

**Regeneration discipline:** compressed artifacts must **never** be byte-compared across regenerations — deflate
output depends on the zlib/engine version. The only stable contract is decode-to-input. Regenerating produces
*different valid bytes*; that is expected, not a failure.

**Prove the pipeline on one case end-to-end before scaling** — one input → both generators → both suites decode
it → matrix passes → *then* fill in the case list.

---

## Out of scope (stays language-local on purpose)
C# generic-`T` matrices, saturation-by-type, dispose idempotency; TS buffer pre-sizing + Web-Streams gzip
mechanics (`getByteSize`, `getBufferSize`, `compressBuffer`, `getDecodingDataStream`). These are language
mechanics, not wire data — see `test-vectors/README.md` "What is not here."

---

## Notes for the next reader (from the 2026-07-02 review)

- **Why the two suite totals differ (C# 1329 vs TS 954) — expected, not a gap.** Only the *shared-vector*
  coverage matches (same JSON, same `Name`s); e.g. the 256 encoding-info combos run 256×3 on **each** side. The
  difference is all language-local: C#-only test classes multiplied by the generic `T` (saturation-by-type,
  dispose, `Read8Bits` per type, the boundary matrices), coarser `test.each` granularity in TS, and the two
  wide 64-bit little-endian rows that are C#-only. The invariant to protect is "same shared scenarios on both
  sides," not "same total."
- **A known, harmless parser tolerance difference:** C# `VectorParser.ParseThree` uses `long.Parse` (throws on
  empty/garbage); TS `parseThree` uses `Number(...)` (`Number("")` is `0`, bad input is `NaN`, neither throws).
  Only matters for a *malformed* vector, of which there are none. Fix only if the vectors ever carry
  deliberately-malformed triples.
