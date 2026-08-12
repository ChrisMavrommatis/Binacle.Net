# Artifacts

Build output lands here, one folder per thing that produced it:

- `binacle-net/` — the published API, copied into the Docker image by the root `Dockerfile`.
- `docs/` and `web/` — the generated Jekyll sites.
- `openapi/` — the OpenAPI documents emitted on build.
- `tests/` — per-suite test results (`<Project>.ctrf.json`), written by `just test` / `just coverage`.
- `coverage/` — per-suite coverage, one flat folder per consumer (`cobertura/`, `sonar/`), plus the merged
  report in `html-report/`.

These are build artifacts, not source. Do not edit by hand; they are regenerated on each build.

Not to be confused with repo-root `results/`, which holds committed measured evidence - benchmark output and
packing-efficiency reports, kept so a change can be diffed against a known baseline. Those are records that
outlive a build; everything here is regenerated and gitignored.
