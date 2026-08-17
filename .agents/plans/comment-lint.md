---
description: A check that nothing outside the agent guidance directory points a reader into it.
---

# Comment lint - stop code pointing at agent guidance

**Status:** Not built. Split out of the architecture work on 2026-08-17, where it had been folded in because
both were "checks" - which is not a relationship. It shares no design, no tool and no file with that plan.

**All 27 known violations were fixed on 2026-08-13.** So this lands green on day one, which is the state a new
check wants. **Do not go looking for sites to fix.** Confirm it catches nothing, then keep it that way.

## What it enforces

`CLAUDE.md` states the rule: nothing outside `.agents/` may point a reader into it - not a filename, not a `$`
reference, not a bare ref code. The rule has never had a check behind it, which is why 27 sites accumulated
without anyone noticing.

## Why it needs three arms

The rule is phrased as a ban on pointing at a directory, so everyone greps for the directory name. **Almost
none of the real violations contain it.** Measured on 2026-08-13: 14 name a guidance file with the path filed
off, 2 use the `$` reference scheme, and **11 name no file at all** - a bare ref code like `D16`, which means
nothing to a reader and gives them no way to find out what it was.

So three arms, each blind to the other two:

1. **Filenames.** Every `.md` basename under `.agents/`.
2. **`$` references.** Matched against the ids the docs actually declare - a generic `$word` pattern hits every
   shell variable and every minified asset in the repo, which is thousands of lines of noise.
3. **Bare ref codes.** Matched against the `D1…`/`O1…`/`F1…` headings that define them.

Arm two is what caught `api/src/Binacle.Net/v4/Contracts/ExampleData.cs` and `Directory.Packages.props`. Arm
three caught 11 sites neither of the others can see. **Build all three or the check ships with a hole a whole
file type has already fallen through.**

## Derive every list. Never hardcode one.

A hardcoded list needs a second mechanism to tell you when it has rotted. **Derivation is that mechanism, for
free** - a doc added tomorrow is covered the day it lands, and a name that gains a twin elsewhere drops out by
itself.

That was measured wrong once and the wrong answer nearly stuck: deriving from every guidance basename was
measured at 94 hits against 15 for a hand-written list, and dismissed. Re-measured, those 94 came from two
things - `README.md`, which exists all over the repo, and the published sites legitimately linking their own
pages. **Derive the exclusions too**: drop any basename that also exists outside `.agents/`, and the list comes
out at 77 names producing 13 files and 14 lines, every one a real violation. Zero noise.

That derivation yields `README.md`, `presets.md` and `_index.md` today. **Do not write those three down as a
list** - that is the hardcode coming back in through the side door.

**`CLAUDE.md` is exempt as a file.** It is the declared door into the guidance directory and it names both a
path and a bare ref code on purpose.

**Assert the derived lists are non-empty, loudly.** An empty list makes the check report clean forever. This is
the single most likely way it dies.

## Scan comment text, never whole lines

This is the finding that shapes arms two and three, and it survives matching against the derived lists - it is
not the noise problem above.

- **In C#, `:D16` and `:F2` are number-formatting instructions**, which is exactly the shape of a ref code.
  `VipaqProtobufSizeComparisonTest.cs` and `CodecCompressionCrossoverTest.cs` both carry one.
- **A shell's `"$packages"` is exactly the shape of a `$` reference**, and `packages` is a declared id.
  `tooling/image.just` uses the variable twice.

Both are code rather than comment, so **restricting each arm to the commentary on a line removes all four with
no allow-list.** An allow-list here would have been four entries that rot silently.

**How to extract it:** one small `awk` program per comment family - C-like (`//`, `///`, and `/* */` with a
line-to-line flag), hash-like (`#`, for just recipes, shell and yaml), and XML-like (`<!-- -->`, for `.props`
and `.csproj`). Feed their output to grep instead of the raw files.

`awk` rather than `sed` because the C-family block form needs state across lines, and **a violation sitting
inside a `/* */` block is exactly the hole a one-line `sed` leaves.** The repo already parses front matter with
`awk` in `tooling/agents.just`, so the idiom is there to copy.

**Scan every tracked file type, not just `.cs` and `.ts`.** `Directory.Packages.props` carried one. A scan
limited to source files exempts a whole file type. Skip vendored bundles under `assets/lib/` and any `.map`.

## Build it as a `just` recipe, not Semgrep

The whole job is regex over comment text. Semgrep means a Python toolchain, a new CI job, a committed generated
artifact and a generator recipe, to do what `grep` already does. A recipe under `tooling/` plus a step in the
PR gate is the same check in the repo's existing idiom. **Adopt Semgrep the day a rule appears that a regex
genuinely cannot express.**

**Do not reach for `xargs`.** Two traps were recorded against the earlier attempt - it returns 123 when any
grep batch matches nothing, and six algorithm folders have spaces in their names that plain `xargs` splits.
**Both are `xargs` artifacts.** Nothing in this repo uses it, and `grep` searching a directory directly has
neither problem.

## Watch out

- **Nothing is committed by an agent.** Leave every change in the working tree.
