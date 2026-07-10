---
description: Manifest of every file under .agents/plans, grouped by area. Regenerate with config/agents-index.sh.
---

# Agent Plans Index

Every plan in `.agents/plans/` (recursive), grouped by area. Plans are work not yet done — read the one
you need, and trim or delete it once the work lands.

## General

| File | Description |
|---|---|
| [results-migration.md](results-migration.md) | Results Migration Plan |
| [todos.md](todos.md) | TODOs |

## API

| File | Description |
|---|---|
| [api/uimodule-alpine-port.md](api/uimodule-alpine-port.md) | UIModule — Port from Blazor Reactivity to Alpine.js |
| [api/v4-endpoints.md](api/v4-endpoints.md) | v4 API — Endpoint Buildout |

## Shared

| File | Description |
|---|---|
| [shared/testskernel-data-extraction.md](shared/testskernel-data-extraction.md) | TestsKernel — pull all fixture data out to shared/data |

## ViPaq

| File | Description |
|---|---|
| [vipaq/architecture.md](vipaq/architecture.md) | ViPaq architecture — the blind encode/decode layer, the layout codecs, and the serializer that chooses. Phase 1 is the base structure. |
| [vipaq/decisions.md](vipaq/decisions.md) | ViPaq — decisions ledger |
| [vipaq/findings.md](vipaq/findings.md) | ViPaq — findings (the measured evidence) |
| [vipaq/prompt.md](vipaq/prompt.md) | Persistent ViPaq session prompt. Read first, work, then update it before you finish. |
| [vipaq/README.md](vipaq/README.md) | ViPaq — build plan |
| [vipaq/reference/01-benchmark-permanent.md](vipaq/reference/01-benchmark-permanent.md) | Session 1 — The permanent benchmark (vs protobuf, 8/16 only) |
| [vipaq/reference/04-implement-csharp.md](vipaq/reference/04-implement-csharp.md) | Session 4 — Implement v2 in C# → update benchmarks |
| [vipaq/reference/05-ts-mirror-tests.md](vipaq/reference/05-ts-mirror-tests.md) | Session 5 — TypeScript mirror + tests |
| [vipaq/reference/06-regenerate-vectors.md](vipaq/reference/06-regenerate-vectors.md) | Session 6 — Regenerate interop vectors |
| [vipaq/reference/07-additional-features.md](vipaq/reference/07-additional-features.md) | Session 7 — Decide additional features |
| [vipaq/testskernel-restructure.md](vipaq/testskernel-restructure.md) | Binacle.ViPaq.TestsKernel — remaining alignment work. Core alignment is done (2026-07-09, see D10); this tracks what is left. |
