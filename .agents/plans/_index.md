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
| [vipaq/codec-race.md](vipaq/codec-race.md) | The report the codec race must produce — modes, tables and columns — so PROTOCOL.md §6 can name a codec. |
| [vipaq/migration-api-followups.md](vipaq/migration-api-followups.md) | What the API and UIModule migration left behind — stale OpenAPI examples, saved browser tokens, and the v3 payload break. |
| [vipaq/prompt.md](vipaq/prompt.md) | Persistent ViPaq session prompt. Read first, work, then update it before you finish. |
| [vipaq/README.md](vipaq/README.md) | ViPaq — build plan |
| [vipaq/testskernel-restructure.md](vipaq/testskernel-restructure.md) | Binacle.ViPaq.TestsKernel — remaining alignment work. Core alignment is done (2026-07-09, see D10); this tracks what is left. |
