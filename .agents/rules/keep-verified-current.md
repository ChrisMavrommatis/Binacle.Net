---
description: When you edit a doc, update its verified date and check its also_update list.
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
