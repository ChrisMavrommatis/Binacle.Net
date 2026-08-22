---
description: Manifest of every file under .agents/plans, grouped by area. Regenerate with just agents all.
---

# Agent Plans Index

Every plan in `.agents/plans/` (recursive), grouped by area. Plans are work not yet done — read the one
you need, and trim or delete it once the work lands.

## General

```yaml
- file: architecture-checks.md
  description: "Derive the repo's dependency graph into a generated file, draw it, and lint it with a small ruleset."
- file: comment-lint.md
  description: "A check that nothing outside the agent guidance directory points a reader into it."
- file: image-base-slimming.md
  description: "Harden and slim the base image"
- file: packing-demo-rework.md
  description: "The packing demo's sample data, randomizer and buttons - one component change landing on both hosts, module first"
  paths: ["packages/binacle-net-ui/**", "api/src/Binacle.Net.UIModule/**", "sites/demo/**"]
- file: sonar-issue-triage.md
  description: "Sonar - what is left after the 2026-08-09 sweep"
- file: todos.md
  description: "TODOs"
- file: ui-test-harness.md
  description: "A test harness for the UI"
```

## API

```yaml
- file: api/integration-test-additions.md
  description: "Integration tests: cover what the harness cannot see today"
  paths: ["api/**"]
- file: api/ui-clients-off-v3.md
  description: "Migrate the shipped UI clients off the v3 API"
  paths: ["api/**", "packages/binacle-net-ui/**"]
- file: api/v4-stable.md
  description: "v4 - flip from experimental to stable"
  paths: ["api/**"]
```

## CI/CD

```yaml
- file: ci-cd/ci-gates.md
  description: "CI - make the PR gate mean something"
  paths: [".github/workflows/**"]
- file: ci-cd/dockerhub-overview.md
  description: "The Docker Hub repository page"
  paths: [".github/workflows/**"]
- file: ci-cd/dockerhub-tag-immutability.md
  description: "Turn on Docker Hub tag immutability, for release tags only"
  paths: [".github/workflows/**"]
- file: ci-cd/multi-arch-images.md
  description: "CI - publish the image for arm64 as well as amd64"
  paths: [".github/workflows/**"]
- file: ci-cd/workflow-restructure.md
  description: "CI - what is left after the workflow restructure landed, and the gap the next workflows session inherits"
  paths: [".github/**"]
```

## Lib

```yaml
- file: lib/benchmark-ledger.md
  description: "Refresh the curated lib benchmark ledger"
  paths: ["lib/**"]
- file: lib/parallel-processors-decision.md
  description: "Decide what happens to the three `Parallel*` processors"
  paths: ["lib/**"]
```

## Shared

```yaml
- file: shared/testskernel-data-extraction.md
  description: "TestsKernel - grow the shared fixture cases"
  paths: ["shared/**"]
```

## Tooling

```yaml
- file: tooling/scripts-to-just-recipes.md
  description: "Convert the last `tooling/*.sh` scripts to `just` recipes"
  paths: ["tooling/**"]
```
