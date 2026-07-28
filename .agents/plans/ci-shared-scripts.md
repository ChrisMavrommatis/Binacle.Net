# CI - one set of commands, run by both CI and a human

**Status (2026-07-28):** Everything except the build has moved. Tests, coverage, the OpenAPI documents, the
agent indexes and running things locally are `just` modules under `config/`; setup is `just install` /
`just assets` in the root justfile. `run-tests.yml` and `sonar-analysis.yml` call the recipes.

Left: the build and the docker image, and `performance.{lib,vipaq}.sh`, `benchmarks.{lib,vipaq}.sh`, `tmux.sh`.

**For the docs/web session.** `docs/README.md` and `web/README.md` both open with a setup block that says
`npm run copy-assets-to-<site>` "from the repo root", then `bundle exec jekyll serve`. Those still work, but
the one command now is `just install` once, then `just serve docs` / `just serve web` - which runs jekyll and
the webpack watch together under one Ctrl-C. `just assets` is the asset copy on its own. Both files are off
limits from a coding session, so the correction is left here.

## Why

`release-docker-image.yml` inlines its own `dotnet publish`, and `config/build.sh` does that same publish plus a
`docker build`. So the image CI ships and the image a maintainer builds locally come from two separate recipes
that drift: a flag added to one is not added to the other, and "works on my machine" stays a real answer.

The same argument covers the scripts CI never calls. `just --list` is one place that answers "what can I run",
and recipe names complete out of the box; nothing in `config/` completes anything. The obvious fix - a
hand-written bash completion file per script - duplicates each script's alias list (`Encode|Decode`), so
changing a script makes its completion silently lie.

## What

One entry point per job, called by both CI and a maintainer. CI keeps only what is genuinely CI's - checkout,
SDK setup, service containers, caching, the matrix. Anything that decides *what runs* belongs in the entry
point, not the workflow.

**The build and the image.** `release-docker-image.yml` and `config/build.sh` collapse into one recipe both call.
This is the one that blocks other work: the PR image gate needs it too.

Three things are tangled in `build.sh` today and want separating: the publish (all CI needs), the
`docker build` on top of it, and the mkdir + `chmod 777` of the bind-mounted `config/data` and `config/azurite`,
which belongs to running the compose stack rather than to building anything. The azurite one needs `sudo`, so
it cannot sit in a recipe CI calls.

**`just openapi lint` on a PR.** One call, it generates the documents itself, nothing to bring up - so the spec
standards stop depending on someone remembering to run it.

**The rest of `config/`:** `performance.{lib,vipaq}.sh`, `benchmarks.{lib,vipaq}.sh`, `tmux.sh`. CI runs none
of them, so they are the tail of the same move and gate nothing. Open: `tmux.sh` builds a session and attaches
- no arguments to complete, nothing to parameterise - so it may be the one that stays a script.

## How, from the moves that landed

- **Absorbed, not wrapped.** The recipe runs the tool directly and the script is deleted. A recipe that only
  calls a script is two files where there was one, and keeps the drift it was meant to remove. A script that is
  a program rather than a command line still counts as absorbed when it moves into a shebang recipe body whole -
  that is how the 103-line `agents-index.sh` moved.
- **An alias list becomes a parameter whose `case` rejects an unknown value.** The launch profiles
  (`N|S|U|All`) went in that way, and `Encode|Decode` is the same shape. Without the reject, a typo falls
  through to the default and reports a green run for something nobody asked for.
- **One module per job, not per script.** `serve` holds the API and both site dev loops because to a
  maintainer they are one job - bring a thing up and hold the terminal. Recipes that answer different
  questions do not share a module just because their scripts sat in the same folder.
- **An env var carries CI's flags into the shared recipe.** `DOTNET_TEST_ARGS="--configuration Release
  --no-build"` is how CI runs every leaf against one Release build without a CI-only recipe.
- **Module recipes need `set working-directory := '..'`**, and a tool that resolves paths itself (MSBuild
  resolves a relative output directory against the project, not the caller) needs an absolute path passed in.
- **When the moved script is a generator, prove the move by diffing its output**, not by its exit code.

## Done when

Every workflow step that decides what runs is a call a maintainer can run the same way - `run-tests.yml` and
`sonar-analysis.yml` already are, `release-docker-image.yml` is not - and every `config/*.sh` a maintainer types
is a `just` recipe or says in a line why it stayed a script.
