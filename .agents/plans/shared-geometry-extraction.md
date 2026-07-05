# Shared geometry leaf — one home for the common models

**Status (2026-07-05): NOT STARTED — next-session initiative.** Too big to fold into the current session. This
plan is the handoff: what led here, what is already done, and exactly what to do next.

**Goal.** Extract the common geometry types — the `IWith*` interfaces and the concrete `Dimensions` / `Coordinates`
models — into **one shared leaf project**, and use it across `Binacle.Net`, `Binacle.Lib`, `Binacle.ViPaq`, the
shared notation, and the tests. Today those types are duplicated as **two parallel interface families**, which is
the one thing blocking the last step of the compact-notation work.

---

## Naming decision (open — pick before starting)

We cannot have both "Core" and "Kernel" — `Binacle.Net.Kernel` and `Binacle.TestsKernel` already own "Kernel".
The words mean different things and should stay distinct:

- **Kernel** = *layer plumbing* — API host wiring (`Binacle.Net.Kernel`), test scaffolding (`Binacle.TestsKernel`).
- **This new leaf** = *domain model shared by every layer*.

| Name | Pros | Cons |
|---|---|---|
| `Binacle.Kernel` | "Kernel" already reads as foundational | Collides with **two** existing kernels (API + tests), three meanings. Worst. |
| `Binacle.Core` | Conventional "everything depends on it"; grows beyond geometry | Clashes with "Kernel" (which is more fundamental?); generic; forces renaming the kernels. |
| `Binacle.Geometry` | Precise; says what's inside; no clash; zero rename churn | Too narrow if it later holds non-geometry shared models. |
| `Binacle.Primitives` | Broad enough for geometry + other shared value types; no clash | Slightly vague. |

**Recommendation:** `Binacle.Geometry` if it stays dimensions/coordinates/quantity; `Binacle.Primitives` if it's
the general shared-model home. Avoid `Binacle.Core` unless renaming both kernels; avoid `Binacle.Domain`
(`ServiceModule.Domain` exists); "Kernel" is out for the leaf. Below the leaf is called **`Binacle.Geometry`** —
swap once decided.

## Companion cleanup — `src` / `test` / `packages` convention per slice

Do this **with** (or just before) the leaf extraction, so the new leaf lands under the right layout from day one.
Target: every slice is `src/` (C#) · `test/` (C#) · `packages/` (TS/JS). Today it's inconsistent:

- `lib/`, `api/` → `src` + `test` ✓
- `vipaq/` → `src` + `test` + `tools`, but its TS mirror sits at `vipaq/binacle-vipaq` (not `packages`)
- `shared/` → **flat**: `Binacle.CompactNotation`, `…UnitTests`, `Binacle.TestsKernel`, `data`, `README.md`
- **root `packages/`** → `binacle-net-ui`, `cookies`, `theme-switcher`, `binacle-compact-notation`

Moves:
- `shared/Binacle.CompactNotation` + `Binacle.TestsKernel` → `shared/src/…`; `…UnitTests` → `shared/test/…`;
  new leaf → `shared/src/Binacle.Geometry`. (`shared/data` stays or → `shared/data` unchanged.)
- `packages/binacle-compact-notation` → `shared/packages/binacle-compact-notation`.
- `vipaq/binacle-vipaq` → `vipaq/packages/binacle-vipaq`.
- **Open decision:** the root UI packages (`binacle-net-ui`, `cookies`, `theme-switcher`) need a slice home —
  likely `api/packages/` or a `web/packages/`. Not mechanical; decide first.

What it ripples into (miss one → build/CI breaks — do it as its own verified commit):
- `Binacle.Net.slnx` — solution folders (`/shared/` → `/shared/src/`, `/shared/test/`) + every `<Project Path>`.
- Every `.csproj` `ProjectReference` relative path (shared move breaks refs from `lib/test`, `api/test`,
  `lib.Abstractions`→`TestsKernel`, and `TestsKernel`/vipaq→`CompactNotation`).
- npm `workspaces` in root `package.json` (`packages/*`, `vipaq/binacle-vipaq`) → new globs; cross-package deps
  (`binacle-vipaq`→`binacle-compact-notation`) still resolve after the move (re-run `npm install`).
- `config/*.sh` run/test scripts, `gulpfile` asset copy, `Dockerfile`/`config/build.sh`, and the agent docs
  (`.agents/docs/README.md` repo-layout table + `_index.md` + slice docs).

Verify gates after: full `dotnet build Binacle.Net.slnx`, all C# suites, `npm install` + TS suites, and a docker
build. Best sequenced as **step 0** of the leaf branch.

---

## Why we're doing this — the blocker we hit

There are **two parallel families of the same idea**:

- **Lib** (`Binacle.Lib.Abstractions.Models`): `IWithReadOnlyDimensions` / `IWithReadOnlyDimensions<T>` (getters,
  `where T : INumber<T>`), `IWithDimensions` / `IWithDimensions<T>` (add setters), `IWithReadOnlyCoordinates`,
  `IWithQuantity`. Non-generic variants are `int`-based. Used by **every** algorithm, processor, model, and test.
- **CompactNotation** (`Binacle.CompactNotation`): `IWithDimensions<T>` / `IWithCoordinates<T>` / `IWithQuantity<T>`
  (read-only, `where T : struct, INumber<T>`) + concrete `Dimensions<T>` / `Coordinates<T>` / `Item<T>`.

Because they're different types, the shared `CompactNotationFormatter` (generic, reads the CompactNotation
interfaces) **cannot format a lib model** without mapping into a shared model or bolting on adapters. That is the
wall Phase 3 of the compact-notation plan hit. Unifying the families removes it — and removes the duplication
for good.

**vipaq caveat (must preserve):** vipaq's `Deserialize` writes through a **mutable** interface (`new TBin(); obj.Length = …`),
while formatting only needs **read-only**. So the leaf must keep the read-only vs mutable split (read-only base,
mutable derived) — do **not** collapse them into one mutable interface, or immutable Format consumers get locked out.
This is why the earlier "Approach B" was rejected wholesale; the safe slice is the **read-only** interfaces.

---

## What is already DONE (context for the next session)

The compact-notation extraction (`compact-notation-extraction.md`) is done through **Phase 2**:

- **Phase 0 / 0-TS / 1 / 1b / 1c** (committed, latest `41b0fcef`): built `shared/Binacle.CompactNotation` (C#)
  + `packages/binacle-compact-notation` (TS); migrated vipaq tests/tools onto the shared notation; renamed
  vipaq's own notation to `EncodingInfoNotation`; split the C# facade into `CompactNotationParser` /
  `CompactNotationFormatter`; tightened vipaq's public surface.
- **Phase 2 — TestsKernel (this session, awaiting commit):**
  - Removed `Binacle.TestsKernel/Helpers/DimensionHelper.cs` and its own `Models/DimensionsAndQuantity.cs`.
  - Added to the shared lib: `DimensionsAndQuantity<T>` (dims + `int Quantity` + `Flatten()`),
    `CompactNotationParser.ParseDimensionsAndQuantity<T>`, and a private `SplitQuantity` shared with `ParseItems`.
    (+6 unit tests, CompactNotation 42→**48**.)
  - `TestBin`/`TestItem` gained a `Binacle.CompactNotation.IWithDimensions<int>` ctor + `FromCompactString`
    factories; `Scenario.Create` / `OperationResultHelper` / the benchmark provider just map — **no `[` parsing in
    TestsKernel**.
  - Migrated the whole scenario corpus `-Q`→` [Q]` (7324 quoted item literals across 10 embedded JSON files + 70
    inline C# benchmark literals). `Name` fields never touched (descriptive, never parsed).
  - Docs updated (`shared/README.md`, `.agents/docs/shared/README.md`).
  - Green: build 0 errors · CompactNotation **48** · lib **8615** · api **269** · vipaq **1371**.
- **Phase 3 — DEFERRED into this initiative.** The lib `Format*` extensions still emit `-Q`
  (`DimensionExtensions.cs`, `CoordinateExtensions.cs` in `Binacle.Lib.Abstractions`). No test asserts on that
  output (verified), so the eventual `-Q`→`[Q]` break is safe.

`Flatten()` on `DimensionsAndQuantity<T>` currently has **no consumer** — it exists as the shared capability;
wire it or drop it during this work.

---

## What we WILL do

### 1. Create the leaf `shared/src/Binacle.Geometry` (under the new convention)
BCL-only (`System.Numerics`), `net10.0`, registered in `Binacle.Net.slnx` under `/shared/`. Same near-empty
csproj style as `Binacle.CompactNotation`. It becomes the lowest dependency in the graph — **it references nothing
of ours**.

### 2. Define the canonical interfaces (reconcile the two families)
Move/define here, keeping the read-only/mutable split:
- `IWithReadOnlyDimensions<T>` (getters) and `IWithDimensions<T> : IWithReadOnlyDimensions<T>` (setters).
- `IWithReadOnlyCoordinates<T>` / `IWithCoordinates<T>`.
- `IWithQuantity<T>` (decide: read-only getter; quantity type — keep it `int`-flavoured or `T`? today
  CompactNotation uses `T`, TestsKernel/lib use `int`).
- Decide the **`struct` constraint**: CompactNotation uses `where T : struct, INumber<T>`; lib uses
  `where T : INumber<T>`. Standardize (likely `struct, INumber<T>` — all real `T` are `int`/`long`), and check no
  generic method breaks.
- Decide the **non-generic `int` convenience aliases** (lib's `IWithReadOnlyDimensions : …<int>`). Either keep
  them in lib for source-compat or drop them.

### 3. Move the concrete models
`Dimensions<T>`, `Coordinates<T>` into the leaf. Decide whether `Item<T>` and `DimensionsAndQuantity<T>` move too
(they're notation-flavoured — likely stay in `Binacle.CompactNotation`, which will now reference the leaf).

### 4. Rewire the consumers to the leaf
- **`Binacle.CompactNotation`** → reference the leaf; delete its own copies of the interfaces/models it moved.
  (This supersedes the earlier idea of a separate `CompactNotation.Abstractions` — fold it into the leaf.)
- **`Binacle.Lib.Abstractions`** → its `IWith*` family becomes / extends the leaf's. This is the big blast radius:
  every algorithm, processor, model, and lib test references these. Expect a wide but mechanical sweep + namespace
  churn (watch the `IWithDimensions<T>` name collision — pick one home).
- **`Binacle.ViPaq`** → it already serializes over `IWith*<T>`; point it at the leaf's interfaces.
- **`Binacle.Net`** (API + UIModule + DiagnosticsModule) and **tests** → use the leaf's types.

### 5. Finish the compact-notation Phase 3 (now trivial)
Once lib/API models implement the leaf's read-only interfaces, `CompactNotationFormatter.Format<int>(model)` "just
works". Then:
- Swap the 5 `Format*` call sites (`UIModule/ViewModels/Item.cs`, `Bin.cs`, `ProtocolDecoder.razor.cs`,
  `DiagnosticsModule/LogProcessorHandlingExtensions.cs` lines 30/36/41 — note line 41's hand-written `-{Quantity}`).
- Delete the lib `DimensionExtensions.FormatDimensions` + `CoordinateExtensions.FormatCoordinates`.
- Migrate UIModule input too (`SampleDataService.ParseItem` `-`-splitter + `wwwroot/data/sample_data.json`'s 8
  `-Q` item strings + the hard-coded `_defaultJsonSampleData`).
- Output becomes `[Q]` in logs + UI IDs. No assertions depend on it (verified).

### 6. Wire or drop `Flatten()`; verify; review
Green gates: full `dotnet build`, CompactNotation, lib, api, vipaq, TS suites. Then `/code-review` the branch —
this touches the core's most-used interfaces, so it earns a dedicated review.

---

## Risks / notes
- **Blast radius is the whole `Binacle.Lib` slice + its tests** — the interfaces are everywhere. Mechanical but wide.
- **Do it on its own branch/PR**, not mixed with feature work. Land Phase 2 (this session's changes) first.
- **TS is unaffected** — the TS mirror is structural (no interfaces); `packages/binacle-compact-notation` needs no
  parallel leaf.
- Keep the **read-only vs mutable** interface split (vipaq deserialize needs setters).
- Confirm nothing depends on the exact **`Binacle.Lib.Abstractions.Models`** namespace for the moved types
  (or add `[TypeForwardedTo]` / keep thin shims if source-compat matters).

## Open questions
1. Final leaf name (`Binacle.Geometry` recommended).
2. Quantity type in `IWithQuantity<T>` — `int` vs `T`.
3. The `struct` constraint — standardize on `struct, INumber<T>`?
4. Do `Item<T>` / `DimensionsAndQuantity<T>` move to the leaf, or stay notation-side?
5. Keep lib's non-generic `int` interface aliases as shims, or sweep every call site?
