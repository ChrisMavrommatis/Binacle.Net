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

Plain, simple language in all docs, comments, explanations, and chat replies. Short is better — cut any word
that doesn't change the meaning. **Avoid jargon unless it is genuinely needed** — when a plain word works, use it;
only reach for a technical term when there is no simpler way to be exact, and then keep it. Keep lines to ~120
chars; break at a full stop when a line runs long.

This applies to every agent working in this repo, not just the main session.

**Text that reaches a user stays plain ASCII.** Validation and exception messages, log lines, OpenAPI
descriptions, UI strings: no em or en dashes, no curly quotes, no ellipsis character, no arrows or symbols.
Write `-` and `...`, and say "0-100", not "0–100". These land in consoles, log files, JSON and terminals where the
encoding is not ours to control, and a mangled character in a startup error is one more thing to debug. Prose in
`.agents/` and code comments are read by us, so they are free.

## Critical rules

- **Never commit, stage, or push.** The human commits, always — even when a task is done and green.
  Leave all changes in the working tree.

- **References point one way: out of `.agents`, never into it.** A file under `.agents/` may point at code, a
  path, a README, anything. **No file outside `.agents/` may point a reader at `.agents/` content** — not a
  code comment, not a workflow comment, not a repo README. This file is the single exception, because
  something has to say the directory exists.

  A **path a tool operates on is not a reference.** `config/agents.just` reads and writes `.agents/**/_index.md`
  and the root `justfile` registers it; those are operands, and they stay. What is banned is the pointer — "see
  `.agents/design/ci-cd/decisions.md` for why" — because it makes a file outside the system depend on a layout
  the system is free to change.

- **Comments are strictly for humans. Anything an agent needs goes in `.agents/`.** The test is who is reading:
  a person editing that line, or an agent being briefed. A comment earns its place by explaining the trap in
  front of it — why the path must be absolute, why there is no `--` before the runner options. Background, task
  history, "keep this in step with X", and anything that reads like instructions to an agent belong in the
  matching `.agents/` layer instead. Never both: a fact duplicated in a comment and a doc will disagree.

- **Never edit repo-root `docs/` or `web/`.** Both publish to the internet — `docs/` is the versioned
  documentation site, `web/` is the marketing site. (`.agents/docs/` is a different thing entirely — the agent
  reference layer, and editing it is fine.) They are written in their own dedicated session, by an agent whose
  whole job is that content, so a change made in passing gets published without anyone reviewing it as public writing. Read
  them freely. If work needs a page written or corrected, **write down what the page must say** in the relevant
  plan or release file and leave it for that session.

  **One carve-out: a security fix to a downloadable sample file.** The `docs/collections/_versions/**` folders
  hold sample files readers download and run - compose files, Kubernetes manifests, config json. When an
  analyser flags one of those as vulnerable, a coding session may fix the **file**, because that is the same
  mechanical change it would make to `samples/` and nobody reads it as writing. This is narrow on purpose:
  the change must touch no prose, no front matter and no `.md` file, and it must match what repo-root
  `samples/` already does. Anything else - including a sentence on the page explaining the fix - still goes
  in a plan for the docs session. The carve-out exists because these files are the only public attack surface
  in the repo and a fix to `samples/` does not reach the frozen copies; leaving one vulnerable until an
  unrelated session runs was the worse trade. Record every use of it in the plan that owns the work.
