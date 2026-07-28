# CI - one set of commands, run by both CI and a human

**Status (2026-07-28):** Tests, coverage, the OpenAPI documents and the agent indexes are `just` modules
(`config/tests.just`, `config/coverage.just`, `config/openapi.just`, `config/agents.just`), and
`config/tests.*.sh`, `config/coverage.sh`, `config/lint.openapi.sh` and `config/agents-index.sh` are deleted.
`run-tests.yml` and `sonar-analysis.yml` call the recipes.

Left: the build and the docker image, and the `config/*.sh` that have not moved yet.

## Why

`release-docker-image.yml` inlines its own `dotnet publish`, and `config/build.sh` does that same publish plus a
`docker build`. So the image CI ships and the image a maintainer builds locally come from two separate recipes
that drift: a flag added to one is not added to the other, and "works on my machine" stays a real answer.

The same argument covers the scripts CI never calls. `just --list` is one place that answers "what can I run",
and recipe names complete out of the box; nothing in `config/` completes anything. The obvious fix - a
hand-written bash completion file per script - duplicates each script's alias list (`N|S|U|All`,
`Encode|Decode`), so changing a script makes its completion silently lie.

## What

One entry point per job, called by both CI and a maintainer. CI keeps only what is genuinely CI's - checkout,
SDK setup, service containers, caching, the matrix. Anything that decides *what runs* belongs in the entry
point, not the workflow.

**The build and the image.** `release-docker-image.yml` and `config/build.sh` collapse into one recipe both call.
This is the one that blocks other work: the PR image gate needs it too.

**`just openapi lint` on a PR.** One call, it generates the documents itself, nothing to bring up - so the spec
standards stop depending on someone remembering to run it.

**The rest of `config/`:** `api.sh`, `performance.{lib,vipaq}.sh`, `benchmarks.{lib,vipaq}.sh`, `tmux.sh`. CI
runs none of them, so they are the tail of the same move and gate nothing. Open: `tmux.sh` builds a session and
attaches - no arguments to complete, nothing to parameterise - so it may be the one that stays a script; and
whether `api.sh`'s launch profiles are worth ~10 lines of recipe.

## How, from the moves that landed

- **Absorbed, not wrapped.** The recipe runs the tool directly and the script is deleted. A recipe that only
  calls a script is two files where there was one, and keeps the drift it was meant to remove. A script that is
  a program rather than a command line still counts as absorbed when it moves into a shebang recipe body whole -
  that is how the 103-line `agents-index.sh` moved.
- **An alias list becomes a parameter whose `case` rejects an unknown value.** `N|S|U|All` and `Encode|Decode`
  are that shape. Without the reject, a typo falls through to the default and reports a green run for something
  nobody asked for - the same reason the ServiceModule backend is a positional argument today.
- **An env var carries CI's flags into the shared recipe.** `DOTNET_TEST_ARGS="--configuration Release
  --no-build"` is how CI runs every leaf against one Release build without a CI-only recipe.
- **Module recipes need `set working-directory := '..'`**, and a tool that resolves paths itself (MSBuild
  resolves a relative output directory against the project, not the caller) needs an absolute path passed in.
- **When the moved script is a generator, prove the move by diffing its output**, not by its exit code.

## Watch out

`config/build.sh` starts compose in the foreground and cannot hand the terminal back, so publish + `docker build`
must be separated from "run it" before either CI or a smoke run can use it. That split is the first step here,
not a detail.

## Done when

Every workflow step that decides what runs is a call a maintainer can run the same way - `run-tests.yml` and
`sonar-analysis.yml` already are, `release-docker-image.yml` is not - and every `config/*.sh` a maintainer types
is a `just` recipe or says in a line why it stayed a script.
