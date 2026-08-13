---
description: Put a fact in exactly one place and cross-link. A fact written twice will disagree.
load: on-trigger
when: adding a fact to any file under .agents/
paths:
  - ".agents/**"
---

# One fact, one place

Put a fact in exactly one place. Cross-link agent docs with `$` references; never duplicate.

A fact written in two places will disagree within a release, and neither copy announces that it is the stale
one.
