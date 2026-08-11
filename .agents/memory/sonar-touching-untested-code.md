---
name: sonar-touching-untested-code
description: Fixing an old Sonar smell in an untested file makes the quality gate worse - changed lines become "new code" and count as uncovered
type: gotcha
---

Sonar counts a line as **new code** because it changed, not because of why it changed. Cleaning up an old
code smell edits lines. If those lines sit in a file with no test coverage, they arrive in the new code
period as uncovered, and `new_coverage` is the condition the gate hangs on.

**This already happened here.** Commits `a20a2a39` and `938c6d7e` fixed old findings across the codebase.
Some of them landed in the Blazor `UIModule`, which is at 0% coverage. The 2026-08-08 run then reported
1289 new lines, of which only 51 were coverable and **35 were uncovered - 29 of them in files those two
commits touched**. New coverage came out at 31.4% against a threshold of 80%, and that single condition is
the whole reason the gate is red. Nothing was broken and nothing regressed. The cleanup itself moved the
number.

**The rule:** fix old findings in files that have tests. Leave old findings in files that do not, until
they do. The UI is the clear case and its harness is its own plan (the PR gate plan points at it).

Two things that follow, both easy to get wrong:

- **Old issues do not fail the gate.** Only new code is graded, and `new_maintainability_rating` sits at A.
  509 open issues block nothing. Cleaning them up is housekeeping with a real gate cost attached, so it is
  worth doing deliberately rather than in passing.
- **A rolling new code period forgives it.** New Code is "days = 30", so damage from a cleanup ages out
  about a month later with no action. Useful to know, and a warning: a gate that repairs itself by waiting
  is not measuring much between times.

Not every fix costs coverage. Changes to non-executable lines are free - adding `sealed` to a class
declaration cannot add an uncovered line, wherever the file sits. That is what makes S3260 safe to sweep
and S2325 ("make this method static") not, since the latter edits call sites inside method bodies.

**Why:** the gate is a clean-as-you-code measure. It reads an edit to an untested file as "you touched this
and left it untested", which is exactly what happened, and exactly what it is for.

**How to apply:** before a bulk fix of an old rule, check the coverage on the files it lands in. Sort the
work by that, not by rule. See the memory on Sonar issue ignores for what to do with findings you will not fix.
