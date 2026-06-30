# ViPaq shared test vectors

Language-neutral test data read by **both** ViPaq implementations:

- C# — `vipaq/test/Binacle.ViPaq.UnitTests`
- TS — `vipaq/binacle-vipaq`

One file, two consumers. The same inputs are graded against the same answers, so the two
implementations can't silently drift on the wire format. These files are the single source of truth;
edit a case here, not in either suite.

## Rules every file follows

- **PascalCase keys** — `Name`, `Bin`, `Items`, `Bytes`, etc. (so C# binds with no case-insensitive
  option; reads fine in TS too).
- **Unique `Name` per scenario.** It is the join key across languages and the test label on each side:
  C# passes the `Name` and resolves the row from a name-keyed dictionary (so failures read
  `one 8-bit item`, not a dump of every argument); TS uses jest `test.each(...)("$Name", ...)`.
- **Compact strings for inputs** (same convention as `shared/Binacle.TestsKernel`):
  - **Dimensions / bin** — `"LxWxH"`, e.g. `"10x20x30"` (split on `x` → 3 ints).
  - **Item** — `"LxWxH (X,Y,Z)"`, e.g. `"1x2x3 (4,5,6)"` (dims, space, then `(x,y,z)`).
  - **Values triple** — `"AxBxC"` (three plain ints; negatives allowed where a case expects a throw).
- **Bytes are strings.** The encoding header byte is grouped **binary** so the flags are legible
  (`"0b00_00_00_00"` = `Version | Bin | ItemDim | ItemCoord`); every other byte is **hex**
  (`"0x0A"`). Parser rule: `0b` → base 2, `0x` → base 16, strip `_`; each entry is one byte.
- **Enum names, not numbers** — `Version` is `Uncompressed | CompressedGzip | Reserved2 | Reserved3`;
  bit sizes are `Eight | Sixteen | ThirtyTwo | SixtyFour`. Both languages already have these names.

## Integer range

Every value is within ViPaq's interoperable range `[0, 2^53 − 1]` (`9007199254740991`,
`Number.MAX_SAFE_INTEGER`). See the integer-range spec. Consequences:

- **C# reads these scenarios as `long`** (not `int`) — `long` holds the whole range exactly, and it is
  the natural pair for JS `number`. A value either fits `int` (then `int` and `long` produce identical
  bytes — no extra coverage) or exceeds `int` (then only `long`/`number` can hold it). Section width is
  chosen from the value, not from the type, so `long` yields the same goldens.
- `little-endian.json` `UInt64` rows stop at `max interoperable` (`2^53 − 1`). The raw-primitive rows
  above it (full `0xFFFF…`, `0x0102…0708`) are out of protocol and stay **C#-local**.

## Files

| File | Shape | Read by |
|---|---|---|
| `exact-bytes.json` | `{Name, Bin, Items[], Bytes[]}` | C# serialize+deserialize golden; TS serialize golden |
| `encoding-info-bytes.json` | `{Name, Version, BinDimensionsBitSize, ItemDimensionsBitSize, ItemCoordinatesBitSize, Byte}` — all 256 combos | C# `EncodingInfoByteTests`; TS `encodingInfo.test.ts` |
| `little-endian.json` | `{UInt16, UInt32, UInt64}[]` of `{Name, Value, Bytes[]}` | C#/TS protocol reader + writer |
| `bit-size-selection.json` | `{Name, Values, ExpectedBitSize}` | C# `BitSizeHelperTests`; TS `getDimensions/getCoordinatesBitSize` |
| `bit-size-invalid.json` | `{Name, Kind, Values, Field}` — inputs that must throw | both bit-size pickers (C# also asserts the `Field` as the thrown param name) |
| `round-trip-scenarios.json` | `{Name, Bin, Items[]}` or `{Name, Bin, Item, Count}` | both serialize → deserialize → assert equal |

### Notes per file

- **`encoding-info-bytes.json`** is combinatorial (`Byte = Version<<6 | Bin<<4 | ItemDim<<2 | ItemCoord`),
  generated, not hand-authored. Regenerate if the enums change.
- **`round-trip-scenarios.json`** uses either an explicit `Items` array or, for many identical items, an
  `Item` template plus a `Count` (the loader expands `Item` × `Count`).
- **`bit-size-invalid.json`** carries `Field` for C# to assert the thrown parameter name; TS only asserts
  that it throws (no typed param name).

## What is **not** here (stays language-local on purpose)

These encode language mechanics, not wire data, so they are not shared:

- C# generic-`T` matrices — `SerializationRoundTripProvider`, `EncodingInfoHelperTestCaseProvider`,
  `BitSizeBoundaryByTypeProvider` (type dispatch, typed exceptions, per-type capping).
- C#-only — saturation-by-type, dispose/double-dispose, `Read8Bits` per numeric type.
- TS-only — `getByteSize`, `getBufferSize`, `writeEncodingInfoToBuffer`, `compressBuffer`,
  `getDecodingDataStream` (buffer pre-sizing and Web-Streams gzip mechanics C# does not have).
- **Compressed payloads are never byte-shared** — not because of the threshold (C# and TS now agree: both
  compress when the body is `> 255`; the old off-by-one is fixed), but because the two gzip engines
  (`GZipStream` vs `CompressionStream`) emit different valid bytes for the same input. So a compressed golden
  can't be byte-compared across languages; that path is covered by decode-to-input, not exact bytes (see the
  gzip cross-decode plan). `round-trip-scenarios.json` stays comfortably over the threshold on purpose, so it
  exercises the compressed path via serialize → deserialize, which works regardless of engine differences.
