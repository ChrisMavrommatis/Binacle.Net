# ViPaq shared vectors — TypeScript port plan

## Session handoff (2026-07-02) — decisions locked, C# refactor DONE, then TS

A working session settled the open choices and **completed the C# parser refactor (suite green, 1329
passed)**. It also left **draft** TS files on disk. **The original plan below is still useful reference,
but this handoff overrides two things it left open:** the "Decision to make first" (JSON loading) chose
**file read, not `resolveJsonModule`**, and the parser is **free functions, not name-keyed
providers/dictionaries**. The C# side is now the mirror target — match it in TS.

### Decisions locked
1. **JSON loading = file read (NOT `resolveJsonModule`).** `tests/support/vectorReader.ts` exports a free
   function `readVectors<T>(fileName): T[]` = `JSON.parse(fs.readFileSync(path.join(__dirname,
   "../../../test-vectors", fileName), "utf8"))`. `@types/node` was added to `package.json` devDeps
   (`^24.0.0`). **`node_modules` is not installed** (no lockfile) — a `npm install` is required regardless,
   so the `@types/node` cost is free. Mirrors C# `VectorReader` (which reads embedded resources).
2. **Free functions over static-only classes.** `tests/support/vectorParser.ts` exports free functions,
   not a class. PascalCase filenames are reserved for files whose export is a PascalCase class. (Recorded
   as a durable user preference in agent memory.)
3. **One provider per set, exporting a parsed ARRAY.** Drop C#'s `Dictionary<string,Scenario>` + `Get(name)`
   — jest `test.each(array)("$name", row => …)` hands the whole row to the test. Tests stay pure assertions.
4. **Parser = two honest parsers, no generic "triple"/"values" parser (applies to BOTH C# and TS):**
   - `parseDimensions("LxWxH")` splits on `x`. Used for bin + item dims (C# `ParseBin` keeps its own type).
   - `parseCoordinates("X,Y,Z")` splits on `,`. Used for item coords AND bit-size `Coordinates` rows — a
     coordinate is parsed the SAME way everywhere. (C# has a private `ParseThree(compact, separator)` for the
     mechanical 3-split; there is **no** public `parseTriple`/`parseValues`.)
   - `parseItems` composes the two: split off `:Q`, split on the space, strip the parens off the coords,
     hand `"X,Y,Z"` to `parseCoordinates`.
   - **`Values` is parsed by the row's `Kind`** (decision 5) — `Dimensions` → `LxWxH`, `Coordinates` →
     `X,Y,Z`. No format ambiguity, so no generic parser is needed.
5. **Both bit-size files split into two kinds; no scenario carries both.** Each file is `{Name, Kind, Values,
   …}`; `Kind` splits its rows into a dimensions set and a coordinates set, and says how `Values` is parsed
   (`Dimensions` → `LxWxH`, `Coordinates` → `X,Y,Z`). A row is ONE kind — no scenario holds both a dimensions
   and a coordinates value (no empty placeholder, no derived twin). Selection runs each row through only its
   kind's picker; "the two pickers can't drift" holds because the two sets together cover every width bucket.

### C# refactor — DONE this session (the thing to mirror), green at 1329
- `Providers/VectorParser.cs` — `ParseTriple` deleted; `ParseCoordinates` splits on `,`; `ParseBin`/
  `ParseDimensions` split on `x` via a private `ParseThree(compact, separator)`; `ParseItemParts` uses
  `ParseDimensions` + `ParseCoordinates` (no inline comma split).
- `Providers/BitSizeKind.cs` — NEW shared `internal enum BitSizeKind { Dimensions, Coordinates }` (used only
  to split rows in the two providers; no test references it).
- `Providers/BitSizeInvalidProvider.cs` + `Providers/BitSizeSelectionProvider.cs` — each holds TWO
  dictionaries (`dimensions`, `coordinates`), a generic `public sealed record Scenario<TValue>(TValue Value,
  …)` (matching `LittleEndianProvider.Scenario<TValue>`; `.Value` is the typed triple, plus `Field` for
  invalid / `Expected` for selection), and exposes `DimensionNames`/`CoordinateNames` + `Dimension(name)`/
  `Coordinate(name)`.
- `Tests/BitSizeInvalidTests.cs` + `Tests/BitSizeSelectionTests.cs` — each now has TWO theories (one per
  kind), each running just that kind's picker on `scenario.Value`.
- **Data changed** (`vipaq/test-vectors/`): `bit-size-invalid.json` `Coordinates` rows now comma
  (`"-1,0,0"`, `"9007199254740992,0,0"`, …); `bit-size-selection.json` gained `Kind` per row (its
  `Coordinates` rows are comma). README compact-string rules + both bit-size notes updated.

### Then — TS (draft files already on disk in `vipaq/binacle-vipaq/tests/`, will break `npm test` until wired)
Drafts present: `support/vectorReader.ts`, `support/vectorParser.ts`,
`providers/{roundTripCases,decodeInvalid,bitSizeInvalid}.ts`, `roundTrip.test.ts`, `decodeInvalid.test.ts`,
`bitSizeInvalid.test.ts`, and `EXAMPLES-README.md` (index + status).
- **The draft `vectorParser.ts` still has the PRE-redesign parser** (`parseTriple`, `parseCoordinates`
  splitting on `x`). Update it to decisions 4–5: DELETE `parseTriple` (there is no `parseValues`), make
  `parseCoordinates` split on `,`, and have `parseItems` compose `parseDimensions` + `parseCoordinates`.
- **Mirror the two-kinds split** (decision 5). `providers/bitSizeInvalid.ts` and the new
  `providers/bitSizeSelection.ts` each split their file by `Kind` into a dimensions array and a coordinates
  array, parsing `Values` with `parseDimensions` or `parseCoordinates` accordingly — never both. Each
  `.test.ts` gets two `test.each` blocks (one per kind), running only that kind's picker. (`BitSizeKind` is
  just the string union `"Dimensions" | "Coordinates"` in TS.)
- Repoint the 3 existing providers to `readVectors`: `exactBytes` (3→8 cases), `encodingInfoCases`
  (128→256), `littleEndianCases` (split per width: `little-endian/uint8|16|32|64.json`; the two wide rows
  above 2^53 stay C#-only).
- Add the remaining providers + `*.test.ts` (mirror the C# classes): `bitSizeSelection`, `encodeInvalid`,
  `encodingInfoByte` (keyed by the `EncodingInfo` string; use it as the `$` title). `roundTrip`,
  `decodeInvalid`, `bitSizeInvalid` are drafted.
- `npm install`, then `npx tsc --noEmit && npm test`. Spot-check decode-invalid "truncated body" and
  encode-invalid "item count over uint16 max" reject. Delete `EXAMPLES-README.md` when green.

### Gotchas to carry (from the sections below)
- `serialize`/`deserialize` are **async** → `await expect(...).rejects.toThrow()`, never bare `toThrow()`.
- TS pickers throw the **lowercased** field in the message → assert ``toThrow(`'${field.toLowerCase()}'`)``.
- `getCoordinatesBitSize(item)` needs a full item (dims+coords) → build a probe with dims defaulted to 1.
- Use `toEqual` (not `toStrictEqual`) for items (parsed plain objects vs decoded `Item` instances).

---

Status: **planned, not started.** The C# side is wired and green (see below). This is the handoff for the
TS session. Companion to [vipaq-cross-language-testing.md](vipaq-cross-language-testing.md) (master plan)
and the conventions in `vipaq/test-vectors/README.md`. Follow the **Locked TS test style** in the master
plan (mirror `src/` tree, sentence test names, `$name` rows, `// ports C#:` ties, `expectBytes`).

**Before writing any code, surface the open choices below to the user with concrete examples and let them
pick — same discipline as Session 1's style-lock ("propose options; the user picks").** The open choices
are: (1) how TS reads the JSON — see "Decision to make first"; (2) whether each new small case set gets its
own `tests/providers/*.ts` file or is inlined in its `.test.ts` — see "Infra to add". The test *style* is
already locked (above), so it is not up for grabs; only these two are.

## What the C# side already did (the thing to mirror)

C# now reads every file in `vipaq/test-vectors/` instead of inline literals. Done in
`vipaq/test/Binacle.ViPaq.UnitTests/` — the shared files are embedded under a virtual `Data/` folder:

- `Providers/VectorReader.cs` — loads embedded JSON arrays.
- `Providers/VectorParser.cs` — parses the compact strings and byte tokens (the parser layer to mirror).
- `Providers/<Name>Provider.cs` — 8 per-file loaders that turn rows into typed scenarios keyed by `Name`
  (`EncodingInfoByteProvider` keys by the EncodingInfo string). Each has a `private const string FileName`,
  loads once in a static constructor, exposes `Names` (theory labels) + `Get(name)`, and its resolved-row
  record is `Scenario`. Each provider also **nests its own raw JSON row** as a `private sealed class Vector`
  (there is no shared models file — the shapes are 1:1 with providers). `ExactBytesProvider` also nests
  `Blob`/`Item`, with `Blob.ToByteArray()` flattening the by-segment golden; `BitSizeKind` is a public
  nested enum of `BitSizeInvalidProvider`.
- The 3 old hand-coded providers were deleted; 4 tests were repointed; 5 new test classes were added.
- Lib fix: `ProtocolReader.ReadByte` now throws `EndOfStreamException` at EOF (PROTOCOL.md §7), so a
  truncated 8-bit body is rejected. **TS already rejects this** (`DataView.getUint8` throws `RangeError`),
  so no TS source change — but the `decode-invalid` "truncated body" case must confirm it rejects.

Principle: the JSON is the single source of truth. TS parses the same files the same way, so the two
implementations can't drift. Edit a case in the JSON, never in a suite.

## Decision to make first — how TS reads the JSON

The files live **outside** `vipaq/binacle-vipaq/` (in `vipaq/test-vectors/`). Two options:

- **(A) `resolveJsonModule` + static import (recommended, zero deps).** Set `"resolveJsonModule": true`
  in `tsconfig.json` (`esModuleInterop` is already true). Each provider/test does
  `import raw from "../../test-vectors/<file>.json";` then casts `raw as unknown as <Vector>[]`. Jest reads
  `.json` natively; ts-jest transforms in memory so the out-of-rootDir path should not trip `TS6059`
  (neither `rootDir` nor `outDir` is set). **Verify** with `npx tsc --noEmit` + `npm test`.
- **(B) runtime `fs.readFileSync` loader.** A `tests/support/vectors.ts` with
  `JSON.parse(fs.readFileSync(path.join(__dirname, "../../../test-vectors", file), "utf8"))`. Robust to
  paths, but needs `@types/node` added as a devDep (for `fs`/`path`/`__dirname` under `strict`) + an
  `npm install`.

Show the user both as short runnable snippets (the `tsconfig` line + a one-line import for A; the
`vectors.ts` loader + the `@types/node` devDep for B) and let them choose. Recommend (A) — it matches the
master plan's `resolveJsonModule` note and adds no deps; fall back to (B) if tsc/jest complains about the
out-of-project path. Do not silently pick one.

## Infra to add (mirror `Providers/`)

`tests/support/vectorParser.ts` — port `VectorParser.cs` 1:1:
- `parseByte(token)` — `0x`→base16, `0b`→base2, strip `_`.
- `parseBytes(tokens)` → `number[]`.
- `parseTriple("AxBxC")` → `[a,b,c]` via `Number(...)` (allows leading `-`).
- `parseBin` / `parseDimensions` → `Dimensions`; `parseCoordinates` → `Coordinates`.
- `parseItems(string[])` → `(Dimensions & Coordinates)[]`, expanding the `:Q` suffix (split on `:` first,
  then `LxWxH (X,Y,Z)`: split on the space, strip the parens, split coords on `,`). Default Q = 1.
- `parseEncodingInfo("Version_Bin_ItemDim_ItemCoord")` → `EncodingInfo`. Version words:
  `Uncompressed | Compressed(→CompressedGzip) | Reserved2 | Reserved3`; widths `8|16|32|64`.

Per-file typed loaders: keep shared row data in `tests/providers/` (locked style). Inline the tiny new
ones in their `.test.ts` if a separate provider feels like over-indirection.

## Repoint the 3 existing providers (keep their exported shape — consumers don't change)

- `tests/providers/exactBytes.ts` → load `exact-bytes.json`. Flatten `Bytes` =
  `Header :: Count :: Bin :: (Dims :: Coords per item)`. Keep `exactBytesCases: {name, bin, items, bytes}[]`.
  Now **8** cases (adds 64-bit coord, shared-width, no-items). Consumers unchanged:
  `tests/ViPaqSerializer.test.ts` (`test.each(exactBytesCases)`, plus `exactBytesCases[0].bytes` in the
  reserved-version mutate test).
- `tests/providers/littleEndianCases.ts` → load `little-endian/uint16|32|64.json` (add `uint8` if you also
  repoint the single-byte tests, as C# did). Keep `uint16Cases/uint32Cases/uint64Cases: {name, value, bytes}[]`.
  `uint64` is now **3** rows (0, 2^32, 2^53-1) — all JS-safe, so drop the old 2-row precision note. The two
  wide rows (`0x0102030405060708`, `0xFFFF…FF`) stay **C#-only** (above 2^53, not in the shared file).
  Consumers unchanged: `tests/ProtocolReader.test.ts`, `tests/ProtocolWriter.test.ts`.
- `tests/providers/encodingInfoCases.ts` → load `encoding-info-bytes.json` (**256**). Parse the
  `EncodingInfo` string + `Byte`. Keep `EncodingInfoData.All` and the field name `binDimensionBitSize`.
  TS gains Reserved2/3 coverage (was 128 → 256; `Version` const enum already has them, and
  `encodingInfoToByte`/`FromByte` handle 2/3 numerically). Consumer unchanged: `tests/utils/encodingInfo.test.ts`.

## Add 5 new `.test.ts` (mirror the new C# classes)

Place at `tests/` root (no single `src/` file backs them, like `ViPaqSerializer.test.ts`). Tie each with
`// ports C#: <ClassName>`.

- `roundTrip` ← `RoundTripScenarioTests`. `serialize(bin, items)` → assert the 4 header fields of
  `encodingInfoFromByte(data[0])` equal `parseEncodingInfo(ExpectedEncodingInfo)` → `deserialize` →
  `expect(result.bin).toEqual(bin)`, `expect(result.items).toEqual(items)`. **async.**
- `bitSizeSelection` ← `BitSizeSelectionTests`. Build one probe `{length,width,height,x,y,z}` with
  dims **and** coords = the `Values` triple; assert `getDimensionsBitSize(probe)` and
  `getCoordinatesBitSize(probe)` both equal the expected width (map the enum name → `BitSize`).
- `bitSizeInvalid` ← `BitSizeInvalidTests`. Route by `Kind`; assert the call throws with the field
  name: `expect(call).toThrow(`'${Field.toLowerCase()}'`)` (TS picker messages read `'length'`, `'x'`, …).
- `decodeInvalid` ← `DecodeInvalidTests`.
  `await expect(deserialize(new Uint8Array(parseBytes(Blob)))).rejects.toThrow()`.
- `encodeInvalid` ← `EncodeInvalidTests`.
  `await expect(serialize(parseBin(Bin), parseItems(Items))).rejects.toThrow()`.

## Parity gotchas (the things that bite)

- `serialize`/`deserialize` are **async** → use `rejects.toThrow()`, never bare `toThrow()`.
- `getCoordinatesBitSize(item: Dimensions & Coordinates)` needs a full item → use the dims+coords probe
  (matches C# building `Dimensions<long>` + `Coordinates<long>` from the same triple).
- TS picker error messages contain the **lowercased** field in quotes (`'length'`, `'x'`); C#'s
  `ParamName` is PascalCase. The shared `Field` is PascalCase — lowercase it on the TS side only.
- `toEqual` (not `toStrictEqual`) for items: parsed cases are plain objects, decoded items are `Item`
  instances — `toEqual` ignores the prototype, matching existing usage.
- Truncated-body and over-MaxInteger decode already throw in TS; just confirm.
- Name files `*.test.ts` so jest's `**/tests/**/*.test.ts` picks them up. Providers/support are not
  `.test.ts`, so jest skips them.

## Out of scope (unchanged, see master plan / README "What is not here")

gzip cross-decode matrix, generators / `vipaq/tools/`, C#-local generic-T matrices, compressed
byte-sharing. The compressed path is still exercised only via serialize→deserialize round-trips.

## Verify

In `vipaq/binacle-vipaq`: `npx tsc --noEmit` then `npm test`. Expect green, with counts up (encoding-info
128→256, exact-bytes 3→8). Spot-check that `decode-invalid` "truncated body" and `encode-invalid`
"item count over uint16 max" both reject. Then the loop closes: any future C#↔TS wire disagreement
surfaces as a failing shared case in whichever suite is wrong.
