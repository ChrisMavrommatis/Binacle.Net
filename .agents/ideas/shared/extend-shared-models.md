---
description: take the shared model leaf further
paths:
  - "shared/**"
---

# Idea: take the shared model leaf further

**Status:** Unvetted idea. Parked leftovers from the finished `Binacle.Geometry` extraction.

The extraction itself is done: one `Binacle.Geometry` leaf holds the `IWith*` family and the concrete
`Dimensions<T>` / `Coordinates<T>` / `Item<T>`; `CompactNotationFormatter` is the single formatter; lib ⊥ vipaq.
What follows is what we deliberately did **not** do.

## 1. Move identity into the leaf — recommendation: don't, unless bundled

**Resolved 2026-08-13 — this item is done.** `IWithID` / `IWithReadOnlyID` and the read-only markers
`IIdentifiableBin` / `IIdentifiableItem` now sit in `shared/src/Binacle.Packing/Abstractions/`. That is the
"add a layer above the leaf" option this item named, and no leaf rename was needed. They are **not geometry**,
so they did not go into `Binacle.Geometry`. The original reasoning is kept below.

Moving them only pays off if a leaf-only consumer (vipaq, say) later needs identity. And a shared ID-carrying
model family (`Bin` / `Item` / `PackedItem`) drags in two more decisions:

- Rename the leaf? `Geometry` → `Primitives` / `Core`, or add a `Binacle.Models` layer above it.
- Is quantity an `int` or a `T`?

Only worth doing as one bundle. Piecemeal, it is churn.

## 2. TypeScript parity — low ROI

The TS side duplicates the model shapes. TS is structurally typed, so the duplicates already interoperate.
Nothing is broken; there is just no single source. Do it if the shapes start drifting.

## Won't reduce — leave alone (this is settled, not an open question)

lib internal result models (internal ctors, immutable) · algorithm working types (they carry behaviour) ·
v3 DTOs (frozen) · UIModule ViewModels (DataAnnotations + computed ID) · lib **internal** readonly-struct
`Dimensions` / `Coordinates` (value-type performance — they must stay structs).

## Related

- the shared-slice doc
