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

## One set of scripts, run by both CI and a human

Today the workflows inline their own `dotnet test` / `dotnet build` commands while `config/` holds a parallel set
of scripts doing the same thing. The two drift: a step added to CI is missing locally, a flag fixed locally never
reaches CI, and "works on my machine" becomes a real answer. `sonar-analysis.yml` already inlines every coverage
command with no wrapper at all.

The fix is one entry point per job — build, test, coverage, image — living in `config/`, with CI calling the same
script a maintainer calls. CI keeps only what is genuinely CI's: checkout, SDK setup, service containers, caching,
and the matrix. Anything that decides *what runs* belongs in the script.

- Give every job a script: build, test (per slice), coverage, docker image.
- Rewrite the workflow steps to call them, passing the leg through arguments or env (`BINACLE_TEST_INFRA` already
  works this way).
- Keep the scripts argument-driven and quiet about their environment, so the same call works on a laptop and on a
  runner.
- Watch the split: `config/build.sh` currently starts compose in the foreground and cannot hand the terminal back,
  so publish + `docker build` needs separating from "run it" before either CI or a smoke run can use it.

Related but separately owned: smoke-testing the built image is its own piece of work, handled in its own session —
see the idea file for it.

## Notes
- The integration-test harnesses currently run **core modules only**, not all modules. Enabling every module in
  CI is part of making the gate meaningful, not just green.
- `sonar-analysis.yml` now inlines every coverage command (no wrapper script) and pins the service suite to
  SQLite, so its coverage does not exercise the Azure/Postgres providers. Add extra `.coverage.xml` steps for
  those backends if their provider code needs covering.
