# CI - one set of scripts, run by both CI and a human

**Status:** Not started. After v3.0.0. Everything else on the CI list gets easier once this lands, so do it
first among them.

## Why

The workflows inline their own `dotnet test` / `dotnet build` commands while `config/` holds a parallel set of
scripts doing the same thing. The two drift: a step added to CI is missing locally, a flag fixed locally never
reaches CI, and "works on my machine" becomes a real answer. `sonar-analysis.yml` inlines every coverage command
with no wrapper at all.

Worked example, 2026-07-27: adding `Binacle.Net.Kernel.UnitTests` meant editing four places - the `.slnx`,
`config/tests.api.sh`, `run-tests.yml`, and this list of places. Miss the third and the suite silently never runs
in CI, which is worse than not having written it. With one script per job it would have been the `.slnx` and the
script.

## What

One entry point per job - build, test, coverage, image - living in `config/`, with CI calling the same script a
maintainer calls. CI keeps only what is genuinely CI's: checkout, SDK setup, service containers, caching, and
the matrix. Anything that decides *what runs* belongs in the script.

- Give every job a script: build, test (per slice), coverage, docker image.
- Rewrite the workflow steps to call them, passing the leg through arguments or env. `BINACLE_TEST_INFRA`
  already works this way.
- Keep the scripts argument-driven and quiet about their environment, so the same call works on a laptop and on
  a runner.

## Watch out

`config/build.sh` starts compose in the foreground and cannot hand the terminal back, so publish + `docker build`
must be separated from "run it" before either CI or a smoke run can use it. That split is the first step here,
not a detail.

A `justfile` already exists at the repo root and covers the docs and web dev loops. Decide whether these
entry points join it or stay as `config/*.sh` called by it - do not end up with three ways to run a build.

## Done when

Every workflow step that decides what runs is a call to a script a maintainer can run the same way.
