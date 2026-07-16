# Idea: run the tests in CI, and wire up Sonar

**Status:** Unvetted idea. The pieces exist in the tree, nothing is switched on, nothing is decided.

**No workflow runs a test.** The workflows under `.github/workflows/` build the docs, build the site, and
publish the docker image. Every suite is green on a laptop and nothing checks that on a machine. Coverage
measured locally is a curiosity; the value of CI is a machine that objects when a change breaks something.

That is the gap. Everything below is a possible way to close it, not a commitment.

## What is already in the tree

Built and verified locally, but never run on GitHub:

- `.github/workflows/sonar-analysis.yml` — manual (`workflow_dispatch`), Azurite as a service container,
  begin → script → end.
- `config/coverage.sonar.sh` — builds, runs all five C# suites + TS, emits the formats Sonar reads.
- `config/coverage.sh` — the local HTML report (cobertura + ReportGenerator). Baseline **49.4% line /
  37.9% branch**.

The begin/end round-trip is the one part never exercised — it cannot be tested off GitHub.

## Two Sonar workflows currently compete

`build.yml` (`name: SonarQube`) and `sonar-analysis.yml` both analyse the same Sonar project. Whichever runs
last overwrites the other.

| | `build.yml` | `sonar-analysis.yml` |
|---|---|---|
| Runs tests | no — `dotnet build` only | yes, 5 C# suites + TS |
| Coverage | **none** | C# (VS xml) + TS (lcov) |
| Runner | windows-latest | ubuntu-latest |

`build.yml` is SonarCloud's copy-paste tutorial. It reports no coverage, so running it after the other
replaces a real number with 0% — and the run still looks successful. **Only one should survive.** `build.yml`
has two things worth keeping either way: **pinned action SHAs** (a tag can be moved, a SHA cannot) and
**scanner caching**.

## Open questions

- **What should trigger it?** Tests on every PR with Sonar manual is the cheap option. Full analysis per PR
  gives per-diff coverage — "is the code in *this* PR tested?" — which is the version that changes behaviour,
  since a whole-repo percentage is not actionable by anyone. Costs a Sonar analysis per PR.
- **Is Sonar wanted at all?** Tests in CI stand on their own. Sonar is a separate question and the two do not
  have to be answered together.
- **The plan tier.** The project is on the **Free** plan — main branch only, which is why a feature branch
  reads "Not analyzed". The repo is public and Sonar's **OSS plan is free with unlimited branch and PR
  analysis**, set at the *organization* level. Without it, per-PR coverage is impossible.
- **The gate.** Sonar Way wants 80% coverage on new code against a 49.4% baseline, so the first runs go red.
  A ratchet ("must not drop") over a target ("must reach 80%") avoids rewarding worthless tests on easy code.
  Possibly `sonar.coverage.exclusions` for `UIModule` (0% by choice) and `DiagnosticsModule` (logging).

## Manual steps, if it is ever switched on

- Automatic Analysis must be turned **off** — it cannot import coverage (it never runs tests) and it
  conflicts with CI analysis. Main's last result is a failed run from ~4 months ago.
- `SONAR_TOKEN` (secret), `SONAR_PROJECT_KEY` and `SONAR_ORGANIZATION` (variables).
- `workflow_dispatch` shows no "Run workflow" button until the file is on the **default branch** — it cannot
  be triggered from a feature branch, and `gh workflow run --ref` does not help (it needs the workflow to
  have appeared in the Actions tab once).

## Notes worth keeping

**Everything in this area fails quietly.** Cobertura fed to Sonar (it accepts neither for C# nor TS),
unresolvable lcov paths, a failed suite, Azurite down — each produces a green run and a plausible number
rather than an error. Azurite being down once yielded a confident **42%**. `config/coverage.sh` now refuses
to write a report unless every suite passed. Never read a coverage number from a run nobody watched pass.

## Related

- `$ideas/mutation-testing` — whether the covered code is actually checked. Too slow for per-run CI.
