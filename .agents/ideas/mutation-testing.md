---
description: mutation testing with Stryker.NET
---

# Idea: mutation testing with Stryker.NET

**Status:** Unvetted idea. Nothing adopted, nothing decided. Worth one contained experiment before any
opinion is formed.

Coverage answers "did this line run?". Mutation testing answers "if this line were **wrong**, would any test
fail?" It changes the code on purpose — flips `>` to `>=`, `&&` to `||`, deletes a statement — reruns the
suite, and reports each mutant as **killed** (a test failed, good) or **survived** (everything passed, so the
line is covered but never actually checked).

It is the one measure that looks at **assertions** rather than execution. A test that calls a method and
asserts nothing scores 100% coverage and kills no mutants.

`dotnet-stryker`, documented on Microsoft Learn (`Mutation testing - .NET`).

## Why it might be worth it here

`lib/src/Binacle.Lib/ResultSelection/` is the natural first target:

- It reports ~88% line and ~88% branch — a healthy shape. Mutation testing says whether that is real.
- `BestBin_v2.cs:24` adds a magic `1000` to rank fully-packed bins first. That constant **is** a business
  rule. Coverage proves the line runs; it cannot prove a single test would notice if the rule inverted.
- Its failures are **silent**. A wrong bin choice returns 200 and looks fine — the API never complains, and
  a customer finds out months later. Silent-failure code is exactly where mutation testing pays.

The manual version already works: deleting the cancellation guard in `LoopBinProcessor` failed exactly two
tests, which is what proved the guard was tested rather than merely covered. Stryker is that, automated
across every operator.

## Open questions

- **Scope.** The lib suite is ~8,700 tests and Stryker reruns them per mutant. Whole-repo is likely hours.
  Start at `ResultSelection/` (five small files) and see what the run costs before going wider.
- **Is the answer actionable?** If `ResultSelection` scores well, the exercise ends there and the idea is
  closed as "checked, not needed". That is a fine outcome and worth knowing.
- **Equivalent mutants.** Some changes do not alter behaviour, so no test can kill them. They are noise and
  they cap the achievable score — a reason never to treat the score as a target.
- **Global tool or manifest?** `.config/dotnet-tools.json` already pins ReportGenerator and the Sonar
  scanner, so a local pin would match. Only worth it if it is run more than once.
- **CI?** Almost certainly not — too slow for every run. A manual or scheduled job at most.

## Do not

- **Do not chase a mutation score.** Microsoft's own guidance: focus on high-risk, business-critical code
  where an undetected bug costs most. It is another number, and it games the same way coverage does.
- **Do not point it at `UIModule` or `DiagnosticsModule`.** Loud failures, low value — the same reasoning
  that leaves their coverage alone.
