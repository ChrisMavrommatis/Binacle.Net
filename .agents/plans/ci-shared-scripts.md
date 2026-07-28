# CI - one set of scripts, run by both CI and a human

**Status:** Build and the docker image are left. Tests and coverage landed 2026-07-28.

## Why

`release-docker-image.yml` inlines its own `dotnet publish`, and `config/build.sh` does that same publish plus a
`docker build`. So the image CI ships and the image a maintainer builds locally come from two separate recipes
that drift: a flag added to one is not added to the other, and "works on my machine" stays a real answer.

## What

One entry point per job, called by both CI and a maintainer. CI keeps only what is genuinely CI's - checkout,
SDK setup, service containers, caching, the matrix. Anything that decides *what runs* belongs in the entry
point, not the workflow.

Tests and coverage set the pattern to copy: `just test <leaf>` and `just coverage all [format]`, in
`config/tests.just` and `config/coverage.just`. Two knobs from those are worth reusing - `DOTNET_TEST_ARGS`
carries CI's `--configuration Release --no-build` into every leaf, and the ServiceModule backend is a positional
argument that rejects a typo instead of falling back to the default and reporting a green run for a backend
nobody exercised.

## Watch out

`config/build.sh` starts compose in the foreground and cannot hand the terminal back, so publish + `docker build`
must be separated from "run it" before either CI or a smoke run can use it. That split is the first step here,
not a detail.

## Done when

Every workflow step that decides what runs is a call a maintainer can run the same way. `run-tests.yml` and
`sonar-analysis.yml` already are; `release-docker-image.yml` is not.
