# CI - put Sonar and coverage on the PR gate

**Status:** Not started. After v3.0.0.

## Why

Sonar and coverage are configured but never enforced. `.github/workflows/sonar-analysis.yml` is
`workflow_dispatch` only, so analysis happens when somebody remembers, which is never on the PR that introduced
the problem.

## What

- Run Sonar analysis and coverage reporting on every PR.
- Decide the gate: which suites must pass, and the coverage floor. A floor nobody agreed on gets waived the
  first time it blocks something, so pick a number that is true today and ratchet it.
- Keep Automatic Analysis OFF. Coverage needs a CI run - Automatic Analysis only reads source, and the two
  fight.

## Watch out

- Build + coverage must sit between `Sonar begin` and `Sonar end`; the scanner only sees projects compiled in
  that pair. A failing suite skips `Sonar end`, so a failed run publishes nothing - that is deliberate.
- Sonar needs full git history (`fetch-depth: 0`) to tell new code from old. A shallow clone makes everything
  look new.
- `sonar-analysis.yml` inlines every coverage command with no wrapper, and pins the service suite to SQLite - so
  its coverage never exercises the Azure or Postgres provider code. Add `.coverage.xml` steps for those backends
  if that code needs covering.

## Done when

A PR gets a coverage number and a Sonar verdict without anyone pressing a button.
