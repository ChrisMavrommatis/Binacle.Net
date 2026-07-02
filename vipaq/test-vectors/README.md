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
  (`encoding-info-bytes.json` is the exception: its unique `EncodingInfo` string is the label, so it carries no
  separate `Name`.)
- **Compact strings for inputs** (same convention as `shared/Binacle.TestsKernel`):
  - **Dimensions / bin** — `"LxWxH"`, e.g. `"10x20x30"` (split on `x` → 3 ints).
  - **Coordinates** — `"X,Y,Z"`, e.g. `"4,5,6"` (split on `,` → 3 ints). A coordinate is comma-separated
    everywhere it appears (an item's coords, a bit-size coordinates row), so one parser reads it the same way.
  - **Item** — `"LxWxH (X,Y,Z)"`, e.g. `"1x2x3 (4,5,6)"` (dims, space, then `(X,Y,Z)`). An optional `:Q`
    suffix repeats the item `Q` times, e.g. `"1x2x3 (4,5,6):60"` = 60 copies; no suffix means one. The
    quantity separator is `:` (not `-` as in `shared/Binacle.TestsKernel`) **on purpose**, so `-` stays free for
    negative dims/coords — e.g. `"-1x2x3 (4,5,6)"` or `"1x2x3 (-4,5,6)"` — which `encode-invalid.json` uses to
    check the serializer rejects them. Parse: split off `:Q` first, then `LxWxH (X,Y,Z)` allowing a leading `-`
    on any int.
  - **Values (bit-size files)** — parsed by the row's `Kind`: `Dimensions` → `"LxWxH"`, `Coordinates` →
    `"X,Y,Z"`. `Kind` tells you which parser reads it, so there is no ambiguous "triple"; negatives allowed
    where a case expects a throw.
- **Bytes are strings.** The encoding header byte is grouped **binary** so the flags are legible
  (`"0b00_00_00_00"` = `Version | Bin | ItemDim | ItemCoord`); every other byte is **hex**
  (`"0x0A"`). Parser rule: `0b` → base 2, `0x` → base 16, strip `_`; each entry is one byte.
- **Enum names, not numbers** — `Version` is `Uncompressed | CompressedGzip | Reserved2 | Reserved3`;
  bit sizes are `Eight | Sixteen | ThirtyTwo | SixtyFour`. Both languages already have these names. The one
  exception is `encoding-info-bytes.json`, whose `EncodingInfo` string uses short `Compressed`
  (= `CompressedGzip`) and numeric widths `8 | 16 | 32 | 64` to stay terse across 256 rows (see its note).

## Integer range

Every value is within ViPaq's interoperable range `[0, 2^53 − 1]` (`9007199254740991`,
`Number.MAX_SAFE_INTEGER`). See the integer-range spec. Consequences:

- **C# reads these scenarios as `long`** (not `int`) — `long` holds the whole range exactly, and it is
  the natural pair for JS `number`. A value either fits `int` (then `int` and `long` produce identical
  bytes — no extra coverage) or exceeds `int` (then only `long`/`number` can hold it). Section width is
  chosen from the value, not from the type, so `long` yields the same goldens.
- `little-endian/uint64.json` rows stop at `max interoperable` (`2^53 − 1`). The raw-primitive rows
  above it (full `0xFFFF…`, `0x0102…0708`) are out of protocol and stay **C#-local**.

## Files

| File | Shape | Read by |
|---|---|---|
| `exact-bytes.json` | `{Name, Bin, Items[], Bytes{Header, Count[], Bin[], Items[{Dims[], Coords[]}]}}` | C# serialize+deserialize golden; TS serialize golden |
| `encoding-info-bytes.json` | `{EncodingInfo, Byte}` — all 256 combos | C# `EncodingInfoByteTests`; TS `encodingInfo.test.ts` |
| `little-endian/` | one file per width — `uint8.json` … `uint64.json`, each `[{Name, Value, Bytes[]}]` | C#/TS protocol reader + writer |
| `bit-size-selection.json` | `{Name, Kind, Values, ExpectedBitSize}` | `Kind` routes each row to its own picker; the two sets cover every width bucket, so the pickers can't drift |
| `bit-size-invalid.json` | `{Name, Kind, Values, Field}` — inputs that must throw | both pickers; `Kind` routes; both assert the offending `Field` (C# `ParamName`, TS lowercased message) |
| `round-trip-scenarios.json` | `{Name, Bin, Items[], ExpectedEncodingInfo}` | both serialize → assert byte 0 == `ExpectedEncodingInfo` → deserialize → assert equal |
| `decode-invalid.json` | `{Name, Blob[], Reason}` — blobs that must be rejected | both: feed `Blob` to deserialize, assert it throws |
| `encode-invalid.json` | `{Name, Bin, Items[], Reason}` — inputs that must be rejected | both: serialize, assert it throws |

Together with `bit-size-invalid.json`, these cover the whole `PROTOCOL.md §8` reject table: dim ≤ 0 / coord < 0 /
value > MaxInteger on **encode** → `bit-size-invalid` (picker level) and `encode-invalid` (end-to-end serialize);
item count > 65535 → `encode-invalid`; input < 1 byte / `Reserved2`/`Reserved3` version / value > MaxInteger on
**decode** → `decode-invalid`.

### Notes per file

- **`exact-bytes.json`** — `Bytes` mirrors the wire layout so each segment is legible: `Header` (1 byte),
  `Count` (2-byte `uint16`), `Bin`, then one object per item with `Dims` and `Coords`. The loader flattens
  `Header :: Count :: Bin :: (Dims :: Coords per item)` to get the blob and compares it to the serializer output.
- **`encoding-info-bytes.json`** is combinatorial, all 256 combos, generated (not hand-authored); regenerate if
  the enums change. Each row is `{EncodingInfo, Byte}`. `EncodingInfo` is
  `"{Version}_{BinBitSize}_{ItemDimBitSize}_{ItemCoordBitSize}"`, e.g. `"Compressed_8_8_16"` — `Version` is
  `Uncompressed | Compressed | Reserved2 | Reserved3` (`Compressed` maps to the `CompressedGzip` enum), widths
  are `8 | 16 | 32 | 64`. The loader splits on `_` for the four field **inputs**; `Byte` (grouped binary) is the
  independent **expected** output (`Version<<6 | Bin<<4 | ItemDim<<2 | ItemCoord`). The composer never sees
  `Byte`, so it stays a real golden, not a round-trip.
- **`bit-size-selection.json`** — width selection uses identical math for dimensions and coordinates. `Kind`
  routes each row to **one** picker and says how `Values` is parsed (`Dimensions` → `LxWxH` →
  `getDimensionsBitSize`; `Coordinates` → `X,Y,Z` → `getCoordinatesBitSize`); each picker must return
  `ExpectedBitSize`. No row is fed to both pickers. The dimensions set and the coordinates set **together** cover
  every width bucket, and that is what pins the two pickers so they can't drift apart.
- **`bit-size-invalid.json`** — `Kind` (`Dimensions` / `Coordinates`) routes the case to the right picker
  (dims reject 0, coords allow 0) and says how `Values` is written (`Dimensions` → `LxWxH`, `Coordinates` →
  `X,Y,Z`). `Field` is the canonical PascalCase field name (= C#'s `ParamName`): C# asserts `ParamName ==
  Field`; TS asserts the thrown message contains `Field` lowercased.
- **`round-trip-scenarios.json`** — every scenario is a flat `Items[]`; an item may carry a `-Q` suffix to
  repeat it (see the compact-string rules). Round-trip equality alone is weak — it passes even if the serializer
  picks the wrong widths or compression — so **every** case also carries `ExpectedEncodingInfo` (the same
  `EncodingInfo` string format as `encoding-info-bytes.json`): the test serializes, asserts byte 0 decodes to
  that header (pinning `Version` **and** all three bit sizes), then deserializes and asserts the items match.
  The two `…compression threshold…` cases straddle the §6 trigger as tightly as the format allows — a real body
  is always `≡ 2 (mod 3)` (a 2-byte count plus three-int sections), so body `254` (`Uncompressed_16_8_8`) and
  `257` (`Compressed_8_8_8`) are the closest reachable pair; exactly `255`/`256` can never occur, so the trigger
  is only observable as "raw at 254, compressed at 257".
- **`decode-invalid.json`** — raw blobs (`Blob`, same `0x`/`0b` token rules) the decoder MUST reject. The test
  only asserts *that* deserialize throws; `Reason` is documentation (exception type/message differ per language).
  The reserved-version blobs carry a **valid body** on purpose, so the only possible cause of rejection is the
  version (isolates the §7 step-2 check). `coordinate above MaxInteger` / `bin dimension above MaxInteger` store
  `2^53` in a `SixtyFour` field — coords vs the dimensions guard (a coord-only blob never exercises the dims
  guard); `truncated body` ends mid-body; `invalid gzip body` sets `CompressedGzip` over non-gzip bytes.
- **`encode-invalid.json`** — inputs the serializer MUST reject end-to-end (the picker-level twins live in
  `bit-size-invalid.json`). The test only asserts the throw. `item count over uint16 max` uses `:65536` so the
  loader expands to 65536 items without listing them; the rest feed negative / zero / above-MaxInteger dims and
  coords — expressible now that `-` is free (see the item compact-string rule).

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
