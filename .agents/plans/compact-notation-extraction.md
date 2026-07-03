# Binacle.CompactNotation — extract one shared text notation

**Goal:** one text notation for geometry, in one place, used by vipaq, the lib/API log, and the test scenario
data. Today the same idea is written **three times** with **three dialects**. We build the shared library
first, prove it green in isolation, then swap each consumer over one at a time.

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
- **Phase 1 — NEXT.** vipaq (see below).

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

## Phase 1..3 — replace consumers, one at a time (each independently green)

**Phase 1 — vipaq.** Delete the geometry half of vipaq `CompactNotation`; move `Dimensions`/`Coordinates`/`Item`
out of vipaq into the shared project; vipaq references shared. **Stays in vipaq:** `EncodingInfo`/`BitSize`/
`Version` and the `Version_Bin_ItemDim_ItemCoord` encoding-info notation (wire-specific) — as a small vipaq-local
helper. Verify the serializer only *reads* dimensions (getters) so the read-only interfaces suffice. Then: regen
the shared vectors to the new punctuation, update `PROTOCOL.md`, and mirror the grammar in TS
`compactNotation.ts`. Green: C# vipaq suite + `tsc` + jest + full build.

**Phase 2 — TestsKernel.** `DimensionsHelper.ParseFromCompactString` (`LxWxH-Q`) delegates to `ParseDimensions`
+ `ParseQuantity`, still returning its `DimensionsAndQuantity`. Scenario input strings migrate `-Q`→`[Q]`.
(Leave `ScenarioResultHelper`'s `Status-EarlyExitReason` alone — different notation.) Green: lib test suite.

**Phase 3 — lib/API log.** Delete `DimensionExtensions.FormatDimensions` / `CoordinateExtensions.
FormatCoordinates`; the log objects implement the shared interfaces; `LogProcessorHandlingExtensions` calls
`Format`. Output shifts `-Q`→`[Q]` (the agreed break — any downstream log parser must update). Green: API
integration suite.

## Out of scope
Unifying the lib's own `IWith*` interface families / per-algorithm `Bin`/`Item` models — that's a separate,
larger refactor. This plan only extracts the **notation** and the geometry carriers it returns.

## Open, small
- Where the Phase-0 tests live (own project vs folded into vipaq tests). Lean: own project, mirrors the leaf.
