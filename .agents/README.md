# .agents — guidance for agents working in this repo

Everything an agent needs that isn't code lives here, versioned with the project so it's shared and
reviewable. Read this first to know where things are.

## Layout

| Path | What it is | When to use it |
|---|---|---|
| `rules/` | **Every standing rule, one file per rule.** Each declares in its front matter whether it is always-on or fetched on a trigger. | Read `rules/README.md` at the start of any task that writes something. Match the trigger, open that one file. |
| `docs/` | Stable reference docs for the codebase — slices, endpoints, modules, build. | Find the topic in `docs/_index.md` (or a task in `docs/README.md`), then read that file. |
| `design/` | The settled design *behind* the docs — decisions (why) and findings (measured evidence). Permanent, but it can change. | Find it in `design/_index.md`. |
| `plans/` | Work not yet done — designs, TODOs, migrations, deferred decisions. | Find the plan in `plans/_index.md`. Trim/delete an item once it lands. |
| `ideas/` | Rough, unvetted ideas — no commitment, no timeline. | Find the idea in `ideas/_index.md`. Move it to `plans/` once it's picked up (`ideas/README.md` says how). |
| `memory/` | Durable "why" with no home in a doc or plan — gotchas, settled decisions, conventions. | Scan `memory/_index.md` at session start. Add a fact only if no doc/plan fits (`memory/README.md` says how). |
| `board.md` | **Where work is picked from.** Every plan, idea and one-liner not tied to a release, grouped by theme and then by readiness. Permanent — releases come and go underneath it. | Start here when choosing what to work on next. |
| `release-v<version>.md` (+ companions) | The per-version release set, at root: the release plan, plus `release-notes-v<version>.md` (the GitHub release body) and `post-release-v<version>.md` (right-after-release work). | When cutting a release. The first two are deleted once the version is out; the post-release list goes when its own items are done. |

Nothing here is loaded up front. `CLAUDE.md` carries the four always-on rules and points at this file; you
fetch the rest on demand.

**Every file declares when it is needed, so you can decide without opening it.** The front matter is the fetch
key, and every layer uses the same three optional keys on top of whatever else it carries:

```yaml
description: what this file is, in one line     # every file
when: the plain-language trigger                # rules and memory - when this fires
paths: ["lib/src/**"]                           # the code it covers
```

**Each layer has a generated `_index.md` listing those fields as yaml**, so one read of an index tells you
which files matter for the code you are about to touch: match your path against `paths:`, or grep
`load: always`. Regenerate them all with `just agents all` after adding, renaming or re-describing a file.

**Everything is grouped by slice, mirroring the repo layout.** A slice is a top-level area of the
codebase (`api`, `lib`, `vipaq`, `shared`, `tooling`, `ci-cd`, …); files that don't map to one live at the root
under `General`. Most slices are a directory; `ci-cd` is the exception and covers `.github/workflows/`.
The docs, design, plans, and ideas for a slice sit in a folder of the same name, so an agent can open
just the slice it's working on and skip everything else — the point is to find the relevant guidance fast,
not to eagerly load unrelated context. Keep a new doc/plan/idea in its slice folder for the same reason.

## How the pieces differ

- **docs = what is true now.** Present tense, verified against code, **current canon only — no history**: no
  "used to", no "was dropped", no reversals. History belongs in design. Has `verified:` / `check:` /
  `also_update:` front matter — keep it current when you touch the described code.
- **design = why + evidence + history.** The settled design *behind* the docs — the decisions (rationale) and
  findings (measured evidence). Permanent and maintained like docs (same `verified:` / `check:`), but it holds
  *why we built it this way*, not *what it is*, and it **can change** as the reasoning evolves. It also owns the
  **history** — superseded evidence and reversed decisions — which can go in a dedicated `<slice>/history.md`.
- **plans = what is not done yet.** When a plan is fully implemented, delete it (or trim to only the
  part that remains). A plan and a doc should never describe the same finished thing.
- **ideas = a rough thought, unvetted.** No commitment, no timeline. When one is picked up it becomes a
  plan; when it's built or dropped, delete it. If you can't tell plan from idea: is it scheduled work
  (plan) or a maybe (idea)?
- **memory = the leftover why.** Not product behaviour (that's docs) and not future work (that's plans).
  Durable but mutable — it can change. One fact per file. If a memory's fact moves into a doc/plan,
  delete the memory.
- **`board.md` = what to work on.** At root, permanent, and the answer to "what next". It groups every plan,
  idea and one-liner by theme and then by readiness (`ready` / `blocked` / `deferred` / `in progress`), and it
  names what each blocked item waits on. `rules/the-board-and-the-release-set.md` holds how it is kept and who
  decides what goes on it.
- **the `release-v<version>` set = shipping.** At root, one set per version: the plan, its notes, and the
  post-release list. The plan and notes are deleted once the version ships; the post-release list is deleted
  when its own items are done, not by the tag. That list holds only what must happen **because** the release
  shipped — standing work belongs on the board. Same rule file as the board.

## The `docs/` trap

Repo-root `docs/` is the published documentation site and is off limits — `rules/never-edit-published-sites.md`
covers it and `web/` together. `.agents/docs/` is this system's reference layer and is unrelated. Only the first
is off limits.

## Who may reference whom

One table, one file: `rules/who-references-whom.md`. It covers every layer, the outward boundary, and the
three exceptions. Nothing here restates it.

## How to reference — the `$` symbol scheme

Point at another agent doc with a **`$` reference**, not a file path. Paths break when a file moves (this whole
`design/` split is proof); a `$` reference does not, and it greps cleanly both ways.

- Every doc/design file carries an **`id:`** in its front matter — a stable handle: `vipaq`, `vipaq/decisions`, …
- **`` `$id` ``** points at a whole doc (`` `$vipaq/findings` ``). **`` `$slice#anchor` ``** points at a section
  (`` `$vipaq#D16` ``, `` `$vipaq#width-selection` ``) — anchors are unique within a slice, so a reader greps the
  anchor inside that slice's `docs/` + `design/`.
- **Annotate a referenced section** so the target is findable: a heading tag `{#width-selection}`, or a label the
  heading already carries (the decision headings `D1…`, `O1…` are their own anchors).
- `also_update:` lists sibling docs by **id** (`vipaq/findings`), not path.
- Reference **code, `results/`, and the wire spec by real path** — the `$` scheme is for `.agents` docs only.

**Ref codes stay inside the agent docs** - see `rules/ref-codes-stay-in-the-agent-docs.md`.

## Rules of thumb

The standing rules moved to `rules/`, one file each - read `rules/README.md` rather than hunting for them
here. Two that shape how you use *this* directory:

- **Find the layer first, then the file.** Each layer has an `_index.md` manifest; open that, not every file.
- **A rule about the whole system belongs in `rules/`. A fact about one topic belongs in the doc or memory
  that owns it** - v3-is-frozen in `memory/v3-frozen.md`, endpoint rules in `docs/api/`.
