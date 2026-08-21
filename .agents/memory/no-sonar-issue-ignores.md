---
name: no-sonar-issue-ignores
description: Sonar findings are answered in code, never with a sonar.issue.ignore rule in tooling/sonar-analysis.xml
type: decision
when: answering a Sonar finding
paths:
  - "tooling/sonar-analysis.xml"
  - "Directory.Build.props"
---

`tooling/sonar-analysis.xml` carries no `sonar.issue.ignore.multicriteria` entries, and none should be added.
A suppression there hides a finding from everyone reading the code, in a file nobody opens.

**Two honest answers to a rule you disagree with:**

1. **Change the code** so the analyser reads it correctly.
2. **Mark the individual finding** Accepted or False Positive in the SonarCloud UI, with a reason. It is
   per-finding rather than per-path, so the rule stays armed for the next occurrence and the reason is on the
   record.

Worked examples of the first:

- **S2699 "tests should include assertions"** — restructure so the assert sits in the test body, or mark the
  helper that does the asserting with `[AssertionMethod]`. The analyser matches that attribute **by name
  alone**: no package, any namespace. It is declared twice, in
  `shared/test/Binacle.TestsKernel/AssertionMethodAttribute.cs` and
  `vipaq/test/Binacle.ViPaq.UnitTests/AssertionMethodAttribute.cs`, because ViPaq.UnitTests deliberately does
  not reference TestsKernel (`$vipaq/dependencies`). C# S2699 has no rule parameters, so the Java
  `customAssertionMethods` advice found in Sonar community threads does not transfer.
- **S6418 "hard-coded secret"** on a dev placeholder — change the value so it stops looking like a credential.
  A path ignore would also blind that file to a real secret pasted in later.

**Two things `[AssertionMethod]` cannot reach**, worth knowing before reworking a test: it does not survive a
delegate hop (a test reaching its assert through a `Dictionary<Type, Action>` shows the analyser only
`Action.Invoke`), and there is no code fix for the jwt.io sample JWT in the `TokenResponse` OpenAPI example.

**Three findings survive with nothing honest to change** and are marked in the SonarCloud UI: that jwt.io JWT;
**S2245 "use a cryptographically strong RNG"** on `getRandomInt.ts`, not a security context, where swapping in
`RandomNumberGenerator` to pick a demo box would be cargo cult (the rule is `scope: MAIN`, so the same finding
in a benchmark or test kernel disappears once that project is marked as test code) - it was also marked on
`SampleDataService`, which the UIModule rebuild deleted; and **S2068 "hard-coded credential"** on
`AccountGetResponse`'s OpenAPI example, where `PasswordHash` is the literal `"type::hash::salt"` — it documents
the *shape* of a stored hash, and the rule fires on the property name, so any literal there would trip it.

**Why:** a finding answered in code stays reviewable and keeps the rule armed for the next occurrence; a
finding answered in config is invisible and switches the rule off for everything matching the path.

**How to apply:** never add an ignore rule to `tooling/sonar-analysis.xml`. Fix the code, mark the assertion
helper, or mark the individual finding in the SonarCloud UI with a reason.
