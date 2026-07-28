# CI - one set of commands, run by both CI and a human

**Status (2026-07-28):** Every script CI cares about has moved. Tests, coverage, the OpenAPI documents, the
agent indexes, running things locally, the build and the compose stacks are `just` modules under `config/`;
setup is `just install` / `just assets` in the root justfile. `run-tests.yml` and `sonar-analysis.yml` call the
recipes.

Left, in order:

1. **Wire `release-docker-image.yml`.** `config/build.just` exists and `config/build.sh` is deleted, but the
   workflow still inlines its own restore + publish, so the drift is still there - it is just now one edit away
   from gone. Its `Restore` and `Publish` steps become `just build publish`. `vars.API_PROJECT_PATH` and
   `vars.BUILD_OUTPUT` stop being referenced (`BUILD_OUTPUT` had to equal the path the Dockerfile hardcodes, so
   it was a repo setting that could silently break the image). `vars.BUILD_DOCKERFILE` stays -
   `docker/build-push-action` needs `file:`, and the push, the semver metadata and `latest=auto` stay CI's.
2. `performance.{lib,vipaq}.sh`, `benchmarks.{lib,vipaq}.sh`, `tmux.sh`.

**For the docs/web session.** `docs/README.md` and `web/README.md` both open with a setup block that says
`npm run copy-assets-to-<site>` "from the repo root", then `bundle exec jekyll serve`. Those still work, but
the one command now is `just install` once, then `just serve docs` / `just serve web` - which runs jekyll and
the webpack watch together under one Ctrl-C. `just assets` is the asset copy on its own. Both files are off
limits from a coding session, so the correction is left here.

## Why

`release-docker-image.yml` inlines its own `dotnet publish` and `config/build.just` has another. So the image CI
ships and the image a maintainer builds locally still come from two recipes that drift: a flag added to one is
not added to the other, and "works on my machine" stays a real answer. The recipe exists now, so closing this
is one edit to the workflow.

The same argument covers the scripts CI never calls. `just --list` is one place that answers "what can I run",
and recipe names complete out of the box; nothing in `config/` completes anything. The obvious fix - a
hand-written bash completion file per script - duplicates each script's alias list (`Encode|Decode`), so
changing a script makes its completion silently lie.

## What

One entry point per job, called by both CI and a maintainer. CI keeps only what is genuinely CI's - checkout,
SDK setup, service containers, caching, the matrix. Anything that decides *what runs* belongs in the entry
point, not the workflow.

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
  questions do not share a module just because their scripts sat in the same folder. The compose files split
  on the same line: `docker-compose.yml` supports an API run from source, so it is `just serve services`,
  while the three that run the built image are the `image` module. `build` stays separate again because what
  CI can call must have no `sudo` and no local paths in it.
- **Copy the few lines, do not reach across modules.** `serve` and `image` both create and open bind-mounted
  folders. Having one call the other's private recipe would restore exactly the coupling that splitting them
  removed, and it is about six lines of `mkdir` and `chmod`.
- **The recipe is the place to answer the confusing failure.** `just image up` checks for
  `binacle-net:local` first, because without it compose falls back to pulling from Docker Hub and reports
  "pull access denied" - a credentials error for a missing local build. The check costs one line and removes
  a question that has been asked twice.
- **An env var carries CI's flags into the shared recipe.** `DOTNET_TEST_ARGS="--configuration Release
  --no-build"` is how CI runs every leaf against one Release build without a CI-only recipe.
- **Module recipes need `set working-directory := '..'`**, and a tool that resolves paths itself (MSBuild
  resolves a relative output directory against the project, not the caller) needs an absolute path passed in.
- **When the moved script is a generator, prove the move by diffing its output**, not by its exit code.

## Done when

Every workflow step that decides what runs is a call a maintainer can run the same way - `run-tests.yml` and
`sonar-analysis.yml` already are, `release-docker-image.yml` is not - and every `config/*.sh` a maintainer types
is a `just` recipe or says in a line why it stayed a script.
