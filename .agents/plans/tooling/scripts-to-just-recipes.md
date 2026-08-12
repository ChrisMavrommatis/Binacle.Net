# Convert the last `tooling/*.sh` scripts to `just` recipes

**Status:** Not started. Split out of `ci-shared-scripts` on 2026-08-07, and deliberately not named `ci-`
anything: **CI runs none of these.** They gate nothing and no workflow calls them. This is about
discoverability, and nothing else.

Every script CI cares about has already moved. Tests, coverage, the OpenAPI documents, the agent indexes,
running things from source, the build and the image stacks are `just` modules under `tooling/`; setup is
`just install` / `just assets` in the root justfile.

Five are left:

- `tooling/benchmarks.lib.sh`
- `tooling/benchmarks.vipaq.sh`
- `tooling/performance.lib.sh`
- `tooling/performance.vipaq.sh`
- `tooling/tmux.sh`

## Why bother, given they work

`just --list` answers "what can I run here", and recipe names complete on tab. Nothing in `tooling/` completes
anything, so these five are findable only by knowing they exist. That is the whole benefit - it is real, but it
is small, and this plan should not be allowed to grow past it.

**Open question: `tmux.sh` may be the one that stays.** It builds a session and attaches. No arguments, nothing
to parameterise, and a recipe that only calls a script is two files where there was one. If it stays, say so in
a line at the top of the script and delete it from this list.

## How, from the moves that already landed

These are the lessons from converting everything else. They are worth following rather than rediscovering.

- **Absorbed, not wrapped.** The recipe runs the tool directly and the script is deleted. A recipe that only
  calls a script keeps the drift it was meant to remove. A script that is a program rather than a command line
  still counts as absorbed when it moves into a shebang recipe body whole - that is how the 103-line
  `agents-index.sh` moved.
- **An alias list becomes a parameter whose `case` rejects an unknown value.** The launch profiles
  (`N|S|U|All`) went in that way, and `Encode|Decode` is the same shape. Without the reject, a typo falls
  through to the default and reports a green run for something nobody asked for.
- **One module per job, not per script.** Recipes that answer different questions do not share a module just
  because their scripts sat in the same folder. Where two modules need the same few lines, copy them - one
  reaching into another restores the coupling the split removed.
- **Module recipes need `set working-directory := '..'`**, and a tool that resolves paths itself (MSBuild
  resolves a relative output directory against the project, not the caller) needs an absolute path passed in.
- **When the moved script is a generator, prove the move by diffing its output**, not by its exit code.

## Watch out

The benchmark and performance harnesses write results, and `results/` is a hand-curated vault - harnesses write
to gitignored scratch, never straight into it. Do not let a recipe change where output lands.

## Six stale references clear themselves when this lands

`lib/README.md` (2), `results/lib/README.md` (2), `results/lib/benchmarks/README.md` and
`results/lib/efficiency/README.md` all give `./tooling/performance.lib.sh` or `./tooling/benchmarks.lib.sh` as
the command to run.

Those are the only places outside `tooling/`, `.agents/` and the repo's top-level docs that name the folder,
and the rule says nothing else should. They were left alone deliberately on 2026-08-12 rather than deleted:
each is the only place a reader of that slice learns how to run its benchmarks, so removing them costs
something real. Converting these scripts turns every one into a recipe name - `just bench lib` - which is not a
path into `tooling/` at all, so the violation disappears without anyone writing the prose twice.

**Update those four files as part of this work.** If the conversion is abandoned, the references need deciding
on their own terms instead.

## Done when

Every `tooling/*.sh` a maintainer types is a `just` recipe, or says in one line at the top why it stayed a
script.
