# Binacle.Geometry — one home for the common models

**Status (2026-07-06): DONE — only optional/parked items remain.** The geometry leaf and the follow-on
model-consolidation both landed. One shared `Binacle.Geometry` leaf holds the `IWith*` family (generic + `int`
shortcuts) and the concrete `Dimensions<T>`/`Coordinates<T>`/`Item<T>`; `CompactNotationFormatter` is the single
formatter (lib `Format*` deleted, and the runtime-polymorphic `Format<T>` is now gone too — only the typed
primitives/composites remain); `[Q]` everywhere; lib ⊥ vipaq (each references only the leaf). All C# suites + TS
green.

The former open tags are all resolved and no longer in the code (grep confirms zero `[Migrate-Review]` /
`[CompactFormatterDecision]`):
- **`[CompactFormatterDecision]` — DONE.** `CompactNotationFormatter.Format<T>` was deleted; the log echo now uses
  the typed composites (`FormatDimensions` / `FormatItem` / `FormatDimensionsAndQuantity`).
- **`[Migrate-Review]` — DONE.** `DimensionsAndQuantity<T>.Flatten()` is now `internal` (test-only); the redundant
  `TestBin`/`TestItem` ctor overloads were dropped (each now has just the default + one
  `Binacle.Geometry.IWithDimensions<int>` ctor).

## What remains (all optional / parked)

- **Tier 2 — move `IWithID`/`IWithReadOnlyID` into the leaf (PARKED).** Recommendation: **keep them in
  `Binacle.Lib.Abstractions.Models`**. They are not geometry; moving them only pays off if a leaf-only consumer
  (e.g. vipaq) later needs identity. Note the read-only identity markers `IIdentifiableBin` / `IIdentifiableItem`
  now live in `lib.Abstractions.Models` (`IWithReadOnlyID` + read-only geometry [+ quantity]) — the packing log
  reads through them. A shared ID-carrying model family (`Bin`/`Item`/`PackedItem`) would also need the
  leaf-rename decision (`Geometry` → `Primitives`/`Core`, or a `Binacle.Models` layer) and the
  quantity-as-`int`-vs-`T` call. Only worth it as a bundle.
- **TypeScript parity — deferred.** TS is structurally typed, so the duplicated shapes already interoperate; low ROI.
- **Verification gap — docker image build not run.** The C# suites (incl. ServiceModule) and TS suites pass; a
  docker image build was skipped by choice. Run it for a fully green sweep.

**Won't reduce (leave as-is):** lib internal result models (internal ctors, immutable), algorithm working types
(carry behaviour), v3 DTOs (frozen), UIModule ViewModels (DataAnnotations + computed ID), lib **internal**
readonly-struct `Dimensions`/`Coordinates` (value-type perf — must stay structs).
