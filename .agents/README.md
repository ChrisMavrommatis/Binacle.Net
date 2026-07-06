# .agents — guidance for agents working in this repo

Everything an agent needs that isn't code lives here, versioned with the project so it's shared and
reviewable. Read this first to know where things are.

## Layout

| Path | What it is | When to use it |
|---|---|---|
| `docs/` | Stable reference docs for the codebase — slices, endpoints, modules, build. | Find the topic in `docs/_index.md` (or a task in `docs/README.md`), then read that file. |
| `plans/` | Work not yet done — designs, TODOs, migrations, deferred decisions. | Find the plan in `plans/_index.md`. Trim/delete an item once it lands. |
| `ideas/` | Rough, unvetted ideas — no commitment, no timeline. | Find the idea in `ideas/_index.md`. Move it to `plans/` once it's picked up (`ideas/README.md` says how). |
| `memory/` | Durable "why" with no home in a doc or plan — gotchas, settled decisions, conventions. | Scan `memory/_index.md` at session start. Add a fact only if no doc/plan fits (`memory/README.md` says how). |
| `release-notes.md` | The ongoing "Unreleased" changelog, in the maintainer's GitHub-release format. | Append an entry whenever a change would matter to a release or an upgrading operator. |
| `pending-actions.md` | External/manual steps that can't be done from the repo (CI vars, dashboard settings). | Add a step when a change needs action outside the code. |

Nothing here is loaded into the session up front — `CLAUDE.md` only points at this file, and you open
what you need on demand. `docs/`, `plans/`, `ideas/`, and `memory/` each have a generated `_index.md` (a
grouped manifest); regenerate them all with `config/agents-index.sh` after adding or renaming a file.

**Everything is grouped by slice, mirroring the repo layout.** A slice is a top-level area of the
codebase (`api`, `lib`, `vipaq`, `shared`, …); files that don't map to one live at the root under
`General`. The docs, plans, and ideas for a slice sit in a folder of the same name, so an agent can open
just the slice it's working on and skip everything else — the point is to find the relevant guidance fast,
not to eagerly load unrelated context. Keep a new doc/plan/idea in its slice folder for the same reason.

## How the pieces differ

- **docs = what is true now.** Present tense, verified against code. Has `verified:` / `check:` /
  `also_update:` front matter — keep it current when you touch the described code.
- **plans = what is not done yet.** When a plan is fully implemented, delete it (or trim to only the
  part that remains). A plan and a doc should never describe the same finished thing.
- **memory = the leftover why.** Not product behaviour (that's docs) and not future work (that's plans).
  One fact per file. If a memory's fact moves into a doc/plan, delete the memory.
- **release-notes + pending-actions = shipping.** Maintained as work lands so a release is ready to cut
  and no manual step is forgotten. Both live in this root, next to each other.

## Rules of thumb

- Put a fact in exactly one place. Link across with paths or `[[name]]`; never duplicate.
- The human commits — never commit, stage, or push. Leave changes in the working tree.
- When you edit a doc, update its `verified:` date and check its `also_update:` list. When verifying a
  doc, its `check:` field says exactly what to confirm.
- Code-level rules live with their topic, not here: e.g. v3-is-frozen in `memory/v3-frozen.md`, endpoint
  rules (`BindingResult<T>`, rate limiting, CORS) in `docs/api/`. `CLAUDE.md` keeps only the one hard
  guardrail (never commit).
