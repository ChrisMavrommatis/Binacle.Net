# ViPaq — Cross-Language Wire Testing (master plan)

**Goal:** guarantee the C# `Binacle.ViPaq` library and its hand-maintained TypeScript mirror
(`vipaq/binacle-vipaq`) stay **wire-compatible** — bytes written by one are readable by the other. The
mechanism is **shared reference vectors** in `vipaq/test-vectors/`: one set of JSON inputs+answers read by
*both* test suites, so each is graded against the same answer key and the two can't silently drift.

**Status (2026-07-03):** unit suites, shared vectors, and the interop matrix **green** — C# **1350**, TS **970**
(20 suites), `tsc` clean. The three-pass review's findings (F1 uint8 symmetry, F2 stale README note, F3 pin
`typescript`) are **fixed**. The gzip interop matrix is **complete** (C# + TS generators, `interop/` vectors,
both suites decode both artifacts, uncompressed byte-identity, integrity per file); the measured "compressed
bytes aren't reproducible across runtimes" finding lives permanently in `PROTOCOL.md §6` + `test-vectors/README.md`.
Cross-runtime tests were built then dropped as low-value. Both former nice-to-haves are now **done** (single
`regen` entry point, `encoding-info-bytes.json` generator), and the interop inputs are **widened** to
8/16/32/64-bit and mixed-width sections plus nonzero coordinates.

> **HANDOFF → new session.** Everything is green (C# 1350, TS 970). The interop matrix is complete and the C#
> `InteropProvider` simplification is **done** (see below). No blocking work remains — the two former nice-to-haves
> ("Remaining") are done.
>
> **Git state (verify with `git status` — do not trust this blindly):** the session up to the provider split is
> committed (`git log`: `eeccd1e6 vipaq cs ts interop`, `5bd32202 ts interop generator`, `f1334b22 interop tests
> and vipaq test reorganize`, `cfb757d3 review and fixes`). The `InteropProvider` split into `InteropVectors` +
> `CSharpArtifacts` + `TypeScriptArtifacts` may still be uncommitted — check `git status`. **Do not commit it
> yourself** (see the "Never commit" rule in `CLAUDE.md`); leave it in the working tree for the human.
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
- `vipaq/tools/Binacle.ViPaq.Generators` — C# console (`dotnet run`, no args), reads `interop/input.json`,
  writes `interop/artifact-cs.json`. Standalone: own concrete-`long` `Bin`/`Item` + compact-string parser.
  Modular: `Program` runs a list of `IVectorGenerator`s (`InteropArtifactGenerator`, `EncodingInfoBytesGenerator`).
- `binacle-vipaq/tools/generateVectors.ts` — TS runner (`npm run generate:interop`), same input →
  `artifact-ts.json`. Mirrors the C# shape but idiomatic TS: generators are plain async functions
  (`interopArtifactGenerator.ts`), run from a no-arg list; the serialized output shape is a class
  (`Artifact.ts`) so the file schema is controlled, matching the C# concrete class.
- `interop/input.json` — the shared answer key. Seven cases spanning the width buckets and compression: the
  two original threshold-straddling `_8_8_8` cases (one **under** the 255-byte body threshold, one **over** →
  `Compressed_8_8_8`), plus `Uncompressed_16_16_16` / `32_32_32` / `64_64_64` (the last within the interop
  range), a mixed-width `Uncompressed_8_16_32`, and a `Compressed_16_16_16`. Nonzero and negative-free
  coordinates are exercised. Both generators serialize all of them.
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

**Done — C# `InteropProvider` simplified (was the nested-dictionary provider):** split into a shared static
loader `InteropVectors` (holds `input.json` + `Load(fileName)` + the `Input`/`ArtifactCase` records) and two
thin static providers `CSharpArtifacts` / `TypeScriptArtifacts`, each `= InteropVectors.Load(ownFile)`. This
resolved the xUnit constraint (`[MemberData]` must be static, static classes can't derive) as "shared static
helper + two static providers," not real inheritance. The nested `Dictionary<file, Dictionary<name, case>>` and
the `(producer, name)` threading are gone — each provider *is* its file. `InteropDecodeTests` and
`InteropIntegrityTests` now have one method per producer; `InteropByteIdentityTests` is the only test that
touches both files. Matches the flat TS `InteropArtifacts.ts`. Green at 1336, no case-count change.

**Done — former nice-to-haves (2026-07-03):**
- **Single `regen` entry point**: `npm run regen:interop` (in `binacle-vipaq`) runs the C# generator
  (`dotnet run --project ../tools/Binacle.ViPaq.Generators`) then the TS one (`npm run generate:interop`), so
  the interop halves can't drift. The C# tool **takes no arguments** — it always regenerates every committed C#
  vector, so a regen can't half-run. Output is deterministic, so a no-change re-run is byte-identical.
- **`encoding-info-bytes.json` generator**: part of the same C# tool. Emits all 256 header combos off the enums
  (Version outer, item-coordinates inner; `Byte` = `Version<<6 | Bin<<4 | ItemDim<<2 | ItemCoord` as grouped
  binary). Each row is a concrete `EncodingInfoByteVector` and the whole list is serialized with
  `JsonSerializer` (tab-indented, same options as the interop file) — no hand-built strings, schema lives in
  the class. Moving off the old hand-authored format reflows the golden to standard pretty JSON, a one-time
  cosmetic reformat — tests parse JSON, so they're unaffected.

**Also done this session (2026-07-03) — review fixes + coverage:**
- Alignment fixes from the C#/TS interop review: renamed TS `expected` → `expectedEncodingInfo`; made the
  integrity check report cleanly on an *extra* artifact name (C# `InteropVectors.ReadNames` reads names without
  the input join; TS `loadInteropArtifactCases` is lazy so importing for integrity no longer runs the join);
  centralized the C# interop file names in `InteropFiles` (mirrors TS `artifactFiles`); `Producer` on the C#
  `ArtifactCase` was **kept** by request.
- Widened interop inputs (see the input.json note above) — the byte-identity test now proves C#/TS emit
  identical uncompressed bytes at every width bucket, not just `_8_8_8`.

**Still open (deferred correctness):** the compact grammar has four parser copies (test `VectorParser` +
generator `CompactParser`, in C# and TS). A cross-project parser-agreement guard was tried and reverted — it
coupled the test project to the generator, which must stay standalone. The right fix is to dedupe the parsers
(one shared source per language), after which there's nothing to guard. Until then, drift is caught only by the
interop decode tests, and only for the strings in `input.json`.

---

## Out of scope (stays language-local on purpose)
C# generic-`T` matrices, saturation-by-type, dispose idempotency; TS buffer pre-sizing + Web-Streams gzip
mechanics (`getByteSize`, `getBufferSize`, `compressBuffer`, `getDecodingDataStream`). These are language
mechanics, not wire data — see `test-vectors/README.md` "What is not here."

---

## Notes for the next reader (from the 2026-07-02 review)

- **Why the two suite totals differ (C# 1336 vs TS 960) — expected, not a gap.** Only the *shared-vector*
  coverage matches (same JSON, same `Name`s); e.g. the 256 encoding-info combos run 256×3 on **each** side. The
  difference is all language-local: C#-only test classes multiplied by the generic `T` (saturation-by-type,
  dispose, `Read8Bits` per type, the boundary matrices), coarser `test.each` granularity in TS, and the two
  wide 64-bit little-endian rows that are C#-only. The invariant to protect is "same shared scenarios on both
  sides," not "same total."
- **A known, harmless parser tolerance difference:** C# `VectorParser.ParseThree` uses `long.Parse` (throws on
  empty/garbage); TS `parseThree` uses `Number(...)` (`Number("")` is `0`, bad input is `NaN`, neither throws).
  Only matters for a *malformed* vector, of which there are none. Fix only if the vectors ever carry
  deliberately-malformed triples.
