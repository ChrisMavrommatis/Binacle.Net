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
- **Phase 0-TS — DONE (awaiting commit).** Built `packages/binacle-compact-notation` — the TS mirror of the
  C# lib (npm workspace, covered by the `packages/*` glob). Same grammar, `number`-based (range `[0, 2^53-1]`),
  free functions + `index.ts` barrel + `types.ts` (`Dimensions`/`Coordinates`/`Item` structural shapes).
  `parseNumber` throws on empty/non-integer, so it now **matches** C#'s throwing parse (kills the old
  `Number("")==0` tolerance gap). **40 tests pass, `tsc` clean.** No consumer wired yet.

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

## Phase 1 — NEXT: wire vipaq tests + tools onto the shared notation

Approach A. One coupled step (cross-language, because the shared vectors are read by both suites):
1. C# `vipaq/test` `VectorParser` + `vipaq/tools` `InteropArtifactGenerator` → call `Binacle.CompactNotation`,
   map the shared model into vipaq's `Bin<long>`/`Item<long>`. `EncodingInfoBytesGenerator` + encoding-info
   parse stay on vipaq's own notation.
2. TS `binacle-vipaq` tests/tools → import from `binacle-compact-notation`; delete the local
   `src/compactNotation.ts` geometry (keep its encoding-info bits, or split them out).
3. Migrate the shared vectors' geometry strings (`:Q`→`[Q]`, `X,Y,Z`→`(X,Y,Z)`) — geometry files only, not
   encoding-info / little-endian; update `PROTOCOL.md`; regen interop artifacts.
4. Green: C# vipaq suite + `tsc` + jest + full solution build.

**Open (deferred):** vipaq src still ships its own `CompactNotation` (now geometry-duplicated, old punctuation).
Left alone per "tests + tools only." Delete its dead geometry in a later, separate step.

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

- **Phase 1 — vipaq tests + tools.** See "Phase 1 — NEXT" above.
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
