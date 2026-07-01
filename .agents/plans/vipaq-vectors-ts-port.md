# ViPaq shared vectors — TypeScript port plan

Status: **planned, not started.** The C# side is wired and green (see below). This is the handoff for the
TS session. Companion to [vipaq-cross-language-testing.md](vipaq-cross-language-testing.md) (master plan)
and the conventions in `vipaq/test-vectors/README.md`. Follow the **Locked TS test style** in the master
plan (mirror `src/` tree, sentence test names, `$name` rows, `// ports C#:` ties, `expectBytes`).

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

Try (A) first (matches the master plan's `resolveJsonModule` note). Fall back to (B) only if tsc/jest
complains about the out-of-project path.

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
