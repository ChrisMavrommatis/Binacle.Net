# CLAUDE.md

Guidance for Claude Code in this repo. Kept intentionally small: detailed guidance lives in `.agents/`
and is read on demand, not loaded up front.

## Where to look

`.agents/` holds all non-code guidance. **Read `.agents/README.md` first** — it maps the directory
(docs, plans, memory, and the release/pending-action trackers) and how to use each.

For a coding task, find the right doc, then read it:
- The "Common Tasks" table in `.agents/docs/README.md` maps a task to the docs it needs.
- `.agents/docs/_index.md` is the full manifest, grouped by area.
- `.agents/memory/` holds conventions and gotchas that have no other home.

Open the docs before you work — don't answer from a vague memory of them.

## Writing style

Plain, simple language in all docs, comments, and explanations. Short is better — cut any word that
doesn't change the meaning. Keep lines to ~120 chars; break at a full stop when a line runs long.

## Critical rule

- **Never commit, stage, or push.** The human commits, always — even when a task is done and green.
  Leave all changes in the working tree.
