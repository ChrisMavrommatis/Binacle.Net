# ViPaq — Cross-Language Wire Testing (master plan)

**Goal:** guarantee the C# `Binacle.ViPaq` library and its hand-maintained TypeScript mirror
(`vipaq/binacle-vipaq`) stay **wire-compatible** — bytes written by one are readable by the other. The
mechanism is **shared reference vectors** in `vipaq/test-vectors/`: one set of JSON inputs+answers read by
*both* test suites, so each is graded against the same answer key and the two can't silently drift.

**Status (2026-07-02):** unit suites, shared vectors, and the interop matrix **green** — C# **1336**, TS **960**
(20 suites), `tsc` clean. The three-pass review's findings (F1 uint8 symmetry, F2 stale README note, F3 pin
`typescript`) are **fixed**. The gzip interop matrix is **complete** (C# + TS generators, `interop/` vectors,
both suites decode both artifacts, uncompressed byte-identity, integrity per file); the measured "compressed
bytes aren't reproducible across runtimes" finding lives permanently in `PROTOCOL.md §6` + `test-vectors/README.md`.
Cross-runtime tests were built then dropped as low-value.

> **HANDOFF → new session (near session limit 2026-07-02).** Everything above is green (C# 1336, TS 960).
> **Start here:** simplify C# `InteropProvider` per the design in "Remaining" below (abstract base reads a file
> name; `CSharp` + `TypeScript` providers derive; decode/integrity use them separately; byte-identity compares
> the two).
>
> **Git state (verify with `git status` — do not trust this blindly):** most of the session is **already
> committed** (`git log`: `cfb757d3 review and fixes`, `f1334b22 interop tests and vipaq test reorganize`,
> `5bd32202 ts interop generator` — the folderization, both generators, and `interop/` vectors). **Still
> uncommitted:** the interop **matrix** expansion (both suites decode both artifacts + byte-identity + per-file
> integrity — files `interop.test.ts`, `interopIntegrity.test.ts`, `InteropArtifacts.ts`, `InteropByteIdentity
> Tests.cs`, and edits to `InteropProvider.cs` / `InteropDecodeTests.cs` / `InteropIntegrityTests.cs`) plus the
> cross-runtime-finding **doc updates** (`PROTOCOL.md §6`, `test-vectors/README.md`, this plan). Commit those.
> The dropped cross-runtime files were never committed, so they leave no trace.
>
> **Run checks:** C# `dotnet test` in `vipaq/test/Binacle.ViPaq.UnitTests`; TS `npx tsc --noEmit` + `npx jest` in
> `vipaq/binacle-vipaq`.

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
(C# has more language-local tests) and that is expected — see "Notes for the next reader" at the bottom.

---

## Interop — the gzip cross-decode matrix

The requirement: **a payload serialized by one side deserializes on the other**, *including compressed payloads*.
Uncompressed bytes are byte-identical across languages (spec-determined, no engine). Compressed bytes are the hard
case; the contract is **decode-to-input, never byte-equality**. The *why* — compressed gzip bytes are not
reproducible across engines/runtimes (the measured .NET-8/10/Node-vs-.NET-9 finding) — is recorded permanently in
`vipaq/PROTOCOL.md §6` and `test-vectors/README.md`. **Keep it there; it must outlive this plan.**

**Built (generate once, commit, consume read-only — no CI, no run-time serialization):**
- `vipaq/tools/Binacle.ViPaq.Generators` — C# console (`dotnet run`), reads `interop/input.json`, writes
  `interop/artifact-cs.json`. Standalone: own concrete-`long` `Bin`/`Item` + compact-string parser.
- `binacle-vipaq/tools/generateArtifact.ts` — TS script (`npm run generate:interop`), same input → `artifact-ts.json`.
- `interop/input.json` — the shared answer key. Two cases: one **under** the 255-byte body threshold (stays
  `Uncompressed_8_8_8`), one **over** (~60 items → `Compressed_8_8_8`). Both generators serialize these.
- **The matrix** — both suites decode **both** artifacts back to input (byte 0 pinned):
  - C# `InteropDecodeTests` — decodes `{artifact-cs, artifact-ts}` × both scenarios.
  - TS `interop.test.ts` — decodes `{artifact-cs, artifact-ts}` × both scenarios (its own output AND C#'s).
  - C# `InteropByteIdentityTests` — the **uncompressed** blob must be byte-identical across producers (the one
    safe byte comparison; compressed is decode-to-input only). Language-agnostic, so it lives on one side.
  - Integrity per file: C# `InteropIntegrityTests` + TS `interopIntegrity.test.ts` — each artifact file's `Name`
    set must equal `input.json`'s (guards a stale/forgotten regen; proven to bite).

Cross-runtime coverage was built then **dropped** (2026-07-02): a gzip decoder reads any valid gzip, so decoding a
foreign-runtime blob is low-value belt-and-suspenders, and the .NET-8/9 rows needed hand-captured Docker bytes —
outside the "one generator, committed output" discipline. The finding it demonstrated is preserved in the docs
above. Revisit only if a real need appears.

**Remaining (nice-to-have, not blocking):**
- **Simplify C# `InteropProvider`** (fresh session). It is doing too much — loads input.json + both artifact
  files and serves three checks (decode, integrity, byte-identity) off a nested `Dictionary<file, Dictionary<
  name, case>>`. Left green and untouched on purpose — do not refactor piecemeal; do it deliberately.
  **Intended design:** an abstract base that reads a **given file name** (all the file-read + parse code lives
  there, once), with a `CSharp` and a `TypeScript` provider deriving from it, each pointing the base at its own
  artifact file. Each derived provider is then self-contained: `InteropDecodeTests` and `InteropIntegrityTests`
  consume whichever they need, separately; `InteropByteIdentityTests` compares the two (the only place that needs
  both). This drops the nested dictionary — each provider *is* its file. `input.json` is shared, so the base (or
  a small shared helper) loads it once. The already-flat TS `InteropArtifacts.ts` is the reference shape.
  - *One thing to square away:* xUnit `[MemberData]` sources must be **static**, and C# static classes can't
    derive. So "abstract base + derived providers" likely means a base with a static `Read(fileName)` helper and
    two thin static `CSharpArtifacts` / `TypeScriptArtifacts` providers that call it — or instance providers fed
    to tests another way. Decide the mechanism first.
- **One `regen` entry point** wiring the two generators (C# `dotnet run` + TS `npm run generate:interop`) so they
  can't drift. Today they're two separate hand-run commands.
- `encoding-info-bytes.json` is marked "generated" but still has no generator — build one in the C# tool when
  convenient, off the shared inputs.

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
