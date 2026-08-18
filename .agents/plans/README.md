# Plans

Work not yet done — designs, migrations, TODOs, deferred decisions. A plan is a scratchpad for work that
is scheduled or being built now-ish. If it's a maybe with no commitment, it's an idea — put it in
`.agents/ideas/` instead.

Group plans by slice, mirroring the repo — a plan that clearly belongs to one area lives in that slice
folder (e.g. `plans/lib/`, `plans/shared/`); anything that maps to no slice stays at the root.

## Rules

- **One item per plan file.** A session should be able to open one file, do the whole thing, and delete it —
  without pulling in three unrelated topics. Something that needs a decision, research, or more than one sitting
  gets its own file. A single mechanical act with a known answer does not: it belongs in `todos.md`, or as a
  checkbox on the release file if it gates a release.
- **One master plan per topic**, holding what is done and what is left. When a review turns up issues, put them
  in **one findings file** beside it; a finding lives there until it is fixed, then moves into the master and is
  deleted from findings. Delete findings when it's empty. Don't let a topic sprawl into four overlapping plans,
  and don't keep a session log inside a plan — history belongs in git.
- **One item per file means facts get repeated. Keep the repeat to a sentence.** A fact two plans both need
  (a breaking change, a config shape) is stated in each. That's the accepted cost — what isn't accepted is a
  shared background section growing inside several plans at once.
- **Delete it when it lands** — or trim it down to only the part that remains. A plan and a doc should never
  describe the same finished thing. When a plan's content is done, its lasting facts move into a doc (what it
  is now) or design (why it was built that way).

## Index

The manifest is generated — see [`_index.md`](_index.md). Regenerate with `just agents all` after adding,
renaming or re-describing a plan.
