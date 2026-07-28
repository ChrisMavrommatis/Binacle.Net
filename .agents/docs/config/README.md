---
id: config
description: config/ — maintainer local-dev tooling: the test, coverage, openapi and agents modules for just, run/benchmark/build scripts, the tmux script, local docker-compose, and emulator state
verified: 2026-07-28
check: Script list, tests.just leaves, coverage.just recipes, openapi.just and agents.just recipes, and the docker-compose file/service table match config/
also_update:
  - commands
  - samples
---

# Config

`config/` is the **maintainer's local-dev tooling** — the just modules for tests, coverage, OpenAPI and the
agent indexes, run/build scripts, local Docker Compose, and
emulator state. It is **not** a deployment template; user-facing deployment starting points live in
samples (`$samples`). For the quick "how do I run X" reference see `$commands`; this
doc describes what's in the directory.

## Scripts and `just` modules (run from the repo root)

| Script | What it does |
|---|---|
| `api.sh [N\|S\|U\|All]` | Runs the API via `dotnet run -lp <profile>` from `api/src/Binacle.Net/`. Profiles `Normal`/`WithServiceModuleOnly`/`WithUiModuleOnly`/`WithAllModules` (aliases `N/S/U/All`); default `Normal` |
| `tests.just` | **Not a script** — the `test` module for the root `justfile`. One recipe per suite, run with `just test <leaf>`; see `$commands` for the list |
| `performance.<slice>.sh` | `dotnet run -c Release` for the slice's `PerformanceTests`. Slices `lib`, `vipaq`. Writes to gitignored `PerformanceTests.Artifacts` |
| `benchmarks.<slice>.sh [alias]` | `dotnet run -c Release --filter <pattern>` from the slice's `Benchmarks` project. Slices `lib`, `vipaq`. No arg = all |
| `build.sh` | Publishes (`-c Release -o build/binacle-net --self-contained --runtime linux-x64`), `docker build -t binacle-net:local`, then `docker compose -f config/docker-compose.build.yml up` |
| `coverage.just` | **Not a script** — the `coverage` module for the root `justfile`. Runs the test leaves with the collector attached and writes to gitignored `build/tests/` + `build/coverage/`; see `$commands` |
| `openapi.just` | **Not a script** — the `openapi` module for the root `justfile`. `just openapi generate [dir]` builds the v3/v4 documents into gitignored `build/openapi/`, `just openapi lint [dir]` generates then Spectral-lints them against `.spectral.yaml` |
| `agents.just` | **Not a script** — the `agents` module for the root `justfile`. `just agents all` regenerates the `_index.md` manifest for `.agents/docs`, `.agents/design`, `.agents/plans`, `.agents/memory` and `.agents/ideas` (grouped by area); `just agents generate-index <name>` does one |
| `tmux.sh` | Builds/re-attaches the `binacle` tmux session (windows `api`/`docs`/`web`/`tests`/`misc`/`bench_1..3`); panes are pre-`cd`'d, nothing auto-runs |

The aliases/kinds live inside each script (`api.sh` and the per-slice `performance.*`, `benchmarks.*`).
`tmux.sh` is standalone — it has no aliases.

The TS leaves (`just test shared-ts-unit`, `just test vipaq-ts-unit`) run jest from the repo root. Run
`npm install` at the root first — the packages are npm workspaces, so one install covers them all.

## Local Docker Compose

| File | Project name | Runs |
|---|---|---|
| `docker-compose.yml` | `binacle-net-services` | **Backing services only** — `aspire-dashboard`, `azurite`, `postgres`. No API. |
| `docker-compose.build.yml` | `binacle-net-build` | **Full** — local image `binacle-net:local` + `azurite` + `postgres` + `aspire-dashboard`, all modules on; injects `JwtAuth.json` and `OpenTelemetry.Production.json` via compose `configs:`. All three storage backends run; pick one by moving the comment on the connection strings |
| `docker-compose.volume.yml` | `binacle-net-volume` | **Simple** — the local image alone, ServiceModule on SQLite, data in a named volume |
| `docker-compose.bind.yml` | `binacle-net-bind` | **Simple** — same, but data bind-mounted to a folder; `BINACLE_DATA_DIR` overrides where |

Each file carries its own `name:`, so no `--env-file` is needed to set the project name.

## Emulator state
- `config/azurite/` holds Azurite emulator state (`__azurite_db_*__.json`).
- `config/config.proj` is a `Microsoft.Build.NoTargets` content project (no compile) that includes the config
  files in the solution — see `$build-topology`.
