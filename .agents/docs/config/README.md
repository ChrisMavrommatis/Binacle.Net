---
description: config/ — maintainer local-dev tooling: run/test/benchmark/build scripts, the doc-index and tmux scripts, local docker-compose, env files, and emulator state
verified: 2026-06-29
check: Script list, docker-compose services, and env keys match config/
also_update:
  - commands.md
  - samples/README.md
---

# Config

`config/` is the **maintainer's local-dev tooling** — run/test/build scripts, local Docker Compose, env files, and
emulator state. It is **not** a deployment template; user-facing deployment starting points live in
[samples](../samples/README.md). For the quick "how do I run X" reference see [commands.md](../commands.md); this
doc describes what's in the directory.

## Scripts (run from the repo root)

| Script | What it does |
|---|---|
| `api.sh [N\|S\|U\|All]` | Runs the API via `dotnet run -lp <profile>` from `api/src/Binacle.Net/`. Profiles `Normal`/`WithServiceModuleOnly`/`WithUiModuleOnly`/`WithAllModules` (aliases `N/S/U/All`); default `Normal` |
| `tests.sh <alias>` | `dotnet run --project <path>`. Aliases `lib`, `api`, `api_service`, `vipaq`, `performance` |
| `benchmarks.sh [FastValidation\|AlgorithmRacing]` | `dotnet run -c Release --filter <pattern>` from `lib/test/Binacle.Lib.Benchmarks/`. No arg = all |
| `build.sh` | Publishes (`-c Release -o build/output --self-contained --runtime linux-x64`), `docker build -t binacle-net:local`, then `docker compose -f config/docker-compose.build.yml up` |
| `docs.sh` | Regenerates `.agents/docs/_index.md` from each doc's `description:` frontmatter |
| `tmux.sh` | Builds/re-attaches the `binacle` tmux session (windows `api`/`docs`/`web`/`tests`/`misc`/`bench_1..3`); panes are pre-`cd`'d, nothing auto-runs |

The aliases live in bash associative arrays inside each script (`api.sh`, `tests.sh`, `benchmarks.sh`). `docs.sh`
and `tmux.sh` are standalone — they have no aliases.

## Local Docker Compose

| File | Project name | Runs |
|---|---|---|
| `docker-compose.yml` | `binacle-net-services` | **Backing services only** — `aspire-dashboard` + `azurite`. No API. (`postgres`/`minio` present but commented out) |
| `docker-compose.build.yml` | `binacle-net-local` | Local image `binacle-net:local` + `azurite` + `aspire-dashboard`, all modules on; injects `JwtAuth.json` and `OpenTelemetry.Production.json` via compose `configs:`. Used by `build.sh` |

## Env files & emulator state

- `.env` → `COMPOSE_PROJECT_NAME=binacle-net-services`; `.env.build` → `COMPOSE_PROJECT_NAME=binacle-net-local`.
  Both set only the compose project name (no secrets — JWT/OTel/storage values are inlined in the compose files).
- `config/azurite/` and `config/services/azurite/` hold Azurite emulator state (`__azurite_db_*__.json`).
- `config/config.proj` is a `Microsoft.Build.NoTargets` content project (no compile) that includes the config
  files in the solution — see [build-topology.md](../build-topology.md).
