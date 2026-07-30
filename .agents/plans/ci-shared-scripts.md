# CI - one set of commands, run by both CI and a human

**Status (2026-07-29):** Every script CI cares about has moved. Tests, coverage, the OpenAPI documents, the
agent indexes, running things from source, the build and the image stacks are `just` modules under `config/`;
setup is `just install` / `just assets` in the root justfile. `run-tests.yml` and `sonar-analysis.yml` call the
recipes.

Left, in order:

1. **Wire `release-docker-image.yml` to `just build publish`.** The workflow still inlines its own restore +
   publish, so the image CI ships and the image a maintainer builds come from two command lines that can drift -
   one edit away from gone. `vars.API_PROJECT_PATH` and `vars.BUILD_OUTPUT` are already gone: hardcoded in the
   workflow on 2026-07-30 to unblock the beta (`BUILD_OUTPUT` had to equal the path the Dockerfile hardcodes, so
   it was a repo setting that could silently break the image). That removed the repo-setting risk but not the
   duplication - the `Restore` and `Publish` steps still need to become `just build publish`.
   `vars.BUILD_DOCKERFILE` stays - `docker/build-push-action` needs `file:`, and the push, the semver metadata
   and `latest=auto` stay CI's.
2. **`performance.{lib,vipaq}.sh`, `benchmarks.{lib,vipaq}.sh`, `tmux.sh`.** CI runs none of them, so they
   gate nothing. What they cost is discoverability: `just --list` answers "what can I run" and recipe names
   complete, while nothing in `config/` completes anything. Open: `tmux.sh` builds a session and attaches -
   no arguments, nothing to parameterise - so it may be the one that stays a script.

**For the docs/web session.** `docs/README.md` and `web/README.md` both open with a setup block that says
`npm run copy-assets-to-<site>` "from the repo root", then `bundle exec jekyll serve`. Those still work, but
the one command now is `just install` once, then `just serve docs` / `just serve web` - which runs jekyll and
the webpack watch together under one Ctrl-C. `just assets` is the asset copy on its own. Both files are off
limits from a coding session, so the correction is left here.

## Why

One entry point per job, called by both CI and a maintainer. CI keeps only what is genuinely CI's - checkout,
SDK setup, service containers, caching, the matrix. Anything that decides *what runs* belongs in the entry
point, not the workflow, or the two drift and "works on my machine" stays a real answer.

## How, from the moves that landed

- **Absorbed, not wrapped.** The recipe runs the tool directly and the script is deleted. A recipe that only
  calls a script is two files where there was one, and keeps the drift it was meant to remove. A script that is
  a program rather than a command line still counts as absorbed when it moves into a shebang recipe body whole -
  that is how the 103-line `agents-index.sh` moved.
- **An alias list becomes a parameter whose `case` rejects an unknown value.** The launch profiles
  (`N|S|U|All`) went in that way, and `Encode|Decode` is the same shape. Without the reject, a typo falls
  through to the default and reports a green run for something nobody asked for.
- **One module per job, not per script.** Recipes that answer different questions do not share a module just
  because their scripts sat in the same folder. `serve` is everything run from source, `image` is the built
  image, `build` is separate again because what CI calls must have no `sudo` and no local paths in it. Where
  two modules need the same few lines, copy them - one reaching into another restores the coupling the split
  removed.
- **Module recipes need `set working-directory := '..'`**, and a tool that resolves paths itself (MSBuild
  resolves a relative output directory against the project, not the caller) needs an absolute path passed in.
- **An env var carries CI's flags into the shared recipe.** `DOTNET_TEST_ARGS="--configuration Release
  --no-build"` is how CI runs every leaf against one Release build without a CI-only recipe.
- **When the moved script is a generator, prove the move by diffing its output**, not by its exit code. The
  publish was proved that way: same file list and sizes as the old restore + `publish --no-restore`.

## Done when

Every workflow step that decides what runs is a call a maintainer can run the same way - `run-tests.yml` and
`sonar-analysis.yml` already are, `release-docker-image.yml` is not - and every `config/*.sh` a maintainer types
is a `just` recipe or says in a line why it stayed a script.
