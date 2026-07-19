# CI, Sonar, and coverage — switch them on

**Status:** Deferred, not started. Nothing here blocks a release — but until it lands, **no workflow runs a
test**, so every green suite is green on a laptop only.

## Why
There is no CI test gate. A regression only surfaces if someone runs the suites by hand. Sonar and coverage are
configured but not enforced on a PR.

## What
- A CI workflow that runs the C# and TS suites on every PR, ideally with the docker image build too.
- Wire Sonar analysis and coverage reporting into that workflow.
- Decide the gate: which suites are required to pass, and the coverage floor.

## Notes
- The integration-test harnesses currently run **core modules only**, not all modules. Enabling every module in
  CI is part of making the gate meaningful, not just green.
- No CI today is why a release depends on one careful local green sweep, including the image build.
