# .agents — guidance for agents working in this repo

Everything an agent needs that isn't code lives here, versioned with the project so it's shared and
reviewable. Read this first to know where things are.

## Layout

| Path | What it is | When to use it |
|---|---|---|
| `docs/` | Stable reference docs for the codebase — slices, endpoints, modules, build. | Find the topic in `docs/_index.md` (or a task in `docs/README.md`), then read that file. |
| `design/` | The settled design *behind* the docs — decisions (why) and findings (measured evidence). Permanent, but it can change. | Find it in `design/_index.md`. It cites docs with `$id` references; docs never cite it. |
| `plans/` | Work not yet done — designs, TODOs, migrations, deferred decisions. | Find the plan in `plans/_index.md`. Trim/delete an item once it lands. |
| `ideas/` | Rough, unvetted ideas — no commitment, no timeline. | Find the idea in `ideas/_index.md`. Move it to `plans/` once it's picked up (`ideas/README.md` says how). |
| `memory/` | Durable "why" with no home in a doc or plan — gotchas, settled decisions, conventions. | Scan `memory/_index.md` at session start. Add a fact only if no doc/plan fits (`memory/README.md` says how). |
| `release-v<version>.md` (+ companions) | The per-version release set, at root: the plan, plus `release-actions-v<version>.md` (manual/external steps), `release-notes-v<version>.md` (the GitHub release body), and `post-release-v<version>.md` (right-after-release work). | When cutting a release. Deleted once the version is out. |

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
  *why we built it this way*, not *what it is*, and it **can change** as the reasoning evolves. It also owns the
  **history** — superseded evidence and reversed decisions — which can go in a dedicated `<slice>/history.md`.
  It cites docs; **docs never cite it** (design can shift under them), and it never cites a plan, idea, or memory.
- **plans = what is not done yet.** When a plan is fully implemented, delete it (or trim to only the
  part that remains). A plan and a doc should never describe the same finished thing.
- **ideas = a rough thought, unvetted.** No commitment, no timeline. When one is picked up it becomes a
  plan; when it's built or dropped, delete it. If you can't tell plan from idea: is it scheduled work
  (plan) or a maybe (idea)?
- **memory = the leftover why.** Not product behaviour (that's docs) and not future work (that's plans).
  Durable but mutable — it can change. One fact per file. If a memory's fact moves into a doc/plan,
  delete the memory.
- **the `release-v<version>` set = shipping.** At root, one set per version: the plan, its actions, its notes,
  and the post-release list. The plan is the **one exception** to the reference rules — it may point at any file
  to coordinate the release, and nothing points back at it. Deleted once the version ships.

## Who may reference whom — keep the layers from bleeding

Two **permanent** layers describe the code as it is now (`docs`, `design`); three **ephemeral** ones capture work
and notes that come and go (`plans`, `ideas`, `memory`). Two rules keep the layers from bleeding:

- **Nothing permanent points at anything ephemeral.** A plan/idea is deleted when built or dropped, and a memory
  can stop being true, so a link to one dangles. If a permanent file needs the content, it wasn't ephemeral —
  move it into a doc or design first.
- **Docs reference only docs.** `docs` are the plain "what is now"; `design` is the "why", and design can change
  under the docs — so a doc must never depend on it. Design points at docs, never the reverse.

| File type | May reference | Never references | Lifecycle |
|---|---|---|---|
| **docs** (permanent) | code, READMEs, **docs** | design, plans, ideas, memory | The "what is now". Kept current. |
| **design** (permanent, can change) | code, READMEs, **docs, design** | plans, ideas, memory | The "why" behind the docs. Points at docs; docs never point back. |
| **plans** (ephemeral) | code, READMEs, docs, design *(only when it helps)* | plans, ideas, memory | Work being built now-ish. Deleted when it lands. |
| **ideas** (ephemeral) | code, READMEs, docs, design *(only when it helps)* | plans, ideas, memory | Future maybes. Become a plan when picked up. |
| **memory** (ephemeral) | ideally nothing — a doc or design only if it truly must | plans, ideas, memory | Rules/conventions that can stop being true. |
| **`.agents/README.md`** (the map) | any file, as navigation | — | The one global map. The only exempt README. |
| **slice READMEs** (per layer) | its own layer's rules — a `docs/` README references only docs | same as its layer | Describe, rule, and index their own folder. |

What falls out of these rules:

- **Nothing points at a plan, idea, or memory** — not even a README section that lists them survives as a live
  `$` link; navigation names them, it does not cite them. They vanish; incoming links dangle.
- **Ideas and plans are self-contained by default.** They may cite a doc or design when it genuinely helps, but
  never each other, and never a memory.
- **Memory stays as self-contained as it can.** Ideally it references nothing; if it truly must, only a doc or
  design — never another memory, plan, or idea.
- **Only the top-level `.agents/README.md` (and `CLAUDE.md`) is exempt** — it is the global map, so it may *name*
  any file. A slice README follows its own layer's rules: it describes, rules, and indexes its folder, but a
  `docs/` README still references only docs.
- **The release set is the second exception.** `release-v<version>.md` and its `post-release` / actions / notes
  companions coordinate a release, so the plan may point at any file — but, like the map, **nothing points at
  it**. It is version-scoped and deleted once shipped.

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
"D16" has no idea what it means. Never put a bare code in anything human-facing — the `release-notes` /
`release-actions` files, PR text, or a message to the maintainer. There, spell out the thing.

## Rules of thumb

- Put a fact in exactly one place. Cross-link agent docs with `$` references (see above); never duplicate.
- The human commits — never commit, stage, or push. Leave changes in the working tree.
- When you edit a doc, update its `verified:` date and check its `also_update:` list. When verifying a
  doc, its `check:` field says exactly what to confirm.
- **Rules live at the scope they cover.** A repo-wide rule → `CLAUDE.md` (which holds only those). A rule about
  the whole `.agents` system → this README. A rule about one layer → that layer's README (a docs rule →
  `docs/README.md`). A rule tied to one topic → its own doc or memory: v3-is-frozen in `memory/v3-frozen.md`,
  endpoint rules (`BindingResult<T>`, rate limiting, CORS) in `docs/api/`.
