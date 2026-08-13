---
description: a schema-migration path for the ServiceModule store
paths:
  - "api/**"
---

# Idea: a schema-migration path for the ServiceModule store

**Status:** Unvetted idea. Not a priority now — becomes one the first time the schema changes after go-live.

## Why

ServiceModule has no migration framework: each backend creates its schema idempotently at startup
(`CREATE TABLE IF NOT EXISTS`, or the Azure-table equivalent) — the ServiceModule doc. That works exactly once,
on a fresh database. The first time a column is added or changed on a store that already has rows, the
create-if-not-exists step sees the table exists, skips, and the new column never appears — silently. So it is fine
today (the schema is new) and becomes a real gap the moment the schema evolves in production.

## What to add

A small, ordered, versioned migration runner — **not EF Core**. EF is a heavy dependency for a two-entity domain,
and it does nothing for the Azure Table backend (schemaless). Two right-sized shapes:

- **DbUp** — runs ordered `.sql` scripts and tracks an applied-version table. One small dependency; relational
  only (SQLite + Postgres, one script folder per dialect).
- **Homegrown** — extend the existing startup task into an ordered runner with an applied-version marker. Zero
  new dependency; basically DbUp-lite.

## Constraints that shape it

- **It has to run in-app at startup.** On an app-only-volume host there is no shell to run migrations by hand, so
  the app itself must apply them.
- **The store choice narrows it.** One relational store = ordered SQL for one dialect, the cheapest version. Two
  (SQLite + Postgres) = two script folders. Azure Tables is schemaless, so DDL migrations barely apply there.
  Decide the store first; the migration shape falls out.

## Open questions

- DbUp vs homegrown — how much migration volume is realistic for this domain.
- Where the applied-version marker lives per backend.
- How this meets the `EnsureRequired*TablesExist` startup tasks — replace them, or keep them for the initial
  create and layer migrations on top.

## Related

- the ServiceModule doc (current create-if-not-exists startup tasks, provider selection)
