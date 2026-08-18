---
description: Comments explain the trap in front of them, for the person editing that line. Thin. Anything an agent needs goes in .agents/.
load: on-trigger
when: writing or editing a code comment
paths:
  - "**/*.cs"
  - "**/*.ts"
  - "**/*.js"
  - "**/*.csproj"
  - "**/*.props"
  - "**/*.just"
  - "**/*.yml"
---

# Comments are for humans, and they are thin

The test is who is reading: a person editing that line, or an agent being briefed.

A comment earns its place by explaining the trap in front of it - why the path must be absolute, why there is
no `--` before the runner options. Background, task history, "keep this in step with X", and anything that
reads like instructions to an agent belong in the matching `.agents/` layer instead.

Never both: a fact written in a comment and in a doc will disagree within a release.

**Write them thin.** A comment carries the trap, the numbers, or the thing the code cannot show - nothing
else. Cut the connective grammar first: "This is required because it throws due to X and Y" is "without this
it throws". Cut the restatement of the line below it. Cut the essay; if the reasoning is worth keeping it goes
in `.agents/`.

A table of widths, a byte layout, a measured number - those stay. A reader cannot recover them.

**A surviving agent comment is not damage.** When a review pass strips agent-written comments, the test is the
one above, not who typed it. Several were kept on purpose because they were better than the line they replaced -
the unchecked-multiply overflow note on the packing algorithms, the empty-catch explanation in
`ConnectionString.cs`, the curated-scenario table in `BischoffCuratedProblemsProvider.cs`.
