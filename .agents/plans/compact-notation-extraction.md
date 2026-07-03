# Binacle.CompactNotation — extract one shared text notation

**Goal:** one text notation for geometry, in one place, used by vipaq, the lib/API log, and the test scenario
data. Today the same idea is written **three times** with **three dialects**. We build the shared library
first, prove it green in isolation, then swap each consumer over one at a time.

## NEXT (not started) — trim vipaq's test/tool-only models out of the library

`vipaq/src/Binacle.ViPaq/Models/` ships four concrete generic models — `Bin<T>`, `Item<T>`, `Dimensions<T>`,
`Coordinates<T>` — as "canonical implementations" of the `IWith*<T>` interfaces. But the **library itself never
uses them**: the serializer is fully generic over `IWithDimensions<T>`/`IWithCoordinates<T>` + the caller's own
`TBin`/`TItem`. Evidence: `grep 'Bin<' / 'Item<'` over `vipaq/src` (excluding `Models/`) = **0** references;
they're referenced only by `vipaq/test` (~14 files) and `vipaq/tools` (1–2). So they're test/tool fixtures that
happen to live in the shipped library.

**Task:** move `Bin<T>`/`Item<T>`/`Dimensions<T>`/`Coordinates<T>` out of `Binacle.ViPaq` into the test/tool
side (a shared test-support file, or the test project + a tools copy), leaving the library src as just:
serializer + `IWith*<T>` interfaces + `EncodingInfo`/`BitSize`/`Version` + `EncodingInfoNotation`. Notes:
- **Keep the `IWith*<T>` interfaces in src** — the serializer's generic constraints need them.
- First **verify** the concrete `Dimensions<T>`/`Coordinates<T>` models (not the `IWith*` interfaces, which the
  grep conflates) truly have no src use — the serializer constructs `new TBin()`/`new TItem()` from caller
  types on deserialize, so it shouldn't, but confirm.
- The interop tool + tests already need their own concrete types; this just relocates them. No wire/behaviour
  change, so all suites + regen stay byte-identical.
- Consider whether these should just **be** the shared `Binacle.CompactNotation` models — but that reopens the
  readonly-vs-setter deserialize conflict (Approach B), so likely keep vipaq's own mutable ones test-side.

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

- **Phase 0 — DONE (awaiting commit).** Built `shared/Binacle.CompactNotation` (BCL-only) + the interfaces,
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
- **Phase 1 — DONE (awaiting commit).** vipaq tests + tools now use both shared notations; **vipaq src
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
- **Phase 1b — DONE (awaiting commit).** Removed the dead geometry from vipaq's own notation and **renamed** it
  so it can't be confused with the canonical `Binacle.CompactNotation`. It now does encoding-info only:
  - **C#**: `Binacle.ViPaq.CompactNotation` → `Binacle.ViPaq.EncodingInfoNotation` (only `ParseEncodingInfo` /
    `FormatEncodingInfo` + the version/width word maps). Callers updated (`VectorParser`,
    `EncodingInfoBytesGenerator`); stale `Bin.cs` comment fixed. Still `[Experimental("BINACLE_VIPAQ_COMPACT")]`
    (kept so the csproj `NoWarn` is untouched — the diagnostic id is opaque; rename it later if desired).
  - **TS**: `src/compactNotation.ts` → `src/encodingInfoNotation.ts` (`parseEncodingInfo` / `formatEncodingInfo`
    only). The `vectorParser` barrel imports from the new path.
  - No `CompactNotation` name left in the vipaq slice except the shared alias. Regen still byte-identical
    (`encoding-info-bytes.json` + interop artifacts unchanged). All suites green as above.
- **Phase 1c — DONE (awaiting commit).** Split + renamed the canonical C# facade and dropped the alias:
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
- **Quantity is a block, not a model field.** Atomic parsers return atomic things; the caller composes. The
  dims+quantity entry (`LxWxH [Q]`, no coords) is split by the caller into `ParseDimensions` + `ParseQuantity`.
  vipaq's list case is served by `ParseItems` expanding `[Q]` into Q copies.

**Parser surface:**
- `ParseDimensions<T>(string) : Dimensions<T>`
- `ParseCoordinates<T>(string) : Coordinates<T>`  — strips `()`
- `ParseQuantity(string) : int`  — strips `[]`
- `ParseItem<T>(string) : Item<T>`  — `LxWxH (X,Y,Z)`, coords required; rejects a `[Q]` (use `ParseItems`)
- `ParseItems<T>(string) : IReadOnlyList<Item<T>>`  — expands `[Q]`
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
- **Phase 2 — TestsKernel.** `DimensionsHelper.ParseFromCompactString` (`LxWxH-Q`) calls `ParseDimensions` +
  `ParseQuantity`, still returning its own `DimensionsAndQuantity`. Scenario input strings migrate `-Q`→`[Q]`.
  (Leave `ScenarioResultHelper`'s `Status-EarlyExitReason` alone — different notation.) Green: lib test suite.
- **Phase 3 — lib/API log.** Route `LogProcessorHandlingExtensions` (and the UIModule `FormatDimensions()` ID
  sites) through the shared formatters; delete the lib `Format*` extensions. Log/UI output shifts `-Q`→`[Q]`
  (the agreed break). Whether the log types implement the read-only interfaces or pass values is a Phase-3
  detail. Green: API integration suite.

## Out of scope
Unifying the lib's own `IWith*` interface families / per-algorithm `Bin`/`Item` models, and touching any
consumer's serializer/models. This plan extracts **only the text notation**; every library keeps its own types.

## Open, small
- Where the Phase-0 tests live (own project vs folded into vipaq tests). Lean: own project, mirrors the leaf.
