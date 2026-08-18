---
name: sonar-no-quality-profile
description: Sonar rules cannot be switched off on this project - custom quality profiles start at the Team plan and this one is on Free, so "Sonar way" is read-only
type: gotcha
when: someone proposes turning a Sonar rule off
paths:
  - "tooling/sonar-analysis.xml"
---

**No Sonar rule can be deactivated anywhere on this project.** Custom quality profiles start at the Team plan
and the project is on Free, so "Sonar way" is read-only. Verified 2026-08-09 against the SonarCloud docs and the
`qualitygates/list` API, which reports `actions.create: false`.

This closes the option people reach for first. What is left is changing the code so the analyser reads it
correctly, or marking the individual finding in the SonarCloud UI with a reason.

**Why:** it is worth knowing that the easiest lever still technically available - a `sonar.issue.ignore` glob in
the analysis file - is the one that must not be pulled. Every other route is either blocked by the plan or
visible on the record.

**How to apply:** when someone proposes turning a rule off, say it cannot be done here rather than looking for
where. Do not substitute a path ignore for it.
