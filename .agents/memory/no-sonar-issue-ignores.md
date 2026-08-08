---
name: no-sonar-issue-ignores
description: Sonar findings are answered in code, never with a sonar.issue.ignore rule in config/sonar-analysis.xml
type: decision
---

`config/sonar-analysis.xml` carries no `sonar.issue.ignore.multicriteria` entries, and none should be added.
A suppression there hides a finding from everyone reading the code, in a file nobody opens.

When a rule is wrong about our code, answer it in the code so the answer is visible:

- **S2699 "tests should include assertions"** — restructure so the assert sits in the test body
  (see the arrange/act/assert memory), or mark the helper that does the asserting with `[AssertionMethod]`.
  The analyser matches that attribute **by name alone**: no package, any namespace. It is declared twice,
  in `shared/test/Binacle.TestsKernel/AssertionMethodAttribute.cs` and
  `vipaq/test/Binacle.ViPaq.UnitTests/AssertionMethodAttribute.cs`, because ViPaq.UnitTests deliberately
  does not reference TestsKernel (`$vipaq/dependencies`). C# S2699 has no rule parameters, so the Java
  `customAssertionMethods` advice found in Sonar community threads does not transfer.
- **S6418 "hard-coded secret"** on a dev placeholder — change the value so it stops looking like a
  credential, rather than silencing the file. A path ignore would also blind that file to a real secret
  pasted in later.

If a rule genuinely does not apply to this project, turn it off in the SonarCloud quality profile, where it
is visible to everyone, not in the analysis file.

Two things the attribute cannot reach, worth knowing before reworking a test: it does not survive a delegate
hop (a test reaching its assert through a `Dictionary<Type, Action>` shows the analyser only `Action.Invoke`),
and there is no code fix for the jwt.io sample JWT in the `TokenResponse` OpenAPI example — that one is marked
False Positive in the SonarCloud UI.

**Why:** a finding answered in code stays reviewable and keeps the rule armed for the next occurrence; a
finding answered in config is invisible and switches the rule off for everything matching the path.

**How to apply:** never add an ignore rule to `config/sonar-analysis.xml`. Fix the code, mark the assertion
helper, or take it to the quality profile.
