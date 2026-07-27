---
description: Manifest of every file under .agents/plans, grouped by area. Regenerate with config/agents-index.sh.
---

# Agent Plans Index

Every plan in `.agents/plans/` (recursive), grouped by area. Plans are work not yet done — read the one
you need, and trim or delete it once the work lands.

## General

| File | Description |
|---|---|
| [beta-verification.md](beta-verification.md) | Beta verification - what to check while the v3.0.0 beta is deployed |
| [ci-all-modules-integration-tests.md](ci-all-modules-integration-tests.md) | CI - run the integration tests with all modules enabled |
| [ci-docker-image-gate.md](ci-docker-image-gate.md) | CI - build the docker image on every PR |
| [ci-shared-scripts.md](ci-shared-scripts.md) | CI - one set of scripts, run by both CI and a human |
| [ci-sonar-coverage-gate.md](ci-sonar-coverage-gate.md) | CI - put Sonar and coverage on the PR gate |
| [docker-release-tagging.md](docker-release-tagging.md) | Docker release - build the image once, and prove a prerelease tag is safe |
| [docs-swagger-documents.md](docs-swagger-documents.md) | Docs site - generate the v3 and v4 OpenAPI documents |
| [docs-v3-pages.md](docs-v3-pages.md) | Docs site - write the v3.0.x pages |
| [docs-vipaq-protocol-page.md](docs-vipaq-protocol-page.md) | Docs site - the shared ViPaq protocol page describes the old format |
| [npm-package-publishing.md](npm-package-publishing.md) | Decide whether the TypeScript packages get published |
| [sample-image-pinning.md](sample-image-pinning.md) | Decide how the samples pin the docker image |
| [todos.md](todos.md) | TODOs |
| [version-stamp.md](version-stamp.md) | Give the build a version of its own |

## API

| File | Description |
|---|---|
| [api/ui-clients-off-v3.md](api/ui-clients-off-v3.md) | Migrate the shipped UI clients off the v3 API |
| [api/v4-algorithm-claim.md](api/v4-algorithm-claim.md) | v4 - the endpoint descriptions overclaim which algorithms run |
| [api/v4-stable-in-3.1.0.md](api/v4-stable-in-3.1.0.md) | v4 — flip from experimental to stable in 3.1.0 |

## Lib

| File | Description |
|---|---|
| [lib/benchmark-ledger.md](lib/benchmark-ledger.md) | Refresh the curated lib benchmark ledger |
| [lib/parallel-processors-decision.md](lib/parallel-processors-decision.md) | Decide what happens to the three `Parallel*` processors |

## Shared

| File | Description |
|---|---|
| [shared/testskernel-data-extraction.md](shared/testskernel-data-extraction.md) | TestsKernel — grow the shared fixture cases |
