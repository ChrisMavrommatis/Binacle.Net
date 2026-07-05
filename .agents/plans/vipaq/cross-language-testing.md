# ViPaq — Cross-Language Wire Testing (master plan)

**Goal:** guarantee the C# `Binacle.ViPaq` library and its hand-maintained TypeScript mirror
(`vipaq/binacle-vipaq`) stay **wire-compatible** — bytes written by one are readable by the other. The
mechanism is **shared reference vectors** in `vipaq/test-vectors/`: one set of JSON inputs+answers read by
*both* test suites, so each is graded against the same answer key and the two can't silently drift.

**Status (2026-07-05):** all green — C# **1371**, TS **984** (20 suites), `tsc` clean, and the **full solution
builds** (`dotnet build Binacle.Net.slnx`). Since 07-03 the compact-notation extraction (companion plan) and the
`41b0fcef` "vipaq cleaning" pass have landed and are **committed**: geometry notation moved to the shared
`Binacle.CompactNotation` lib + `packages/binacle-compact-notation`, vipaq's own renamed to `EncodingInfoNotation`
and made `internal`, `Dimensions`/`Coordinates` models moved to the test project, and the `[Experimental]` gate
dropped. See `.agents/plans/compact-notation-extraction.md` for that work — this plan covers only the wire-testing
matrix, which is unchanged.
> **Note (stale below):** the handoff and "open decisions" that follow were written 07-03, before the extraction
> landed. Most are now answered — see the inline strike-throughs. Kept, not consolidated, per request. The interop matrix is complete (C# + TS generators, `interop/`
vectors, both suites decode both artifacts, uncompressed byte-identity, integrity per file); the "compressed
bytes aren't reproducible across runtimes" finding lives permanently in `PROTOCOL.md §6` +
`test-vectors/README.md`. Interop inputs are widened to every width bucket + boundaries + `MaxInteger`;
`ExpectedEncodingInfo` moved onto the input (a real byte-0 oracle); generators are modular/no-arg; and the
compact grammar + **all** model types are consolidated into the library as `CompactNotation` +
`Bin`/`Item`/`Dimensions`/`Coordinates` — one grammar per language, not four.

> **HANDOFF → reviewer + close-out.** The work is **done and green** (C# 1371, TS 984, `tsc` clean, full solution
> builds). Two things remain, both for the **next person, not this session**:
>
> **1. Review the branch.** ⚠️ Updated 07-05: the vipaq changes are now committed through `41b0fcef "vipaq
> cleaning"` (not `69429ffd` — four more commits landed: `905766b7`, `f2f4275d`, `319f5ec6`, `630242ee`,
> `41b0fcef`). Working tree is otherwise clean. Run a review over the branch — `/code-review`, or `/code-review
> ultra` for the deep cloud pass. Focus areas:
> - **new public lib surface**: `CompactNotation` (`[Experimental("BINACLE_VIPAQ_COMPACT")]`) + concrete models
>   `Bin<T>`/`Item<T>`/`Dimensions<T>`/`Coordinates<T>` — API shape, naming, generic constraints, range-lenient
>   parse vs `Serialize` validation;
> - **consolidation**: generator + test both delegate to the lib; test `Models` namespace deleted (25 usings
>   stripped); TS `src/compactNotation.ts` mirror; generator still standalone (lib-only dep);
> - **interop coverage + oracle**: the boundary / `MaxInteger` `input.json` rows and `ExpectedEncodingInfo` on the
>   input;
> - **vector reader**: the slash-path change (`/`→`.` for embedded resources).
>
> **2. Open decisions** — ⚠️ mostly CLOSED by the 07-05 extraction + `41b0fcef` cleaning:
> - ~~**`Bin<T>` vs `Dimensions<T>`**~~ — CLOSED. Shared lib dropped `Bin<T>` (one geometry model); vipaq keeps
>   `Bin<T>`/`Item<T>` public (the DTOs the serializer accepts) and moved `Dimensions<T>`/`Coordinates<T>` to the
>   test project.
> - ~~**Experimental marking**~~ — CLOSED. `[Experimental("BINACLE_VIPAQ_COMPACT")]` dropped entirely
>   (`EncodingInfoNotation` is `internal` now, so the preview gate bought nothing).
> - **Unused `Format*`** — STILL OPEN. `FormatDimensions`/`FormatItem`/`FormatCoordinates` in the shared lib have
>   no in-repo caller yet. Phase 3 (lib/API log) is meant to be their first consumer — keep, or trim.
> - **Optional interop coverage** (low value, STILL OPEN): coordinate-boundary mirror, empty items, many distinct
>   items, compressed at 32/64-bit — each is just `input.json` rows + a regen.
> - ~~**Stale doc nit**~~ — the vipaq README/`typescript.md` were refreshed in `41b0fcef` (`verified: 2026-07-05`).
>
> **Never commit** (CLAUDE.md) — leave the working-tree changes for the human.
>
> **Run checks:** C# `dotnet test` in `vipaq/test/Binacle.ViPaq.UnitTests`; TS `npx tsc --noEmit` + `npx jest` in
> `vipaq/binacle-vipaq`; full build `dotnet build Binacle.Net.slnx`.

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
- `interop/input.json` — the shared answer key. Fourteen cases spanning the width buckets, compression, and the
  bucket edges: the two original threshold-straddling `_8_8_8` cases (one **under** the 255-byte body threshold,
  one **over** → `Compressed_8_8_8`), `Uncompressed_16_16_16` / `32_32_32` / `64_64_64` (mid-bucket), a
  mixed-width `Uncompressed_8_16_32`, a `Compressed_16_16_16`, six **boundary** cases isolating the flip in the
  item-dimension section (`255`→8-bit, `256`→16-bit, `65535`→16-bit, `65536`→32-bit, `4294967295`→32-bit,
  `4294967296`→64-bit), and a **MaxInteger** case with `2^53-1` in all three sections (`Uncompressed_64_64_64`).
  Together they prove decode works on 8/16/32/64-bit up to the ceiling and that both languages flip width at the
  exact same value. Both generators serialize all of them.
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
  binary). Each row is a concrete `EncodingInfoByteVector`, so the schema lives in the class.

**Output format — one object per line, encoding-info only (decided 2026-07-03):** `encoding-info-bytes.json` is
written **one JSON object per line** (not `WriteIndented`, which spreads each object across four lines, and not
a single-line array). Rationale: a 256-row combinatorial file is only readable and greppable one-per-line. A
serializer has no "compact but per-line" mode, so we add a tiny **compact serializer** — serialize each concrete
row with `WriteIndented = false`, then join the rows with `,\n` inside `[\n … \n]\n`. No StringBuilder; the row
class still owns the schema. The space after the colon (`"Byte": "…"`) is **not** reproduced — we don't care;
one-per-line is the only requirement. Lives in C# only (`CompactJson`) because only C# writes this file.
**The interop artifacts (`artifact-cs.json` / `artifact-ts.json`) stay expanded** (`WriteIndented`, tabs,
`UnsafeRelaxedJsonEscaping` for base64) — they're 7 short rows, so per-line buys nothing there. Tests parse
JSON, so format is free to differ per file.

**Also done this session (2026-07-03) — review fixes + coverage:**
- Alignment fixes from the C#/TS interop review: renamed TS `expected` → `expectedEncodingInfo`; made the
  integrity check report cleanly on an *extra* artifact name (C# `InteropVectors.ReadNames` reads names without
  the input join; TS `loadInteropArtifactCases` is lazy so importing for integrity no longer runs the join);
  centralized the C# interop file names in `InteropFiles` (mirrors TS `artifactFiles`); `Producer` on the C#
  `ArtifactCase` was **kept** by request.
- Widened interop inputs (see the input.json note above) — the byte-identity test now proves C#/TS emit
  identical uncompressed bytes at every width bucket, not just `_8_8_8`.
- **`ExpectedEncodingInfo` moved onto `input.json`** (was derived and stored per-artifact). It's
  producer-independent and spec-determined, so declaring it with the scenario makes the byte-0 pin a real
  oracle — it now catches a generator that picks a consistently-wrong width, which the old derived-from-output
  value could not. Artifacts slimmed to `{Name, Producer, Base64}`; providers read the expected header from the
  input join. Matches the `round-trip-scenarios.json` convention.

**Next up (agreed 2026-07-03):**
- **Compact per-line serializer** — **done.** C# `CompactJson` writes `encoding-info-bytes.json` one object per
  line; the interop artifacts stay expanded (see the "Output format" decision above).
- **More interop coverage** — width boundaries and `MaxInteger` are **done** (six item-dim boundary cases +
  a `2^53-1`-in-all-sections case; see the input.json note). The byte-identity test confirms C#/TS emit
  identical uncompressed bytes at every bucket including the `MaxInteger` 64-bit blob. Still optional, lower
  value: (1) mirror the boundary flips in a *coordinate* (separate encoder from dims, shared picker), (2) empty
  items list, (3) many *distinct* items (varied dims and coords, not `:Q` repeats), (4) compressed at 32/64-bit
  widths. All are just new `input.json` rows + a regen; the matrix fans them across both suites automatically.

**Done — parser dedup via a library feature (2026-07-03).** The four compact-grammar copies (C#/TS × test/
generator) collapsed into **one grammar per language, in the library**: `CompactNotation` (C# `[Experimental
("BINACLE_VIPAQ_COMPACT")]`; TS `src/compactNotation.ts`) with `ParseBin` / `ParseDimensions` /
`ParseCoordinates` / `ParseItem` / `ParseItems` / `ParseEncodingInfo` and the matching `Format*`. The lib also
**absorbed every model type** — it had only the `IWith*` interfaces before; it now ships concrete `Bin<T>`,
`Item<T>`, `Dimensions<T>`, `Coordinates<T>` (+ `Dimensions.Create` / `Coordinates.Create` factories), so parse
has something to return. Result:
- C# generator uses `CompactNotation` + lib models; its `CompactParser`/`Models`/`EncodingInfoExtensions` are
  deleted. Still standalone — it only depends on the lib.
- C# test `VectorParser` is now just the byte-token parsers (`ParseByte`/`ParseBytes`) plus one-line geometry
  delegations. The whole `Binacle.ViPaq.UnitTests.Models` namespace is **deleted** (all four types moved to the
  lib) and the dead `using` was stripped from 25 files.
- TS mirrors both: generator and `tests/support/vectorParser` re-export from `src/compactNotation`; the TS
  `compactParser.ts` / `encodingInfoLabel.ts` and the `parseBin`/`parseItems`/`parseEncodingInfo`/
  `parseDimensions`/`parseCoordinates`/`parseThree` support files are deleted.
Both generator and test projects opt into the experimental API via `<NoWarn>BINACLE_VIPAQ_COMPACT</NoWarn>`.
Green at C# 1371 / TS 984; regen output byte-identical. One grammar and one set of models to change, not four —
nothing left to guard. **Open shape question:** `Bin<T>` and `Dimensions<T>` are identical in shape (distinct
roles) — collapse into one type later if desired.

**Done — vector reader tidy (2026-07-03).** C# `VectorReader.Read` now takes the on-disk **slash** path
(`"little-endian/uint8.json"`), the same string TS `readVectors` takes — it translates `/`→`.` internally for
the embedded-resource name. Removed the dotted call-sites (`InteropFiles`, `LittleEndianProvider`); cached the
assembly; inlined the `Data.` prefix. Both readers now take identical arguments. (C# still reads via embedded
resources vs TS `fs` — mechanism left as-is; only the call syntax was aligned.)

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
