# .agents — guidance for agents working in this repo

Everything an agent needs that isn't code lives here, versioned with the project so it's shared and
reviewable. Read this first to know where things are.

## Layout

| Path | What it is | When to use it |
|---|---|---|
| `docs/` | Stable reference docs for the codebase — slices, endpoints, modules, build. | Find the topic in `docs/_index.md` (or a task in `docs/README.md`), then read that file. |
| `design/` | The settled design *behind* the docs — decisions (why) and findings (measured evidence). Permanent and citable. | Find it in `design/_index.md`; docs cite it with `$id` references. |
| `plans/` | Work not yet done — designs, TODOs, migrations, deferred decisions. | Find the plan in `plans/_index.md`. Trim/delete an item once it lands. |
| `ideas/` | Rough, unvetted ideas — no commitment, no timeline. | Find the idea in `ideas/_index.md`. Move it to `plans/` once it's picked up (`ideas/README.md` says how). |
| `memory/` | Durable "why" with no home in a doc or plan — gotchas, settled decisions, conventions. | Scan `memory/_index.md` at session start. Add a fact only if no doc/plan fits (`memory/README.md` says how). |
| `release-notes.md` | The ongoing "Unreleased" changelog, in the maintainer's GitHub-release format. | Append an entry whenever a change would matter to a release or an upgrading operator. |
| `pending-actions.md` | External/manual steps that can't be done from the repo (CI vars, dashboard settings). | Add a step when a change needs action outside the code. |

Nothing here is loaded into the session up front — `CLAUDE.md` only points at this file, and you open
what you need on demand. `docs/`, `design/`, `plans/`, `ideas/`, and `memory/` each have a generated `_index.md`
(a grouped manifest); regenerate them all with `config/agents-index.sh` after adding or renaming a file.

**Everything is grouped by slice, mirroring the repo layout.** A slice is a top-level area of the
codebase (`api`, `lib`, `vipaq`, `shared`, …); files that don't map to one live at the root under
`General`. The docs, design, plans, and ideas for a slice sit in a folder of the same name, so an agent can open
just the slice it's working on and skip everything else — the point is to find the relevant guidance fast,
not to eagerly load unrelated context. Keep a new doc/plan/idea in its slice folder for the same reason.

## How the pieces differ

- **docs = what is true now.** Present tense, verified against code, **current canon only — no history**: no
  "used to", no "was dropped", no reversals. History belongs in design. Has `verified:` / `check:` /
  `also_update:` front matter — keep it current when you touch the described code.
- **design = why + evidence + history.** The settled design *behind* the docs — the decisions (rationale) and
  findings (measured evidence). Permanent and maintained like docs (same `verified:` / `check:`), but it holds
  *why we built it this way*, not *what it is*. It also owns the **history** — superseded evidence and reversed
  decisions — which can go in a dedicated `<slice>/history.md`. Docs cite it; it never cites a plan, idea, or memory.
- **plans = what is not done yet.** When a plan is fully implemented, delete it (or trim to only the
  part that remains). A plan and a doc should never describe the same finished thing.
- **ideas = a rough thought, unvetted.** No commitment, no timeline. When one is picked up it becomes a
  plan; when it's built or dropped, delete it. If you can't tell plan from idea: is it scheduled work
  (plan) or a maybe (idea)?
- **memory = the leftover why.** Not product behaviour (that's docs) and not future work (that's plans).
  Durable but mutable — it can change. One fact per file. If a memory's fact moves into a doc/plan,
  delete the memory.
- **release-notes + pending-actions = shipping.** Maintained as work lands so a release is ready to cut
  and no manual step is forgotten. Both live in this root, next to each other.

## Who may reference whom — keep the slices from bleeding

Every file points **up** the durability ladder, never **down** into something more ephemeral than itself.
Most durable first: **code / READMEs → docs ≈ design → memory → plans / ideas**. `docs` and `design` are the
permanent pair (one holds *what*, the other *why*); either may cite the other and code, nothing more ephemeral.

| File type | May reference | Never referenced by | Lifecycle |
|---|---|---|---|
| **docs** (permanent) | code, READMEs, docs, design | — | Kept current. Never points at a plan, idea, or memory. |
| **design** (permanent) | code, docs, design | — | The why + evidence behind the docs. Kept current like a doc; freely citable. |
| **memory** (durable, mutable) | anything — code, docs, design, even a plan or idea | anything (nothing links to a memory) | Can change. It owns its outward links: update the memory when what it references changes or is removed. |
| **plans** (ephemeral) | code, READMEs, docs, design | anything (nothing links to a plan) | Deleted when the work lands. |
| **ideas** (ephemeral) | code, READMEs, docs, design | anything (nothing links to an idea) | Deleted when picked up (→ plan) or dropped. |
| **READMEs** (permanent) | any file, as navigation | — | The map. Always current. |

What falls out of the ladder:

- **Docs and design never reference a plan, idea, or memory.** They are permanent; they may cite each other
  and code, but never something that can vanish under them.
- **Nothing references a plan or an idea.** They vanish, so a link to one dangles. If a permanent file
  needs the content, the content wasn't ephemeral — move it to a doc, design, or memory first.
- **Nothing references a memory.** A memory is a leftover note others don't build on. It may link
  outward, but then it carries the burden of staying in sync with what it links to.
- **READMEs are the one exception** — they are the map, so they may name any file to help you navigate.

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

**Ref codes stay inside the agent docs.** A label like `$vipaq#D16` is an agent's cross-reference; a human reading
"D16" has no idea what it means. Never put a bare code in anything human-facing — `release-notes.md`,
`pending-actions.md`, PR text, or a message to the maintainer. There, spell out the thing.

## Rules of thumb

- Put a fact in exactly one place. Cross-link agent docs with `$` references (see above); never duplicate.
- The human commits — never commit, stage, or push. Leave changes in the working tree.
- When you edit a doc, update its `verified:` date and check its `also_update:` list. When verifying a
  doc, its `check:` field says exactly what to confirm.
- Code-level rules live with their topic, not here: e.g. v3-is-frozen in `memory/v3-frozen.md`, endpoint
  rules (`BindingResult<T>`, rate limiting, CORS) in `docs/api/`. `CLAUDE.md` keeps only the one hard
  guardrail (never commit).
