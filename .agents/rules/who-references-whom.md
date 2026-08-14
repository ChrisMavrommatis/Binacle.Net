---
description: The one reference matrix - what every file type may point at, what it may never point at, and the three exceptions.
load: always
when: adding any link, $ reference or pointer, anywhere in the repo
---

# Who may reference whom

The whole rule, in one table. Nothing else states any part of it.

| File type | May reference | Never references |
|---|---|---|
| **docs** (permanent) | code, paths, READMEs, **other docs** | design, plans, ideas, memory |
| **design** (permanent) | code, paths, READMEs, **docs, other design** | plans, ideas, memory |
| **plans** (ephemeral) | code, paths, READMEs - **nothing under `.agents/`** | every layer, including each other |
| **ideas** (ephemeral) | code, paths, READMEs - **nothing under `.agents/`** | every layer, including each other |
| **memory** (ephemeral) | ideally nothing; a doc or design only if it truly must | plans, ideas, other memory |
| **rules** | code, paths, other rules | docs, design, plans, ideas, memory |
| **slice READMEs** | whatever its own layer may | whatever its own layer may not |
| **anything outside `.agents/`** | code, paths, itself | **`.agents/` - anything at all** |

## Why each line

- **Docs never point at design.** Docs are "what is true now"; design is the "why" and can change under them.
  Design points at docs; docs never point back.
- **Nothing permanent points at anything ephemeral.** A plan or idea is deleted when built or dropped, and a
  memory can stop being true, so the link dangles. If a permanent file needs the content, it was not
  ephemeral - move it into a doc or design record first.
- **Plans and ideas cite no `$` reference at all** - not a doc, not a design record, not each other. Name the
  area in plain words ("the ServiceModule doc") and inline the fact. A plan is a scratchpad that gets deleted;
  a `$` reference out of one is a maintenance debt for a file that will not outlive the work.
- **Nothing outside `.agents/` may point into it.** Three shapes, all banned: a filename (`see decisions.md`),
  a `$` reference (`Formula per $lib/result-building`), and a bare ref code (`(D16)`, which names no file at
  all and is the worst of the three to read). 27 of these accumulated silently before anyone counted.

  **A path a tool operates on is not a reference.** `tooling/agents.just` reads and writes
  `.agents/**/_index.md` and the root `justfile` registers it. Those are operands. What is banned is the
  pointer, because it makes an outside file depend on a layout the system is free to rearrange.

## The three exceptions

Each may point at anything, and **nothing may point back at them**.

- **`CLAUDE.md`** - the door. Something has to say `.agents/` exists.
- **`.agents/README.md`** - the map. It may *name* any file as navigation.
- **`board.md`** and the **`release-v<version>` set** - permanent files whose contents are entirely ephemeral.
  A release file may name the board in plain words but may not link it: "nothing points at the board" wins.
  `post-release-v<version>.md` is the only file that may link into `ideas/`.

Navigation is not citation. A README that lists its folder's files is indexing them, not citing them.
