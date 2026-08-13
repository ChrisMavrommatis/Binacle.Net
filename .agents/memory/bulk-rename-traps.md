---
name: bulk-rename-traps
description: Four traps when sweeping a namespace or type rename across this repo — spaces in Algorithms/ folder names, unstaged files and git mv, global usings that collide, and fully-qualified names
type: gotcha
when: sweeping a namespace or type rename across the repo
---

Four things that go wrong when renaming a namespace or moving a type across this repo. All four were hit for
real during the `Binacle.Packing` extraction and the tests-kernel split.

**`grep -rl ... | xargs sed` silently skips `lib/src/Binacle.Lib/Algorithms/`.** Those directory names contain
spaces — `Best Fit Decreasing v1`, `Worst Fit Decreasing v2` and four more — so `xargs` splits the paths and
30 files are never touched. No error, no output, just an incomplete rename that builds fine until something
downstream fails. Use `xargs -d '\n'`, `-print0`/`-0`, or a script that quotes paths.

**Stage new files before moving a directory.** A file created earlier in the same session and never `git add`ed
is invisible to `git mv`, and an `rm -rf` of the emptied directory takes it with it.

**A global using is not always safe.** `<Using Include="Binacle.Packing" />` collides with
`Binacle.Net.UIModule`'s own `Models.PackedItem` / `UnpackedItem`, and with the api's `v3.Contracts.Algorithm`
and `v4.Contracts.Algorithm`. Those two projects need per-file usings; the other nine took the global cleanly.

**Fully-qualified names do not move themselves.** Sites that write `Binacle.Lib.Algorithm` or `Lib.Algorithm`
outright are invisible to any using-line sweep. Grep for the bare type name too, not just the using.

**Why:** each one fails quietly rather than at the compiler, so the rename looks done when it is not.

**How to apply:** after any bulk rename, grep for the old name across the whole repo — including `.csproj`
files — and build clean before believing it. If embedded resources are involved, also check the manifest with
`strings <dll> | grep <prefix>`, because a broken `LogicalName` fails silently at run time rather than at build.
