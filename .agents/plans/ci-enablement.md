# CI, Sonar, and coverage — switch them on

**Status:** Partly done (2026-07-20). `.github/workflows/run-tests.yml` now runs the C# + TS suites and the
ServiceModule integration tests once per DB backend (sqlite/postgres) on every PR. What remains is
Sonar/coverage gating, the docker image build in CI, and making the integration harness run all modules.

## Why
Sonar and coverage are configured but still not enforced on a PR, and the image build only runs at release
time — too late to catch a break.

## What
- ~~A CI workflow that runs the C# and TS suites on every PR.~~ Done — `run-tests.yml`.
- Add the **docker image build** to the PR gate. It runs in CI today only on a published release
  (`release-docker-image.yml`), so a PR never proves the image still builds.
- Wire Sonar analysis and coverage reporting into the PR gate (today `sonar-analysis.yml` is manual only).
- Decide the gate: which suites are required to pass, and the coverage floor.

## Notes
- The integration-test harnesses currently run **core modules only**, not all modules. Enabling every module in
  CI is part of making the gate meaningful, not just green.
- `sonar-analysis.yml` now inlines every coverage command (no wrapper script) and pins the service suite to
  SQLite, so its coverage does not exercise the Azure/Postgres providers. Add extra `.coverage.xml` steps for
  those backends if their provider code needs covering.
