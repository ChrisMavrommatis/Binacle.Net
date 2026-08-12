---
description: Open question - now that Binacle.TestsKernel is lib-free, is there still a reason to split it? The original reason is gone.
---

# Should `Binacle.TestsKernel` still be split?

**Status:** Open question, not scheduled work. **The original reason to split it no longer exists**, so this is a
smaller and weaker question than it was. Nobody has decided it. It may belong in `ideas/` rather than here.

**This is not the old "move the kernel into lib/test" plan.** That plan existed because
`shared/test/Binacle.TestsKernel` referenced `lib/src/Binacle.Lib.Abstractions` - the only upward dependency in
the repo - and moving the project was the cheap way to delete that edge. The 2026-08-13 packing-contract
extraction removed the edge at its cause instead: the types the kernel needed moved down into
`shared/src/Binacle.Packing`. **The kernel is now lib-free where it stands**, referencing only `Binacle.Packing`
and `Binacle.CompactNotation`, both in `shared/src`. There is no longer any architectural reason to move it, and
the move is off the table.

## What is actually left to ask

Only this: the kernel serves two audiences that do not overlap, and one assembly holds both.

- **`Algorithms/`** - the Bischoff-suite and custom-problems scenario providers. Used by the api integration
  suite (43 `using` lines across 27 files) and by all three lib test projects.
- **`ResultSelection/`** - the best-algorithm, best-bin and smallest-bin scenarios. Used by
  `Binacle.Lib.UnitTests` and `Binacle.Lib.Benchmarks` only. **Zero api uses.**

So the api test suite loads an assembly whose result-selection half it never touches. That is the entire
remaining case for splitting, and it is a tidiness argument, not a dependency one. Weigh it against the costs
below before spending anything on it.

## Costs, measured - these still hold

Three findings from the original investigation. They were about splitting along the lib boundary, but two of
them apply to any split at all.

- **One assembly, one manifest.** `Files/EmbeddedResourceFile.cs:34` and `Files/EmbeddedResourceFileProvider.cs:19`
  both call `Assembly.GetExecutingAssembly()`, so a reader only ever sees resources embedded in its own
  assembly. One assembly currently serves both manifest prefixes
  (`Binacle.TestsKernel.Algorithms.Data.` and `Binacle.TestsKernel.ResultSelection.Data.`). Two assemblies means
  two readers, or a changed `IFile`.
- **`ResultSelection` needs friend access.** `ResultSelection/Helpers/OperationResultHelper.cs:26,55` constructs
  `PackedBin` and `OperationResult`, both of which have internal constructors. `Binacle.Packing.csproj` grants
  `InternalsVisibleTo` to `Binacle.TestsKernel` for exactly this. A split half would need its own grant.
- **The assembly name is load-bearing.** Five `LogicalName` values, two hardcoded manifest prefixes in the
  readers, the friend grant above, and the `-Binacle.TestsKernel` coverage filter in `tooling/coverage.just:59`
  all name it. Any split that renames an assembly touches all of them, and a broken manifest name fails
  silently - verify with `strings <dll> | grep Binacle.TestsKernel.Algorithms.Data.BischoffSuite`.

## The data does not move, whatever is decided

The fixture corpus lives in `shared/data/` and is embedded **by link**, not copied - one place it is generated
and edited. Two of the four folders have a reader outside the kernel entirely, so the corpus is genuinely shared
and cannot follow the kernel anywhere:

| Folder | Read by |
|---|---|
| `or-library/` | only the OR-Library converter, which embeds the seven `thpack*.txt` |
| `bischoff-suite/` | the kernel, by embed; and the ViPaq packed-data generator, by path at run time |
| `custom-problems/` | the kernel, by embed; and the ViPaq packed-data generator, by path at run time |
| `result-selection/` | the kernel, by embed. Consumed only by lib unit tests and benchmarks |

## Related but separate

- **The repository-root locator is not part of this.** `RepositoryRoot` / `RepositoryRootLocator` live in
  `shared/test/Binacle.TestReporting`, not in the kernel. They climb from the running binary to the folder
  holding `Binacle.Net.slnx`. Used by the OR-Library converter, both performance-test suites and both ViPaq
  tools. `Binacle.TestReporting` has no project reference of its own and is unaffected by anything here.
- **Growing the fixture cases** is a different plan in this same folder, about coverage rather than structure.

## Watch out

- **Never commit.** Leave changes in the working tree for the human.
