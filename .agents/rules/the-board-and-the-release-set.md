---
description: The board and the release set divide all work between them. An agent maintains both, but never decides placement, readiness or priority.
load: on-trigger
when: touching board.md or a release file, or deciding what to work on next
paths:
  - ".agents/board.md"
  - ".agents/release-v*.md"
  - ".agents/post-release-v*.md"
---

# The board and the release set

## They split all the work between them

**Both are pointer surfaces.** Each holds references to plans and ideas, with the order and the dependencies
between them. Neither holds the work itself - that stays in the plan or idea file.

| | Holds | Lives |
|---|---|---|
| `board.md` | everything **not** tied to a release | permanently - versions come and go underneath it |
| `release-v<version>.md` | everything that **must ship with** that version | until the tag, then deleted |
| `post-release-v<version>.md` | everything that must happen **because** that version shipped | until its own items are done |

**An item is on exactly one of them.** Not both, and not neither.

- **Both** means the same scheduling decision is written twice, and the two copies disagree within a release
  with neither announcing which is stale.
- **Neither** means the work is invisible. A plan file nothing points at is not findable, and `plans/_index.md`
  is a manifest, not a queue.

**When work moves between them, it moves - it is not copied.** Pulling a plan into a release means taking its
row off the board. Cutting it from a release means putting the row back. Both halves happen in the same change.

**A release file may take a slice of a plan and leave the rest.** The plan file stays under `plans/` and stays
on the board for what is left; the release row names the slice it took. A plan file is deleted only when
nothing is left in it.

## Who decides what

**An agent maintains both files. A human directs them.**

- **Recording a row you were told to record is the agent's job**, and so is keeping it current: when a plan
  lands, tick the row and drop the link in the same change. Otherwise both files rot into dead links, which is
  exactly what they exist to prevent.
- **Placement, readiness and priority are the maintainer's call.** Deciding something is "ready", where it sits
  in an order, how urgent it is, or whether it belongs in a release - **ask.** Do not judge it and write it in.
- **If you must state a readiness to make a row legible, say that you chose it** so it can be struck.

## Why

**Scheduling has to live in one place per item, and that place is never the plan** - see
[plans-do-not-schedule-themselves](plans-do-not-schedule-themselves.md). These two files are that place. If
they overlap, or if a plan slips through both, the scheduling layer stops being trustworthy and everyone goes
back to reading every file to find out what is happening.

**And the decision is not the agent's to make.** When and whether something ships is judged across the whole
board with everything in view. An agent working one task sees one corner of it.
