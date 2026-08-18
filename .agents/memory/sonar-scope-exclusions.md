---
name: sonar-scope-exclusions
description: sonar.exclusions and friends are scope exclusions, not issue ignores - they are allowed and already in use
type: convention
when: reading or editing the exclusion lists in tooling/sonar-analysis.xml
paths:
  - "tooling/sonar-analysis.xml"
  - "Directory.Build.props"
---

`tooling/sonar-analysis.xml` **does** carry `sonar.exclusions`, `sonar.cpd.exclusions` and
`sonar.coverage.exclusions`. Do not read the ban on issue ignores as forbidding them - they are a different
thing. An issue ignore says "run this rule here, then hide what it finds". A scope exclusion says
"this is not our code, or not this metric's business".

- `sonar.exclusions` drops vendored `assets/lib/**` and the `shared/data/**` fixture corpus. Nobody reviews or
  fixes either, so measuring them only moved the totals.
- `sonar.cpd.exclusions` covers `lib/src/Binacle.Lib/Algorithms/**`, where the v1/v2 variants are parallel
  implementations by design. **Every rule still runs on those files** — only duplication detection stops.
- Support projects are handled in `Directory.Build.props`, not here, via `SonarQubeTestProject` (`$build-topology`).
  That reclassifies them as test code rather than hiding anything.

**Why:** the two look alike in the same file, and reading the issue-ignore ban too broadly would get a
legitimate exclusion deleted.

**How to apply:** the test is whether a reader of the code would want to know. A hidden finding fails it; a file
that was never ours does not.
