---
description: Manifest of every file under .agents/plans, grouped by area. Regenerate with just agents all.
---

# Agent Plans Index

Every plan in `.agents/plans/` (recursive), grouped by area. Plans are work not yet done — read the one
you need, and trim or delete it once the work lands.

## General

| File | Description |
|---|---|
| [architecture-boundaries.md](architecture-boundaries.md) | A human-readable architecture.yml stating what each part of the repo may and may not reference, readable on its own and consumable by off-the-shelf tools. |
| [image-base-slimming.md](image-base-slimming.md) | Harden and slim the base image |
| [sonar-issue-triage.md](sonar-issue-triage.md) | Sonar - what is left after the 2026-08-09 sweep |
| [todos.md](todos.md) | TODOs |
| [ui-test-harness.md](ui-test-harness.md) | A test harness for the UI |

## API

| File | Description |
|---|---|
| [api/integration-test-additions.md](api/integration-test-additions.md) | Integration tests: cover what the harness cannot see today |
| [api/ui-clients-off-v3.md](api/ui-clients-off-v3.md) | Migrate the shipped UI clients off the v3 API |
| [api/v4-stable.md](api/v4-stable.md) | v4 — flip from experimental to stable |

## CI/CD

| File | Description |
|---|---|
| [ci-cd/ci-gates.md](ci-cd/ci-gates.md) | CI - make the PR gate mean something |
| [ci-cd/multi-arch-images.md](ci-cd/multi-arch-images.md) | CI - publish the image for arm64 as well as amd64 |
| [ci-cd/release-pipeline-rebuild.md](ci-cd/release-pipeline-rebuild.md) | CI/CD - finish the GHCR release pipeline |

## Lib

| File | Description |
|---|---|
| [lib/benchmark-ledger.md](lib/benchmark-ledger.md) | Refresh the curated lib benchmark ledger |
| [lib/extract-packing-contracts.md](lib/extract-packing-contracts.md) | Aftermath of the Binacle.Packing extraction - the boundary file and four prose files still describe a project layout that no longer exists. |
| [lib/parallel-processors-decision.md](lib/parallel-processors-decision.md) | Decide what happens to the three `Parallel*` processors |

## Shared

| File | Description |
|---|---|
| [shared/testskernel-data-extraction.md](shared/testskernel-data-extraction.md) | TestsKernel — grow the shared fixture cases |
| [shared/testskernel-split.md](shared/testskernel-split.md) | Open question - now that Binacle.TestsKernel is lib-free, is there still a reason to split it? The original reason is gone. |

## Tooling

| File | Description |
|---|---|
| [tooling/image-module-stacks.md](tooling/image-module-stacks.md) | Decide what the `image` module is still for |
| [tooling/scripts-to-just-recipes.md](tooling/scripts-to-just-recipes.md) | Convert the last `tooling/*.sh` scripts to `just` recipes |
