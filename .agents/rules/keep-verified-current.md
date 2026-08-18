---
description: When you edit a doc, update its verified date and check its also_update list. One carve-out - a prose-only edit that checks nothing against code does not bump the date.
load: on-trigger
when: editing any doc or design record
paths:
  - ".agents/docs/**"
  - ".agents/design/**"
---

# Keep `verified:` current

When you edit a doc or design record, update its `verified:` date and check its `also_update:` list.

When verifying one, its `check:` field says exactly what to confirm. Confirm it against the code, not against
a memory of the code.

A doc whose paths or names no longer resolve is worse than no doc: it reads as current.

## The one carve-out: a prose-only edit does not bump the date

**`verified:` means "someone confirmed this against the code on this date". It does not mean "someone typed in
this file".** So an edit that confirms nothing leaves the date alone.

The test is one question: **did you read any code?** If the edit only rewords, shortens, fixes a typo, or moves
history out into a design record - touching no claim about a path, type, member or number - the date stays as
it was. If you resolved even one name against the source, bump it.

When in doubt, leave it. A stale date is a job on a list; a wrong one is a job nobody knows to do.

**Why:** bumping without checking is worse than not bumping, because it destroys the only signal there is. The
date is what tells the next session which files still need reading, and a fresh date on an unread file converts
"nobody has checked this" into "someone has" - silently, and in the direction that stops anyone looking again.
