# CI - build the docker image on every PR

**Status:** Not started. After v3.0.0.

## Why

The image is built in CI only when a release is published (`.github/workflows/release-docker-image.yml`). So a
PR never proves the image still builds, and a break is found at release time - which is exactly what happened
after the `Binacle.Geometry` extraction, where the image had not been built for the whole restructure.

`run-tests.yml` builds the solution and runs every suite on each PR. It does not build the image.

## What

- Add an image build step to the PR gate. Build only - no push, no login, no Docker Hub credentials on a PR.
- Use the same Dockerfile and the same publish arguments the release workflow uses, or the gate proves nothing.
- Decide whether it is a step in `run-tests.yml` or its own workflow. One job means one checkout and one SDK
  setup; a separate workflow can run on a schedule as well.

## Watch out

`config/build.sh` currently starts compose in the foreground and never hands the terminal back, so it cannot be
called from CI as it stands. Publish + `docker build` needs separating from "run it" first - that split is its
own piece of work.

## Done when

A PR that breaks the image build fails before it merges.
