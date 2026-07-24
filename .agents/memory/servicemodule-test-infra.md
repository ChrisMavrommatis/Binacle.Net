---
name: servicemodule-test-infra
description: Test-host config goes through an env var the harness reads, never a .runsettings file — the MTP runner ignores VSTest runsettings
type: gotcha
---

ServiceModule integration tests pick their database backend from the `BINACLE_TEST_INFRA` env var, with an
explicit logged SQLite fallback. The mechanics — values, defaults, overrides, CI legs — are in `$api/tests`.

**Why the env var and not a `.runsettings` file:** this repo runs on the Microsoft Testing Platform, which does
not reliably honour VSTest `.runsettings`. A run parameter there would look wired up and silently do nothing.
The choice is logged for the same reason: a green run must never hide which backend it actually used.

**How to apply:** drive any test-host choice through an env var the harness reads itself, and print what it
chose. Don't add a `.runsettings` file to route config into a test project.
