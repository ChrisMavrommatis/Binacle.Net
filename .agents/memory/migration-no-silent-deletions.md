---
name: migration-no-silent-deletions
description: Migrations land as small reviewable diffs — every removed test needs a visible successor, never a silent delete
type: convention
when: deleting or replacing a test during a migration
paths:
  - "**/test/**"
---

Phase 1 of a migration is **small changes a human can review**. A silent deletion is not reviewable: the
reviewer cannot tell whether an equivalent exists elsewhere, was folded into something else, or is genuinely
not needed.

**Why:** the maintainer reads the diff to confirm coverage was preserved. A `D` line forces them to reconstruct
from memory what that file covered and whether it still matters — which has already cost a review once, when a
test data provider was deleted with no visible successor.

**How to apply:** do not delete during phase 1. Convert each file in place to its successor — rename and
rewrite, so the diff reads as a 1:1 transformation. If a concept is genuinely dead, surface it for the
maintainer to approve rather than removing it quietly. Every old test should map to a visible new test, a
documented fold-into-X, or an explicit "dead, no equivalent" that the maintainer signs off. Keep each change
small.
