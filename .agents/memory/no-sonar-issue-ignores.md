---
name: no-sonar-issue-ignores
description: Sonar findings are answered in code, never with a sonar.issue.ignore rule in tooling/sonar-analysis.xml
type: decision
---

`tooling/sonar-analysis.xml` carries no `sonar.issue.ignore.multicriteria` entries, and none should be added.
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

### The quality-profile escape hatch does not exist here {#no-profile-escape}

Earlier versions of this memory said "if a rule genuinely does not apply, turn it off in the SonarCloud quality
profile". **That is not available on this project.** Custom quality profiles start at the Team plan and the
project is on Free, so "Sonar way" is read-only and no rule can be switched off anywhere. Verified 2026-08-09
against the SonarCloud docs and the `qualitygates/list` API, which reports `actions.create: false`.

That leaves exactly two honest answers to a rule you disagree with, and the ban on path ignores stands for both:

1. **Change the code** so the analyser reads it correctly. Everything below is an example of this.
2. **Mark the individual finding** Accepted or False Positive in the SonarCloud UI, with a reason. It is
   per-finding rather than per-path, so the rule stays armed for the next occurrence, and the reason is on the
   record. This is the free-plan substitute for deactivating a rule, and it is a better one - a profile change
   is silent, a marked finding is not.

A `sonar.issue.ignore` glob in the analysis file remains the wrong answer, and is now the *only* wrong answer
still technically available. That is worth knowing: the easiest lever left is the one we do not pull.

Two things the attribute cannot reach, worth knowing before reworking a test: it does not survive a delegate
hop (a test reaching its assert through a `Dictionary<Type, Action>` shows the analyser only `Action.Invoke`),
and there is no code fix for the jwt.io sample JWT in the `TokenResponse` OpenAPI example — that one is marked
False Positive in the SonarCloud UI.

## Scope exclusions are not issue ignores

`tooling/sonar-analysis.xml` **does** carry `sonar.exclusions`, `sonar.cpd.exclusions` and
`sonar.coverage.exclusions`, and those are a different thing — do not read the rule above as forbidding them.
An issue ignore says "run this rule here, then hide what it finds". A scope exclusion says "this is not our
code, or not this metric's business":

- `sonar.exclusions` drops vendored `assets/lib/**` and the `shared/data/**` fixture corpus. Nobody reviews or
  fixes either, so measuring them only moved the totals.
- `sonar.cpd.exclusions` covers `lib/src/Binacle.Lib/Algorithms/**`, where the v1/v2 variants are parallel
  implementations by design. **Every rule still runs on those files** — only duplication detection stops.
- Support projects are handled in `Directory.Build.props`, not here, via `SonarQubeTestProject` — see the
  build-topology doc. That reclassifies them as test code rather than hiding anything.

The test is whether a reader of the code would want to know. A hidden finding fails it; a file that was never
ours does not.

## Findings with no code answer, marked in the UI

Two survive with nothing honest to change, and are marked in the SonarCloud UI beside the jwt.io JWT above:

- **S2245 "use a cryptographically strong RNG"** on `SampleDataService` (which demo sample set to show) and
  `getRandomInt.ts` (random box sizes in the UI demo). Neither is a security context, and swapping in
  `RandomNumberGenerator` to pick a demo box would be cargo cult. Note the rule is `scope: MAIN`, so the same
  finding in a benchmark or test kernel disappears on its own once that project is marked as test code.
- **S2068 "hard-coded credential"** on `AccountGetResponse`'s OpenAPI example, where `PasswordHash` is the
  literal `"type::hash::salt"`. It documents the *shape* of a stored hash and is not a credential; the rule
  fires on the property name, so any literal there would trip it.

**Why:** a finding answered in code stays reviewable and keeps the rule armed for the next occurrence; a
finding answered in config is invisible and switches the rule off for everything matching the path.

**How to apply:** never add an ignore rule to `tooling/sonar-analysis.xml`. Fix the code, mark the assertion
helper, or mark the individual finding in the SonarCloud UI with a reason. The quality profile is not an
option on the Free plan - see [the escape hatch section](#no-profile-escape).
