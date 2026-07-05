# Binacle.CompactNotation — extract one shared text notation

**Goal:** one text notation for geometry, in one place, used by vipaq, the lib/API log, and the test scenario
data. Today the same idea is written **three times** with **three dialects**. We build the shared library
first, prove it green in isolation, then swap each consumer over one at a time.

## DONE (committed `41b0fcef` "vipaq cleaning") — tightened vipaq's public surface

Goal: shrink `Binacle.ViPaq`'s public API to the format's vocabulary + entry point, and push test-only types
out of the shipped assembly. The serializer is fully generic over the `IWith*<T>` interfaces + the caller's own
`TBin`/`TItem` — it never uses the concrete models (verified: **0** src refs to `Bin`/`Item`/`Dimensions`/
`Coordinates` outside `Models/`).

**Review verdict (agreed).** The public contract is bring-your-own-type: `Serialize<TBin,TItem,T>` /
`Deserialize` constrain on `IWithDimensions<T>` / `IWithCoordinates<T>`. So:
- **Keep public:** `ViPaqSerializer`, `IWithDimensions<T>`, `IWithCoordinates<T>`, the ready-made DTOs
  `Bin<T>` / `Item<T>` (the format *does* accept these), the wire vocabulary `EncodingInfo` / `BitSize` /
  `Version`, and `ViPaqLimits`.
- **Move to tests:** `Dimensions<T>` / `Coordinates<T>` (+ their `Create` factories) — no public format takes a
  standalone measurement/point; they exist only to unit-test `BitSizeHelper` and the protocol writer.

**DONE (committed `41b0fcef`) — model move.** `git mv` `Dimensions.cs` + `Coordinates.cs` from
`src/Binacle.ViPaq/Models/` → `test/Binacle.ViPaq.UnitTests/Models/`, keeping `namespace Binacle.ViPaq` so the
~10 test files (global `using Binacle.ViPaq`) + `VectorParser` need **zero edits**. Header comments updated to
"test-only". `Bin`/`Item` stay in src (public). Green: src builds clean (0 warnings), vipaq **1371** pass, tools
builds.

**DONE (committed `41b0fcef`) — internalize the implementation machinery.** Flipped 7 types `public` → `internal`:
`BitSizeHelper`, `EncodingInfoHelper`, `ProtocolReader<T>`, `ProtocolWriter<T>`, `ProtocolReaderExtensions`,
`ProtocolWriterExtensions`, `EncodingInfoNotation`. The src csproj already granted `InternalsVisibleTo` to
`$(ProjectName).UnitTests`; added `$(ProjectName).Generators` (tools drives `EncodingInfoNotation`). Wire types
(`EncodingInfo`/`BitSize`/`Version`) + `ViPaqLimits` stay public. No other project in the repo references any
internalized type (grep-verified). Tests/tools compiled unchanged via IVT. Green: vipaq src + tools 0 warnings,
vipaq **1371** pass, full `dotnet build Binacle.Net.slnx` succeeds, generator regen byte-identical.

**Resulting public surface of `Binacle.ViPaq`:** `ViPaqSerializer` · `IWithDimensions<T>` ·
`IWithCoordinates<T>` · `Bin<T>` · `Item<T>` · `EncodingInfo` · `BitSize` · `Version` · `ViPaqLimits`.

**DONE (committed `41b0fcef`) — follow-up cleanups.**
- **Dropped the dead `[Experimental("BINACLE_VIPAQ_COMPACT")]`** on `EncodingInfoNotation` (it's `internal` now,
  so the public-preview gate bought nothing) + its `using` and both `<NoWarn>BINACLE_VIPAQ_COMPACT</NoWarn>`
  lines (test + tools csproj).
- **Collapsed the redundant generic constraints.** `IBinaryInteger<T>` already implies `INumber<T>` /
  `IComparable<T>`, and the two sibling interfaces disagreed (`IWithDimensions` verbose, `IWithCoordinates`
  minimal). Swept every `where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>` (and the multi-line
  variants) → `where T : struct, IBinaryInteger<T>` across vipaq src + test.
- **Refreshed the stale docs** (`verified: 2026-07-05`): `.agents/docs/vipaq/README.md` (public surface, the two
  shipped models, the internal machinery, and the rewritten "Encoding-info notation (internal)" section — the
  old "Compact notation" section still listed geometry methods removed back in Phase 1b) and its `also_update:`
  target `vipaq/typescript.md` (same geometry-notation staleness: `compactNotation.ts` → `encodingInfoNotation.ts`
  + the `binacle-compact-notation` package).
- Green: vipaq src + tools 0 warnings, vipaq **1371** pass, regen byte-identical, full `dotnet build` succeeds.

## The notation (final)

A number is `-?\d+` (integers for now; `-` is free for negatives, `.` is reserved for decimals later — not
built yet). Three blocks, fixed order, space-separated:

| Block | Shape | Split on |
|---|---|---|
| dimensions | `LxWxH` | `x` |
| coordinates | `(X,Y,Z)` | `,` inside `()` |
| quantity | `[Q]` | int inside `[]` |

Valid entries: `LxWxH` · `LxWxH [Q]` · `LxWxH (X,Y,Z)` · `LxWxH (X,Y,Z) [Q]` · `(X,Y,Z)`.

Parsing is **explicit, no regex**. The caller usually knows the shape ("I want dimensions"). When the shape is
unknown, a tiny detector decides: starts `(` → coords, starts `[` → quantity, contains `x` → dimensions, else
throw. Parse is lenient about range (reads the ints); range limits belong to each consumer (e.g. vipaq's
`[0, 2^53-1]`).

## Why the punctuation changed (agreed)

- quantity moved off `-Q` / `:Q` to `[Q]` — frees `-` for negatives, and `[` is unambiguous.
- coordinates are always parenthesised `(X,Y,Z)` — same token standalone and inside an item.

This is a **breaking** change to the two committed dialects. The migration (later phases) updates the shared
vectors (`X,Y,Z`→`(X,Y,Z)`, `:Q`→`[Q]`), `vipaq/PROTOCOL.md`, and the TS mirror.

---

## Progress log

- **Phase 0 — DONE (committed).** Built `shared/Binacle.CompactNotation` (BCL-only) + the interfaces,
  models, parser/formatter/detector, and `shared/Binacle.CompactNotation.UnitTests` (xUnit v3 / Shouldly /
  MTP, mirrors the vipaq test project). Both registered in `Binacle.Net.slnx` under `/shared/`. **42 tests
  pass, project builds.** No consumer touched yet.
  - **Resolved:** constraint is `where T : struct, INumber<T>` everywhere (interfaces, models, static methods).
    Kills the `CS8618` warnings cleanly; int+long both qualify. Build is warning-free, 42 tests pass.
- **Phase 0-TS — DONE (committed).** Built `packages/binacle-compact-notation` — the TS mirror of the
  C# lib (npm workspace, covered by the `packages/*` glob). Same grammar, `number`-based (range `[0, 2^53-1]`),
  free functions + `index.ts` barrel + `types.ts` (`Dimensions`/`Coordinates`/`Item` structural shapes).
  `parseNumber` throws on empty/non-integer, so it now **matches** C#'s throwing parse (kills the old
  `Number("")==0` tolerance gap). **40 tests pass, `tsc` clean.**
- **Phase 1 — DONE (committed).** vipaq tests + tools now use both shared notations; **vipaq src
  untouched** (its own `CompactNotation` kept, geometry now dead — same on the TS side: `src/compactNotation.ts`
  kept for encoding-info, geometry dead). What changed:
  - **C#**: `VectorParser` + `InteropArtifactGenerator` call the shared parser and map the shared model into
    vipaq's `Bin<long>`/`Item<long>`. `ParseEncodingInfo` still on vipaq's own notation. Project refs added to
    both csprojs. (Referencing style finalized in Phase 1c below.)
  - **TS**: `binacle-vipaq` depends on `binacle-compact-notation`; the `vectorParser` barrel + interop tool
    import geometry from it (`parseDimensions as parseBin`); `parseEncodingInfo` stays local. Workspace
    resolution (jest/ts-node → realpath outside node_modules) works with no extra config.
  - **Vectors**: geometry strings migrated — items `:Q`→` [Q]` (round-trip, encode-invalid, interop input);
    standalone `Coordinates` rows wrapped in parens (bit-size-selection/-invalid). `exact-bytes` (items already
    parenthesised, no quantity), `decode-invalid`, `little-endian`, `encoding-info-bytes` unchanged.
  - **Docs**: `test-vectors/README.md` compact-strings section updated. `PROTOCOL.md` needed **no** change —
    its "Notation" section is byte/binary notation, not the compact text grammar.
  - **Regen**: `npm run regen:interop` produced **byte-identical** `artifact-cs/ts.json` + `encoding-info-bytes`
    (the migration is purely notational; parsed values unchanged) — only `input.json` differs.
  - **Green**: C# CompactNotation 42, C# vipaq **1371**, TS vipaq **984**, TS notation 40, `tsc` clean, full
    `dotnet build Binacle.Net.slnx` succeeds.
- **Phase 1b — DONE (committed).** Removed the dead geometry from vipaq's own notation and **renamed** it
  so it can't be confused with the canonical `Binacle.CompactNotation`. It now does encoding-info only:
  - **C#**: `Binacle.ViPaq.CompactNotation` → `Binacle.ViPaq.EncodingInfoNotation` (only `ParseEncodingInfo` /
    `FormatEncodingInfo` + the version/width word maps). Callers updated (`VectorParser`,
    `EncodingInfoBytesGenerator`); stale `Bin.cs` comment fixed. Still `[Experimental("BINACLE_VIPAQ_COMPACT")]`
    (kept so the csproj `NoWarn` is untouched — the diagnostic id is opaque; rename it later if desired).
  - **TS**: `src/compactNotation.ts` → `src/encodingInfoNotation.ts` (`parseEncodingInfo` / `formatEncodingInfo`
    only). The `vectorParser` barrel imports from the new path.
  - No `CompactNotation` name left in the vipaq slice except the shared alias. Regen still byte-identical
    (`encoding-info-bytes.json` + interop artifacts unchanged). All suites green as above.
- **Phase 1c — DONE (committed).** Split + renamed the canonical C# facade and dropped the alias:
  - The single `Binacle.CompactNotation.CompactNotation` static class → two classes:
    **`CompactNotationParser`** (`ParseDimensions`/`ParseCoordinates`/`ParseQuantity`/`ParseItem`/`ParseItems`/
    `Detect`) and **`CompactNotationFormatter`** (`Format`/`FormatDimensions`/`FormatCoordinates`/
    `FormatQuantity`). Killed the `Binacle.CompactNotation.CompactNotation` stutter. (`CompactNotationKind`
    enum + the models are unchanged.)
  - **Alias dropped.** vipaq consumers now use `using Binacle.CompactNotation;` + `CompactNotationParser.…`
    directly — no alias. This works because vipaq's own `Bin`/`Dimensions`/`Coordinates`/`Item` live in the
    enclosing `Binacle.ViPaq` namespace and **hide** the shared same-named types, so only the parser/formatter
    types come through the using (the shared `Item` is still referenced fully-qualified in the one map site).
  - TS is unaffected — it already exposes free functions (`parseDimensions`/`formatDimensions`/…), no class to
    split.
  - Green: C# CompactNotation **42**, C# vipaq **1371**, TS **984**, full build; regen byte-identical.
- **Phase 2 — DONE (awaiting commit).** TestsKernel now parses through the shared notation, and the whole
  scenario/test-data corpus moved off the `-Q` dialect onto ` [Q]`:
  - **Kernel parser deleted.** Removed `Helpers/DimensionHelper.cs` (`DimensionsHelper.ParseFromCompactString`)
    and the kernel's own `Models/DimensionsAndQuantity.cs` (nothing else referenced it). The `[Q]`-split moved
    **into the shared lib**, not into TestsKernel.
  - **Shared lib gained the dims-only quantity path.** New `DimensionsAndQuantity<T>` model (`IWithDimensions<T>`
    + an `int Quantity` + `Flatten()` → Q `Dimensions<T>`), `CompactNotationParser.ParseDimensionsAndQuantity<T>`,
    and a private `SplitQuantity` now shared by it and `ParseItems`. +6 unit tests (CompactNotation 42→**48**).
  - **Consumers stay thin.** `TestBin`/`TestItem` gained a ctor taking `Binacle.CompactNotation.IWithDimensions<int>`;
    `TestBin.FromCompactString` (dims only) and `TestItem.FromCompactString` (calls `ParseDimensionsAndQuantity`,
    keeps `Quantity`) are one-liners. `Scenario.Create` maps `items.Select(TestItem.FromCompactString)` and
    `OperationResultHelper` / the benchmark provider call the factories — **no `[` parsing anywhere in TestsKernel**.
    The old hand-rolled `Split('-')`/`Split('x')` is gone.
  - **Project ref**: added `Binacle.CompactNotation` to `Binacle.TestsKernel.csproj`.
  - **Data**: migrated **7324** quoted item literals `"LxWxH-Q"`→`"LxWxH [Q]"` across the 10 embedded JSON files
    (`Algorithms/Data/BischoffSuite/orlib_thpack1..7.json` + `CustomProblems/{baseline,simple,complex}.json`).
    Anchored on the surrounding quotes so `Name` fields carrying the same digit pattern
    (e.g. `"Simple_5x5x5-100_FitIn_60x40x10"`) were **not** touched — `Name` is descriptive, never parsed. Bins
    (no quantity) and the 5-token `OperationResult` strings in `ResultSelection/Data/**` needed no change.
  - **Inline C#**: migrated the item literals in `lib/test/Binacle.Lib.Benchmarks/Providers/`
    `SpecializedScalingProblemsProvider.cs` + `CubeScalingProblemsProvider.cs` (70 occurrences — the
    `itemsByQuantity` dictionary repeats items cumulatively). Explanatory comments left as-is (their `[…]` means
    volume, not quantity — migrating would muddy that).
  - **Docs**: `.agents/docs/shared/README.md` compact-string table Dimensions row updated (`[Q]` + delegation to
    `Binacle.CompactNotation`), `verified: 2026-07-05`.
  - **Green**: full `dotnet build Binacle.Net.slnx` 0 errors, CompactNotation **48**, lib **8615**, api **269**,
    vipaq **1371** pass, performance suite runs clean.
  - **Not in scope (Phase 3):** the UIModule's own `sample_data.json` + `SampleDataService.ParseItem` — a
    separate `-`-splitter, not `DimensionsHelper`.

## Scope decision (locked this session) — Approach A, notation only

We centralize **only the text parse/format**. Every consumer **keeps its own models, interfaces, and
serializer.** We do **not** move models or unify interfaces (that was "Approach B" — rejected because it drags
vipaq's serializer in and hits a readonly-vs-setter conflict on deserialize).

- **Why B was rejected:** vipaq's `Deserialize` does `new TBin(); obj.Length = …` through the interface, which
  needs setters; our Format interfaces are read-only by design. Making them mutable would lock immutable
  Format consumers out. Not worth it — see below, nobody actually needs it.
- **vipaq specifically:** only **tests + tools** adopt the shared notation; **`Binacle.ViPaq` src is
  untouched.** The tools/tests parse via the shared lib and map the result into vipaq's own `Bin`/`Item` (C#
  nominal types need a 6-field copy; TS is structural so the shape is assignable with no map). The
  **encoding-info** notation (`Version_Bin_ItemDim_ItemCoord`) **stays in vipaq** — it depends on
  `EncodingInfo`/`BitSize`/`Version`, which the leaf shared lib can't hold.
- vipaq's tests/tools use only geometry **parse** (+ encoding-info); they never call geometry **format**.

## Phase 1 — DONE: vipaq tests + tools on the shared notation

See the Phase 1 entry in the progress log above for what landed. Summary: C# `VectorParser` +
`InteropArtifactGenerator` and TS `binacle-vipaq` tests/tools now call the shared notations; vectors migrated
(`:Q`→` [Q]`, standalone coords parenthesised); `test-vectors/README.md` updated (PROTOCOL.md needed none);
regen byte-identical; all suites green.

**Done in Phase 1b:** the dead geometry was removed and vipaq's notation renamed to `EncodingInfoNotation`
(C#) / `encodingInfoNotation.ts` (TS) — see the Phase 1b progress-log entry.

## Phase 0 — build `shared/Binacle.CompactNotation` (DONE)

New project, **BCL-only** deps (`System.Numerics`), `net10.0` (inherited from root `Directory.Build.props`, so
the csproj is near-empty like `Binacle.ViPaq.csproj`). Register in `Binacle.Net.slnx` under the `/shared/`
folder.

```
shared/Binacle.CompactNotation/
  Binacle.CompactNotation.csproj
  CompactNotation.cs            // static: Parse* + Format* + Detect
  Abstractions/
    IWithDimensions.cs          // IWithDimensions<T>  { T Length; T Width; T Height; }  (getters)
    IWithCoordinates.cs         // IWithCoordinates<T> { T X; T Y; T Z; }
    IWithQuantity.cs            // IWithQuantity<T>     { T Quantity; }
  Models/
    Dimensions.cs               // Dimensions<T>  : IWithDimensions<T>
    Coordinates.cs              // Coordinates<T> : IWithCoordinates<T>
    Item.cs                     // Item<T>        : IWithDimensions<T>, IWithCoordinates<T>
```

All generic `where T : INumber<T>` — so `int` (lib/API) and `long` (vipaq, full 2^53-1) both implement the same
interfaces with no conversion. Interfaces are **read-only** (getters) — that's all Format needs; concrete
models add setters / init;

**Decisions baked in:**
- **`Bin<T>` dropped** — it was identical to `Dimensions<T>`. One geometry model.
- **Quantity is a block.** For a *placed* item (`LxWxH (X,Y,Z) [Q]`) `ParseItems` expands `[Q]` into Q copies —
  the count is flattened away, so `Item<T>` has no quantity field. For the *dims-only* entry (`LxWxH [Q]`, no
  coords) — Phase-2 addition — `ParseDimensionsAndQuantity<T>` returns a `DimensionsAndQuantity<T>` that **keeps**
  the count as a field (so callers like `TestItem` that need a quantity get it), and its `Flatten()` expands into
  Q standalone `Dimensions<T>` when copies are wanted. (This reverses the earlier "the caller composes
  `ParseDimensions` + `ParseQuantity`" note — the `[Q]`-split now lives once in the lib, shared by `ParseItems`
  and `ParseDimensionsAndQuantity` via a private `SplitQuantity`.)

**Parser surface:**
- `ParseDimensions<T>(string) : Dimensions<T>`
- `ParseCoordinates<T>(string) : Coordinates<T>`  — strips `()`
- `ParseQuantity(string) : int`  — strips `[]`
- `ParseItem<T>(string) : Item<T>`  — `LxWxH (X,Y,Z)`, coords required; rejects a `[Q]` (use `ParseItems`)
- `ParseItems<T>(string) : IReadOnlyList<Item<T>>`  — expands `[Q]`
- `ParseDimensionsAndQuantity<T>(string) : DimensionsAndQuantity<T>`  — `LxWxH` or `LxWxH [Q]`, no coords; keeps
  the count as a field. `.Flatten()` expands it into Q `Dimensions<T>`. (Phase-2 addition.)
- `ParseItems<T>(IEnumerable<string>)` — flattens
- `Detect(string)` — the fallback dispatcher

**Formatter surface:**
- `Format<T>(T value)` — single method, appends a block per interface `value` implements
  (`IWithDimensions`→`LxWxH`, `IWithCoordinates`→` (X,Y,Z)`, `IWithQuantity`→` [Q]`). This is the mirror of the
  detector and the one that lets the API log path adopt by just implementing the interfaces.
- explicit `FormatDimensions<T>` / `FormatCoordinates<T>` / `FormatQuantity` kept as thin public helpers.

**Tests:** new `shared/Binacle.CompactNotation.UnitTests` (or fold into an existing home) covering each parser,
the detector, round-trips, every valid entry combo, and the reject cases. Phase 0 is done when this is green and
the solution builds — **no consumer touched yet.**

---

## Later phases — one at a time, each independently green (all Approach A)

Consumers keep their own types; they call the shared notation and map/pass values. No consumer is forced to
implement the shared interfaces (except that immutable objects *may* implement the read-only ones for `Format`,
which is safe — the setter conflict is only on vipaq's deserialize, which we route around by mapping).

- **Phase 1 — vipaq tests + tools.** DONE — see the progress log.
- **Phase 2 — TestsKernel. DONE (awaiting commit).** Removed the kernel's `DimensionsHelper.ParseFromCompactString`
  and its own `DimensionsAndQuantity` model; the dims-only quantity path moved into the shared lib (new
  `DimensionsAndQuantity<T>` + `ParseDimensionsAndQuantity<T>` + `Flatten()`). Consumers stay thin —
  `TestBin`/`TestItem` gained a shared-`IWith` ctor and `FromCompactString` factories, so `Scenario.Create` /
  `OperationResultHelper` / the benchmark provider just map, with **no `[` parsing in TestsKernel**. Scenario input
  strings migrated `-Q`→` [Q]` (7324 in `Algorithms/Data/**` + 70 inline C# literals in the two benchmark
  providers). (`ScenarioResultHelper`'s `Status-EarlyExitReason` left alone — different notation.)
  See the Phase 2 progress-log entry for details. Green: CompactNotation **48**, lib **8615**, api **269**, vipaq **1371**.
- **Phase 3 — lib/API log. DONE** (landed as Step 6 of the shared-geometry-leaf initiative). Once the geometry
  leaf unified the interfaces, the lib/API models satisfied the shared `CompactNotationFormatter` directly. Routed
  the 5 `Format*` sites through it (UIModule `ViewModels/Item.cs` + `Bin.cs`, `ProtocolDecoder.razor.cs`,
  DiagnosticsModule `LogProcessorHandlingExtensions` ×3), deleted the lib `FormatDimensions` +
  `CoordinateExtensions.cs`, and migrated the UIModule input (`SampleDataService.ParseItem`/`ParseBin` →
  `CompactNotationParser`; `sample_data.json` + `_defaultJsonSampleData` → `[Q]`). Output shifted `-Q`→`[Q]`; no
  test asserted on the old output. Added `Binacle.CompactNotation` refs to UIModule + DiagnosticsModule. Green:
  full build 0 errors, api **269**. See **[shared-geometry-extraction.md](shared-geometry-extraction.md)** (Step 6).

## Out of scope
Unifying the lib's own `IWith*` interface families / per-algorithm `Bin`/`Item` models, and touching any
consumer's serializer/models. This plan extracts **only the text notation**; every library keeps its own types.

## Open, small
- Where the Phase-0 tests live (own project vs folded into vipaq tests). Lean: own project, mirrors the leaf.
