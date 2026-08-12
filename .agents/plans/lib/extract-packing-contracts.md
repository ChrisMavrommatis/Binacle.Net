---
description: Aftermath of the Binacle.Packing extraction - the boundary file and four prose files still describe a project layout that no longer exists.
---

# Finish the `Binacle.Packing` extraction

**Status:** The code is done and green as of 2026-08-13, in the working tree, uncommitted. What is left is the
paperwork: one boundary file and four prose files still describe the old shape. Delete this plan when they are
all corrected.

## What changed, in one paragraph

`Binacle.Lib.Abstractions` was three things wearing one name: pure geometry, the packing vocabulary, and the
packer's engine interfaces. The geometry half (`DimensionExtensions`) moved down into `Binacle.Geometry`; the
vocabulary became a new `shared/src/Binacle.Packing` project; the engine interfaces were folded into
`Binacle.Lib` and the Abstractions project was deleted. The stack is now
`Binacle.Geometry -> Binacle.Packing -> Binacle.Lib`, `lib/src` holds a single project, and **the repo has no
upward dependency**: nothing under `shared/` references `lib/` or `api/`. In the api slice only `Binacle.Net`
itself names the packer - the Kernel, both modules and the integration suite are all off lib.

Verified at each step: full solution build 0 warnings 0 errors, lib unit tests 8679 passed, api core integration
622 passed, and all five embedded fixture manifest prefixes intact in the built kernel assembly.

## Left to do

### 1. The boundary file is now wrong - but it is not this plan's to fix

`architecture.yml` at the repo root still declares an upward edge that no longer exists, plus an `api/test -> lib`
edge that is also gone. **That file has its own master plan, in the general plans folder** - the one about
stating the repo's boundaries so tools can read them. The exact edits are listed there, so they are not repeated
here; a fact in two plans disagrees within a week. This entry exists only so the correction is not forgotten,
and so nobody starts the boundary tooling against a file describing the old shape.

### 2. Four prose files describe a project that no longer exists

None were touched, on purpose - they are writing, not mechanical edits.

- **`lib/README.md:10`** has a table row for `src/Binacle.Lib.Abstractions`, "Interfaces only - shared by
  `Binacle.Lib` and the API layer. No dependencies." That project is gone; `lib/src` holds only `Binacle.Lib`.
- **The lib slice's dependency doc** and **the shared slice's dependency doc** in the agent reference layer both
  describe the old graph.
- **The parked idea about taking the shared model leaf further.** Its first item argues against moving
  `IWithID` / `IWithReadOnlyID` / `IIdentifiableBin` / `IIdentifiableItem` out of `Binacle.Lib.Abstractions`
  unless bundled with a leaf rename. That is now resolved: they went to `Binacle.Packing`, which is the
  "add a layer above the leaf" option the idea itself named, and no leaf rename was needed. Mark item 1 done.

### 3. The reasoning belongs in the design layer, not here

This plan gets deleted; the following is durable and should be lifted into the lib or shared design layer first.
Recorded compactly so it survives the deletion.

- **Why the line falls where it does.** `Binacle.Geometry` holds what the notation and the wire format need;
  `Binacle.Packing` holds what only the packer and its consumers need. This is a fact about usage, not taste:
  `IWithQuantity` is used by `Binacle.CompactNotation`, the OR-Library converter and the notation unit tests -
  consumers that never touch packing - so it is correctly in Geometry. `IWithID` is used by none of them, only
  by `api/src` (19 files), `lib/src` (46), `lib/test` (1) and the fixture kernel (2). That is why identity is
  packing vocabulary and not geometry.
- **Why there is no separate abstractions assembly.** After the extraction, the engine interfaces had exactly
  one direct consumer, `Binacle.Lib` itself; `api/src/Binacle.Net` resolves them transitively and every test
  project referenced `Binacle.Lib` only. Nothing in the repo is packable - no `IsPackable`, `PackageId` or
  `GeneratePackageOnBuild` anywhere - so the contract-without-implementation case does not apply. Publishing
  would not change it either: the package an external consumer would want *is* `Binacle.Packing`, the result
  types they read back.
- **What the collapse cost.** `IPackingAlgorithm` could not name a concrete algorithm before - separate
  assembly, one-way reference, enforced by the compiler. Now only convention stops it. That is the sharpest
  candidate rule for any type-level architecture tool: types in namespace `Binacle.Lib.Abstractions` may not
  reference types outside it.
- **An unused reference is not a dead reference.** `Binacle.Net.Kernel` referenced `Binacle.Lib.Abstractions`
  and no file under it named `Binacle.Lib` - but `Binacle.Net.DiagnosticsModule` was resolving four types
  *through* it with no reference of its own. Removing it surfaced an undeclared dependency rather than creating
  one. "No file in this project names the assembly" proves the reference is unused by that project, not that it
  is unused; only a build after removal proves that.

## Watch out if any of this is redone

- **`sed` skips `lib/src/Binacle.Lib/Algorithms/`.** Those directory names contain spaces
  (`Best Fit Decreasing v1`), so `grep -rl ... | xargs sed` silently misses 30 files. Use a script that quotes
  paths.
- **Stage new files before moving a directory.** A file created earlier in the same session and never staged is
  invisible to `git mv`, and an `rm -rf` of the emptied directory takes it with it.
- **A global using is not always safe.** `<Using Include="Binacle.Packing" />` collides with
  `Binacle.Net.UIModule`'s own `Models.PackedItem` / `UnpackedItem` and with the api's `v3.Contracts.Algorithm`
  and `v4.Contracts.Algorithm`. Those two projects use per-file usings; the other nine took the global cleanly.
- **Fully-qualified names do not move themselves.** Four sites wrote `Binacle.Lib.Algorithm` or `Lib.Algorithm`
  outright, which no using-line sweep can see.
- **Never commit.** Everything is in the working tree for the human.
