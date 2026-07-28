# Idea: move the remaining config scripts into `just`

**Status (2026-07-28):** The tool question is settled and the pattern is proven. `just` was picked;
`config/tests.just` and `config/coverage.just` are the `test` and `coverage` modules, both workflows call them,
and `config/tests.*.sh` + `config/coverage.sh` are deleted.

What is left is only this: whether `api.sh`, `performance.*.sh`, `benchmarks.*.sh`, `lint.openapi.sh`,
`tmux.sh` and `agents-index.sh` follow. `build.sh` is **not** part of this - the CI plan owns it.

## For

Discovery and completion. `just --list` is one place that answers "what can I run", and recipe names complete
out of the box. Nothing in `config/` completes anything today, and the obvious fix - a hand-written bash
completion file per script - is the trap: it duplicates each script's argument list (`N|S|U|All`,
`Encode|Decode`), so changing a script makes its completion silently lie.

Completion should be a byproduct of the command definition, not a second file kept in sync by hand.

## Against

The scripts left are single-purpose and rarely typed. Their aliases would have to become recipe parameters,
which is real work for commands run a few times a week.

Note what actually made the tests worth moving: they were **absorbed, not wrapped** - the recipes run the tools
directly and the scripts were deleted. A recipe that only calls a script is two files where there was one, and
keeps the drift it was meant to remove. If a script moves, it moves properly or not at all.
