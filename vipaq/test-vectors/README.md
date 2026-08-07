# ViPaq shared test vectors

Language-neutral test data read by **both** ViPaq implementations:

- C# — `vipaq/test/Binacle.ViPaq.UnitTests`
- TS — `vipaq/packages/binacle-vipaq`

One file, two consumers. The same inputs are graded against the same answers, so the two implementations can't
silently drift on the wire. These files are the single source of truth; edit a case here, not in either suite. The
wire itself is defined in [`../PROTOCOL.md`](../PROTOCOL.md), which stands alone.

## Rules every file follows

- **PascalCase keys** — `Name`, `Bin`, `Items`, `Bytes`, etc. (so C# binds with no case-insensitive option; reads
  fine in TS too).
- **Unique `Name` per scenario.** It is the join key across languages and the test label on each side: C# passes
  the `Name` and resolves the row from a name-keyed dictionary; TS uses jest `test.each(...)("$Name", ...)`.
  (`header-bytes.json` is the exception: its unique `Header` notation string is the label.)
- **Compact strings for inputs** (the shared `Binacle.CompactNotation` / `binacle-compact-notation` grammar, one
  grammar for the whole repo):
  - **Dimensions / bin** — `"LxWxH"`, e.g. `"10x20x30"`.
  - **Coordinates** — `"(X,Y,Z)"`, e.g. `"(4,5,6)"`, always parenthesised.
  - **Item** — `"LxWxH (X,Y,Z)"`, e.g. `"1x2x3 (4,5,6)"`. An optional ` [Q]` suffix repeats the item `Q` times,
    e.g. `"10x10x10 (0,0,0) [40]"` = 40 copies. The quantity is bracketed on purpose so `-` stays free for
    negative dims/coords (which `encode-invalid.json` uses to check rejection).
- **Bytes are strings.** The two header bytes are grouped **binary** so the fields are legible — byte 0 is
  `Version(2)_Compressed(1)_Layout(1)_reserved(4)` (`"0b00_0_0_0000"`), byte 1 is
  `BinWidth(2)_ItemDimWidth(2)_ItemCoordWidth(2)_reserved(2)` (`"0b00_00_00_00"`). Every other byte is **hex**
  (`"0x0A"`). Parser rule: `0b` → base 2, `0x` → base 16, strip `_`; each entry is one byte.
- **Enum names, not numbers** — `Width` is `Eight | Sixteen` (codes 0–1; 2–3 are reserved and never written);
  the header string names `Version`, `raw`/`comp` (`Compressed`), and `row`/`col` (`Layout`).

## Integer range

Every dimension and coordinate is in `[0, 65535]` (PROTOCOL.md §5). There is no wider width and no 32/64-bit tier —
a value above 65,535 is an error, not a wider encoding. C# reads these scenarios as `int`, which holds the range
and is the safe default `T`.

## Header notation

The header's text form, used wherever a vector needs to name a full header:
`v{N}_{raw|comp}_{row|col}_{binWidth}_{itemDimWidth}_{itemCoordWidth}` — e.g. `v1_raw_row_8_8_8` or
`v1_comp_col_16_8_16`. Six tokens in wire order. Mirrors C# `HeaderNotation` / TS `src/headerNotation.ts`.

## Files

| File | Shape | Read by |
|---|---|---|
| `serialization/exact-bytes.json` | `{Name, Bin, Items[], Bytes{Header[2], Count[2], Bin[3], Items[{Dims[3], Coords[3]}]}}` | C# serialize+deserialize golden; TS serialize golden |
| `serialization/round-trip-scenarios.json` | `{Name, Bin, Items[], ExpectedHeader}` | both: serialize → assert the header notation → deserialize → assert equal |
| `serialization/decode-invalid.json` | `{Name, Blob[], Reason}` — blobs that must be rejected | both: feed `Blob` to deserialize, assert it throws |
| `serialization/encode-invalid.json` | `{Name, Bin, Items[], Reason}` — inputs that must be rejected | both: serialize, assert it throws |
| `header/header-bytes.json` | `{Header, Bytes[2]}` — the 32 valid header combos | both: pack `Header` notation → assert the two bytes, and read back |
| `width/width-selection.json` | `{Name, Kind, Values, ExpectedWidth}` | `Kind` (`Dimensions`/`Coordinates`) routes each row to its width picker; both must return `ExpectedWidth` |
| `width/width-invalid.json` | `{Name, Kind, Values, Field}` — inputs that must throw | both pickers; assert the offending `Field` |
| `protocol/little-endian/uint8.json`, `uint16.json` | `{Name, Value, Bytes[]}` | C#/TS protocol reader + writer, the two widths |
| `interop/input.json` | `{Name, ExpectedHeader, Bin, Items[]}` — the shared inputs both generators serialize | both: the answer key a decoded artifact must equal (joined by `Name`) |
| `interop/{cs,ts}/{raw,deflate,gzip}.json` | `{Name, Producer, Base64}` — each language's artifacts, one file per codec | both: decode → assert equals `input.json[Name]` |

## Interop — decode-to-input, never byte-equality

`interop/input.json` is serialized by each language's generator under each codec (`raw`, `deflate`, `gzip`), giving
`interop/cs/*.json` and `interop/ts/*.json`. Each suite decodes **the other language's** artifacts and asserts the
result equals the input.

**Compressed bytes are never compared.** The same body, the same codec, and two different compressor engines can
each emit a different valid DEFLATE stream (`DeflateStream` vs `CompressionStream('deflate-raw')`), so the contract
is decode-to-input (PROTOCOL.md §6.1). Raw artifacts are byte-identical across producers and can be compared
directly.

## Regenerating

The generators own only the derived files — `header/header-bytes.json` and the interop artifacts. **Every other
vector is hand-authored**, so do not expect a regeneration to reproduce them. C#:
`vipaq/tools/Binacle.ViPaq.VectorGenerators/`. TS: `vipaq/packages/binacle-vipaq/tools/`. Output is
deterministic — a no-change re-run is byte-identical.

## What is **not** here (stays language-local on purpose)

Language mechanics, not wire data: C# generic-`T` matrices and typed exceptions; TS buffer pre-sizing and
Web-Streams codec mechanics. Only shared-vector coverage matches across suites; the suites' totals differ by
design.
