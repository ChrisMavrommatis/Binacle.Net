# Plans

Work not yet done — designs, migrations, TODOs, deferred decisions. A plan is a scratchpad for work that
is scheduled or being built now-ish. If it's a maybe with no commitment, it's an idea — put it in
`.agents/ideas/` instead.

Group plans by slice, mirroring the repo — a plan that clearly belongs to one area lives in that slice
folder (e.g. `plans/lib/`, `plans/shared/`); anything that maps to no slice stays at the root.

## Rules

- **Self-contained. No `$` references at all** — not to a doc, not to design, not to another plan. A plan
  gets deleted when the work lands, so a reference out of it is debt for a file that won't outlive the work.
  Name the area in plain words ("the ServiceModule doc") and inline any fact you need. Code and READMEs may
  be referenced by real path.
- **Nothing points at a plan.** Docs and design are permanent; a link to a plan dangles the moment it's deleted.
- **One master plan per topic**, holding what is done and what is left. When a review turns up issues, put them
  in **one findings file** beside it; a finding lives there until it is fixed, then moves into the master and is
  deleted from findings. Delete findings when it's empty. Don't let a topic sprawl into four overlapping plans,
  and don't keep a session log inside a plan — history belongs in git.
- **Delete it when it lands** — or trim it down to only the part that remains. A plan and a doc should never
  describe the same finished thing. When a plan's content is done, its lasting facts move into a doc (what it
  is now) or design (why it was built that way).
- **`docs/` and `web/` are off limits.** When a change needs a page on either site written or corrected, record
  **what the page must say** in the plan that owns the work, and leave the writing to that session.

## Index

The manifest is generated — see [`_index.md`](_index.md). Regenerate it after adding or renaming a plan
with `config/agents-index.sh` (it also rebuilds the docs, design, ideas, and memory indexes).
