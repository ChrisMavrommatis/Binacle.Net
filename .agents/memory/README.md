# Agent Memory

Persistent, versioned memory for agents working in this repo. Lives here (not in a private per-user
store) so it's shared, reviewable, and travels with the code.

## When to add a memory here

Only for a durable fact that is **not already captured** in the codebase, `.agents/docs/`, or
`.agents/plans/`. If the fact belongs in a doc or a plan, put it there instead and link to it — do not
duplicate. Memory is for the leftover "why" that has no other home: a non-obvious gotcha, a settled
decision and its rationale, a working convention.

Do not store: code structure, git history, or anything a doc/plan already records.

## Format

One fact per file, kebab-case name. Front matter:

```markdown
---
name: <short-kebab-case-slug>
description: <one-line summary — used to judge relevance later>
type: decision | gotcha | convention
---

<the fact. For a convention or decision, add **Why:** and **How to apply:** lines.
Link related memories with [[their-name]] and repo files by path.>
```

Delete a memory when it turns out wrong or its fact moves into a doc/plan.

## Index

The manifest is generated — see [`_index.md`](_index.md). Regenerate it after adding or renaming a
memory with `config/agents-index.sh` (it also rebuilds the docs and plans indexes).
