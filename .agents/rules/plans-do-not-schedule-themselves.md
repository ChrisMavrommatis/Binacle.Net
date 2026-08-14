---
description: A plan or idea says what the work is, never when it happens. Scheduling lives on the board and in the release set.
load: on-trigger
when: writing or editing a plan or an idea
paths: [".agents/plans/**", ".agents/ideas/**"]
---

# A plan says what, not when

**Never write scheduling into a plan or an idea.** No "after v3.0.0", no "not a release item", no "not
urgent", no "low priority", no "likely done together with X".

Where scheduling lives instead:

| Question | Answer lives in |
|---|---|
| Is this ready, blocked or deferred? | `board.md` |
| What does it wait on? | `board.md` |
| In what order do these two go? | `board.md` |
| Does it ship in this version? | the `release-v<version>` set |

A plan holds **what the work is, why it is worth doing, and what will bite whoever does it.** That is all.

## The one thing that looks like scheduling but is not

**A technical ordering constraint stays in the plan.** The difference is whether the constraint comes from
the work itself or from someone's calendar.

- *"The page must not go live before the tag, because its text names a tag that does not exist yet"* -
  **keep it.** That is a fact about the work. It is true whenever this gets done.
- *"Do this after v3.0.0"* - **delete it.** That is a calendar decision.

If you cannot tell which one you are writing, ask: would this still be true if the release order changed? If
yes, it belongs in the plan.

## Why

**A plan outlives the schedule that produced it.** Priorities move, releases slip, and an item gets pulled
forward or pushed back by the maintainer - none of which changes a word of what the work actually is. A plan
carrying its own timing goes stale the moment that decision changes, and it goes stale **silently**: nothing
fails, the sentence just quietly starts lying.

It also puts the decision in the wrong hands. **When and whether something ships is the maintainer's call**,
made across the whole board with everything else in view. A plan file only ever sees itself, so a schedule
written there is a guess made with the least possible context - and it is a guess a later reader tends to
obey.

This bit for real: four plans were proposed for v3.0.0 and each carried its own contradictory timing note,
written months apart by sessions that could not see each other.
