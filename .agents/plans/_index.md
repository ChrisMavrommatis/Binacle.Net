---
description: Manifest of every file under .agents/plans, grouped by area. Regenerate with just agents all.
---

# Agent Plans Index

Every plan in `.agents/plans/` (recursive), grouped by area. Plans are work not yet done — read the one
you need, and trim or delete it once the work lands.

## General

| File | Description |
|---|---|
| [ci-gates.md](ci-gates.md) | CI - make the PR gate mean something |
| [ci-release-workflow-build.md](ci-release-workflow-build.md) | CI - rebuild how the image is released |
| [image-base-slimming.md](image-base-slimming.md) | Harden and slim the base image |
| [image-module-stacks.md](image-module-stacks.md) | Decide what the `image` module is still for |
| [scripts-to-just-recipes.md](scripts-to-just-recipes.md) | Convert the last `config/*.sh` scripts to `just` recipes |
| [sonar-issue-triage.md](sonar-issue-triage.md) | Sonar - what is left after the 2026-08-09 sweep |
| [todos.md](todos.md) | TODOs |
| [ui-test-harness.md](ui-test-harness.md) | A test harness for the UI |

## API

| File | Description |
|---|---|
| [api/integration-test-additions.md](api/integration-test-additions.md) | Integration tests: cover what the harness cannot see today |
| [api/ui-clients-off-v3.md](api/ui-clients-off-v3.md) | Migrate the shipped UI clients off the v3 API |
| [api/v4-stable.md](api/v4-stable.md) | v4 — flip from experimental to stable |

## Lib

| File | Description |
|---|---|
| [lib/benchmark-ledger.md](lib/benchmark-ledger.md) | Refresh the curated lib benchmark ledger |
| [lib/parallel-processors-decision.md](lib/parallel-processors-decision.md) | Decide what happens to the three `Parallel*` processors |

## Shared

| File | Description |
|---|---|
| [shared/testskernel-data-extraction.md](shared/testskernel-data-extraction.md) | TestsKernel — grow the shared fixture cases |
