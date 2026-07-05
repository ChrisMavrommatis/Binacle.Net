# Binacle.Geometry — one home for the common models

**Status (2026-07-06): DONE.** All 7 runbook steps landed (layout move → leaf → CompactNotation → lib.Abstractions
→ rest → Phase 3 `[Q]` → cleanup/review). One shared `Binacle.Geometry` leaf; `[Q]` everywhere; all C# suites + TS
green; manual branch review clean (no Critical/Major/Minor). Deferred quality-only cleanups are tagged
`[Migrate-Review]` in-code (grep for them). A **follow-on model-consolidation pass** landed after the runbook
(Tier 1 shared `Item<T>`, `GeometryFactory`, and moving the int-shortcut + volume interfaces into the leaf) — see
**Follow-on consolidation & what remains** at the end of this file. Original handoff notes below kept for history.

> ## Read first — this is a hypothesis, not instructions. Trust nothing here; verify everything.
>
> This document was written in one working session and reflects **what was true at 2026-07-05 in that tree** —
> it may be stale, incomplete, or simply wrong. You are expected to **think for yourself**: investigate the
> current code, validate every claim, challenge the recommendations, and form your own plan before you touch
> anything.
>
> - **Re-verify every concrete fact** before relying on it — file paths, line numbers, type/member names, the
>   counts (e.g. "7324 literals", "48/8615/269/1371"), the IVT map, and especially the claim that **no test
>   asserts on the `-Q` output**. Grep/build/read the code as it is *now*; git state and the code will have moved.
> - **Treat the "Decided defaults" and Runbook as a starting position, not a mandate.** If investigation says a
>   different name, constraint, layout, or order is better, do that and explain why. The earlier session had less
>   context than you'll have with the code in front of you.
> - **Discover → validate → then act.** Build it, prove the current state, run the suites to establish a green
>   baseline, and confirm the dependency graph yourself before the first `git mv`.
> - **If reality contradicts this plan, reality wins** — follow the code and update this document to match.
> - Independently sanity-check the *premise* too: is one shared geometry leaf actually the best fix, or has
>   something changed that suggests otherwise? Say so if it has.
>
> ### Working agreement (how to run this)
> - **Track progress with a checklist** — one item per Runbook step (use the task/todo tools); keep it current so
>   the human can see exactly where things stand.
> - **Stop after every step. This is a checkpoint, not a suggestion.** Each Runbook step ends with: report what
>   changed + the green results (builds/suites), then **wait for the human to verify and say go** before starting
>   the next step. Do **not** chain steps unattended.
> - **The moment something is off — stop and ask.** A failing build, a surprising diff, an assumption that didn't
>   hold, an ambiguous decision, reality contradicting this plan: surface it and ask the human. Do not guess, do
>   not silently work around it, do not barrel ahead. A wrong turn on a core-interface refactor is expensive.
> - **Clean up as you go** — remove the cruft the refactor obsoletes (the triplicate `Dimensions`/`Coordinates`,
>   unused `Flatten()`, dead `TestsKernel.Models.Dimensions`, stale comments/docs). Leave the tree cleaner. Keep
>   each cleanup tied to the step you're in and call it out at the checkpoint — don't fold in unrelated sweeps.
> - **Suggest, don't just execute.** When you spot smells, simplifications, or further consolidation beyond the
>   current step, note them for the human at the checkpoint and let them decide — surface the idea rather than
>   acting on it unilaterally.

## Before you start (entry point)
- **Required reading:** this plan **and** its sibling `compact-notation-extraction.md` (the work that led here);
  skim `.agents/docs/README.md` for the repo map. Re-read CLAUDE.md's critical rules.
- **How to run** (verify current, don't assume): commands live in `.agents/docs/commands.md`. Today:
  `dotnet build Binacle.Net.slnx`; `./config/tests.sh {lib,api,vipaq,performance}`; TS via
  `cd vipaq/binacle-vipaq && npm test` (note: that path **moves to `vipaq/packages/binacle-vipaq`** in Runbook
  step 1) and `npm install` at root. Confirm these against `commands.md` as-is.
- **Establish a green baseline first** — build + run every suite before the first change, so any later red is
  clearly yours.
- **Committing is the human's job, never yours** (CLAUDE.md). Where the Runbook says "commit," it means: you stop
  at the checkpoint, the human verifies and commits. You stage/commit nothing. You *may* create the working
  branch, but leave all changes in the tree for the human.

## Scope & definition of done
**In scope:** unify the geometry *types* — the `IWith*` interfaces + concrete `Dimensions`/`Coordinates` — into
`Binacle.Geometry`, rewire consumers, and finish compact-notation Phase 3 (formatters → `[Q]`).
**Out of scope / behaviour-preserving:** do **not** change algorithm behaviour, wire formats, result-selection, or
public API/response semantics; do **not** unify the per-algorithm `Bin`/`Item` models beyond geometry; do **not**
modify v3. The only intended *observable* change is log/UI output `-Q`→`[Q]`.
**Done when:** one `Dimensions`/`Coordinates` and one `IWith*` family across the repo; `CompactNotationFormatter`
is the single formatter (lib `Format*` deleted); `[Q]` everywhere; all suites + TS + docker green; `/code-review`
passed; docs + both plans updated/closed.

**Goal.** Extract the common geometry types — the `IWith*` interfaces and the concrete `Dimensions` / `Coordinates`
models — into **one shared leaf project, `Binacle.Geometry`** (name decided), and use it across `Binacle.Net`,
`Binacle.Lib`, `Binacle.ViPaq`, the shared notation, and the tests. Today those types are duplicated as **two
parallel interface families**, which is the one thing blocking the last step of the compact-notation work.

---

## Naming decision (DECIDED — `Binacle.Geometry`)

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

**DECIDED: `Binacle.Geometry`.** (Kept the Kernels; `Core` rejected to avoid the kernel clash, `Domain` rejected
because `ServiceModule.Domain` exists. Table kept above as the rationale.)

## Companion cleanup — placement convention (scope first, then language)

Do this **with** (or just before) the leaf extraction, so `Binacle.Geometry` lands under the right layout from
day one.

**The rule.** Decide each project/package by *scope*, then by language:
1. **Cross-slice** (used by more than one slice): **C#** → `shared/` · **TS/JS** → root **`/packages`**.
2. **Slice-specific** (one slice only): **C#** → `<slice>/src` (libs) + `<slice>/test` (tests) · **TS/JS** →
   `<slice>/packages`.
3. Within a C# home, `src/` = library code, `test/` = tests **and** test-support.

Applying it:

| Project / package | Lang | Scope | Home | Move? |
|---|---|---|---|---|
| `Binacle.Geometry` (new) | C# | cross-slice lib | `shared/src/Binacle.Geometry` | new |
| `Binacle.CompactNotation` | C# | cross-slice lib | `shared/src/Binacle.CompactNotation` | from `shared/` |
| `Binacle.TestsKernel` | C# | cross-slice test-support | `shared/test/Binacle.TestsKernel` | from `shared/` (see flag) |
| `Binacle.CompactNotation.UnitTests` | C# | cross-slice test | `shared/test/…` | from `shared/` |
| `binacle-compact-notation` | TS | common (notation mirror) | **root `/packages`** | **stays** |
| `binacle-net-ui`, `cookies`, `theme-switcher` | TS | cross-slice (UI/docs/web) | root `/packages` | **stay** |
| `binacle-vipaq` | TS | vipaq-only | `vipaq/packages/binacle-vipaq` | **moves** |
| `shared/data` | data | — | `shared/data` | unchanged |

Net: at root `/packages` **nothing moves**; the only TS move is `binacle-vipaq` → `vipaq/packages/`. The shared C#
projects get the `src`/`test` split. (Note the C# and TS notation mirrors deliberately live apart —
`shared/src/Binacle.CompactNotation` vs `/packages/binacle-compact-notation` — because the rule routes by language.)

**TestsKernel placement — checked, no smell; name kept.** It legitimately *is* a kernel — the shared foundational
infra kerneling the **`Binacle.Lib` and `Binacle.Net` test suites** (just as `Binacle.Net.Kernel` kernels the API
host). Scope is exactly those two: **vipaq** (Bogus fakers) and **CompactNotation** tests are self-contained — no
shared kernel — so its reach is deliberately lib + .net, not repo-wide. "Kernel" = per-layer shared infra; `Binacle.Geometry` is the *cross-layer domain model*, a
different axis — so no conflict, and `TestsKernel` keeps its name. Direction is correct too: `TestsKernel` references
`lib.Abstractions` (test-support → lib), and `lib.Abstractions` *grants* it access via
`<InternalsVisibleTo Include="Binacle.TestsKernel"/>` (alongside `Binacle.Lib`, `Binacle.Lib.UnitTests`). So
`shared/test/Binacle.TestsKernel` is fine.

What that IVT is actually for (mapped — it is **result-building, not geometry**): lib.Abstractions' internal
members are all in `Algorithms/Models` — `OperationResult()` ctor + `Status`/`EarlyExitReason` internal setters,
and internal ctors of `PackedBin` / `PackedItem` / `ResultItem` / `UnpackedItem`. `TestsKernel` uses
`new OperationResult()` + `.Status` and `new PackedBin(...)` in `ResultSelection/Helpers/OperationResultHelper.cs`.
**Implication for this plan:** those internal ctors stay in lib.Abstractions, so extracting geometry into
`Binacle.Geometry` does **not** require touching the IVT — it stays put.

**The real coupling the leaf must resolve:** there are **three concrete `Dimensions`** (and `Coordinates`) today —
`Binacle.Lib.Abstractions.Models.Dimensions` (consumed by the internal `ResultItem`/`PackedItem` ctors),
`Binacle.TestsKernel.Models.Dimensions`, and `Binacle.CompactNotation.Dimensions<T>`. `PackedBin`'s internal ctor
takes `IWithReadOnlyDimensions`. After the move, `lib.Abstractions` references `Binacle.Geometry`, those internal
ctors take the unified `Geometry` types, and callers (`TestBin` etc.) implement the unified interface — the
construction calls keep working.

What it ripples into (miss one → build/CI breaks — do it as its own verified commit):
- `Binacle.Net.slnx` — solution folders (`/shared/` → `/shared/src/`, `/shared/test/`; add `/vipaq/packages/`) +
  every `<Project Path>` for the moved projects.
- Every `.csproj` `ProjectReference` relative path (shared move breaks refs from `lib/test`, `api/test`,
  `lib.Abstractions`→`TestsKernel`, and `TestsKernel`/vipaq→`CompactNotation`).
- npm `workspaces` in root `package.json` — `vipaq/binacle-vipaq` → `vipaq/packages/binacle-vipaq` (the `packages/*`
  glob is unchanged since root packages stay put); `binacle-vipaq`→`binacle-compact-notation` still resolves after
  the move (re-run `npm install`).
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

## Decided defaults (apply unless vetoed)
1. **Leaf name** — `Binacle.Geometry`. ✅ decided.
2. **`struct` constraint** — standardize on `where T : struct, INumber<T>` everywhere (all real `T` are `int`/`long`;
   matches CompactNotation; kills `CS8618`).
3. **Quantity type** — `int`, via a **non-generic `IWithQuantity { int Quantity { get; } }`** (a count is a count,
   independent of the coordinate `T`). Reconcile with CompactNotation's current `IWithQuantity<T>` during step 3.
4. **`Item<T>` / `DimensionsAndQuantity<T>`** — **stay in `CompactNotation`** (notation concepts). The leaf holds only
   `IWith*` + `Dimensions` + `Coordinates`.
5. **Lib's non-generic `int` interface aliases** — **keep as thin shims that extend the leaf's** interfaces (avoids a
   repo-wide call-site sweep). *This is the main effort lever — shims (less churn) vs. full sweep (cleaner, bigger
   diff). Default = shims; confirm if you'd rather sweep.*

## Runbook — execution order (each step = one checkpoint, ends green)
Each step ends green and stops for the human to verify **and commit** (you never commit — CLAUDE.md).
0. **Prereq** — the human commits the Phase 2 work sitting in the tree; then work on a branch.
1. **Layout convention** *(own commit)* — `git mv` shared C# projects → `shared/src` + `shared/test`,
   `binacle-vipaq` → `vipaq/packages`; fix `.slnx` folders + project paths, every `.csproj` `ProjectReference`,
   npm `workspaces`, `config/*.sh`, `gulpfile`, `Dockerfile`/`build.sh`, agent docs. **Verify:** full build + all C#
   suites + `npm install` + TS suites + docker build.
2. **Create `Binacle.Geometry`** (`shared/src`) — interfaces (read-only + mutable split, `struct` constraint,
   non-generic `int` shims) + concrete `Dimensions`/`Coordinates`. Build the leaf alone.
3. **Point `CompactNotation` at the leaf** — reference it; delete its duplicate `IWith*`/`Dimensions`/`Coordinates`;
   `Item<T>`/`DimensionsAndQuantity<T>` stay. **Verify:** CompactNotation + vipaq suites.
4. **Rewire `lib.Abstractions`** (the big sweep) — its `IWith*` extend the leaf's; concrete `Dimensions`/`Coordinates`
   come from the leaf; watch the `IWithDimensions<T>` name collision. **Verify:** lib + all lib tests.
5. **Rewire the rest** — `Binacle.Net` (API/UIModule/Diagnostics), `ViPaq` src, `TestsKernel`, remaining tests.
   **Verify:** everything.
6. **Finish compact-notation Phase 3** (now trivial) — models satisfy the formatter → swap the 5 `Format*` sites,
   delete lib `FormatDimensions`/`FormatCoordinates`, migrate UIModule `ParseItem` + `sample_data.json`. Output →
   `[Q]`. **Verify:** api suite.
7. **Cleanup + review** — wire-or-drop `Flatten()`, remove dead `TestsKernel.Models.Dimensions` (done in step 5),
   optional TS parity; full green gates; **`/code-review`**; update docs and close out both plans (this one +
   compact-notation Phase 3).
   - **`[Migrate-Review]` tag** — grep the repo for `[Migrate-Review]` to find the deferred cleanups marked in-code
     (the model/contract implementers touched by the migration). What to do at each: (a) drop the redundant explicit
     `Binacle.Geometry.IWith*<int>` on the api/UIModule contracts once lib's `IWithDimensions` shim is made to reach
     the mutable leaf generic (like `IWithCoordinates`/`IWithQuantity` already do); (b) drop the vipaq test
     `Dimensions<T>`/`Coordinates<T>` clones in favour of the leaf's (keep only the `Create` factory helpers);
     (c) review the `TestBin`/`TestItem` ctor overloads (`IWithReadOnlyDimensions` vs `Binacle.Geometry.IWithDimensions<int>`)
     and the vipaq `Bin`/`Item` wiring. These are quality-only and were deferred so the migration commits stay focused.

---

## Follow-on consolidation & what remains (2026-07-06)

After the 7-step runbook, a model-consolidation pass ran. The leaf is now the shared home for the concrete
data-holder models **and** the full `IWith*` family.

**Done (post-runbook):**
- **Tier 1** — one shared `Binacle.Geometry.Item<T>` (dims+coords) replaced the duplicate `ViPaq.Item<T>` +
  `CompactNotation.Item<T>`; vipaq's `Bin<T>` collapsed onto `Geometry.Dimensions<T>`.
- **`GeometryFactory`** — `GeometryFactory.Dimensions(...)` / `.Coordinates(...)` type-inference helpers in the leaf;
  the vipaq test `Dimensions<T>`/`Coordinates<T>` clones were deleted.
- **Interface family moved into the leaf** — the non-generic `int` shortcuts (`IWith[ReadOnly]Dimensions`/
  `Coordinates`/`Quantity`) **and** the volume interfaces now live in `Binacle.Geometry` (volume generic tightened to
  `struct, IBinaryInteger<T>`); the `IWithDimensions` asymmetry is fixed. `IWithID`/`IWithReadOnlyID` stay in lib.
  Consumers reach the leaf via a per-project `<Using Include="Binacle.Geometry"/>`.
- **Idea 1** — v4 + UIModule contracts dropped their redundant explicit `Binacle.Geometry.IWith*<int>`.
- Invariant held throughout: **lib ⊥ vipaq** (each references only the leaf). All C# suites green.

**What remains (all optional / deferred; PARKED — nothing in progress; grep `[Migrate-Review]`):**
- **v3 contracts** — **DONE (2026-07-06, human-authorized).** Dropped the redundant explicit
  `Binacle.Geometry.IWith*<int>` from `v3/Bin.cs` + `v3/PackResponse.cs` `PackedBox`. Behaviour-neutral
  (marker-interface removal only — JSON shape/serialization unchanged); full build + api suite (269) green.
- **Log formatter** (`DiagnosticsModule/LogProcessorHandlingExtensions`) — formats `PackedItem`/`UnpackedItem` as
  separate sub-parts because those result models expose geometry as nested value structs, not by implementing the
  interfaces. **Tier-2-lite fix (self-contained):** have the result-model CLASSES implement `IWithReadOnly*` by
  delegating to their existing structs (`Length => this.Dimensions.Length` — zero extra allocation; the perf value
  structs stay structs, see the value-struct note), then the formatter collapses to a single `Format<int>(item)`.
  Does NOT need the full `IWithID`-move / rename.
- **`TestBin`/`TestItem`** — redundant ctor overloads (`IWithReadOnlyDimensions` vs `Geometry.IWithDimensions<int>`);
  simplify, or fold into Tier 2.
- **`DimensionsAndQuantity.Flatten()`** — no production consumer; kept + tagged (wire it or drop it).
- **Tier 2 (the big lever, not started)** — move `IWithID` into the leaf + a shared **ID-carrying** model family
  (`Bin`=id+dims, `Item`=id+dims+quantity, `PackedItem`=id+dims+coords) to collapse UIModule Models + `TestBin`/
  `TestItem` and let the log formatter simplify. Needs the **leaf-rename** decision (`Geometry` → `Primitives`/`Core`,
  or a `Binacle.Models` layer) and settles **quantity-as-`int`-vs-`T`**. A shared dims+quantity model is only worth
  it inside Tier 2.
- **TS parity** — deferred: TS is structurally typed, so the duplicated shapes already interoperate; low ROI.
- **Verification gaps** — the ServiceModule suite (needs Azurite) and a docker image build were skipped by choice;
  run them for a full green sweep.

**Won't reduce (leave as-is):** lib internal result models (internal ctors, immutable), algorithm working types
(carry behaviour), v3 DTOs (frozen), UIModule ViewModels (DataAnnotations + computed ID), lib readonly-struct
`Dimensions`/`Coordinates` (value semantics).
