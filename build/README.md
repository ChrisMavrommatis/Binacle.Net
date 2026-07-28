# Build

Build artifacts land here, one folder per thing that produced them:

- `binacle-net/` — the published API, copied into the Docker image by the root `Dockerfile`.
- `docs/` and `web/` — the generated Jekyll sites.
- `openapi/` — the OpenAPI documents emitted on build.
- `tests/` — per-suite test results (`<Project>.ctrf.json`), written by `just test` / `just coverage`.
- `coverage/` — per-suite coverage, one flat folder per consumer (`cobertura/`, `sonar/`), plus the merged
  report in `html-report/`.

These are build artifacts, not source. Do not edit by hand; they are regenerated on each build.
