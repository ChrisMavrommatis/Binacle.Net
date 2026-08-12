---
id: tooling
description: tooling/ — every task the repo can run, called by CI and by hand alike: the test, coverage, openapi, agents, changelog, serve, build, image and smoke modules for just, the benchmark/performance scripts, the tmux script, local docker-compose, and emulator state
verified: 2026-08-12
check: Script list, tests.just leaves, coverage.just recipes, openapi.just, agents.just, changelog.just, serve.just, build.just, image.just and smoke.just recipes, and the docker-compose stack/file/service table match tooling/
also_update:
  - commands
  - samples
---

# Tooling

`tooling/` holds **every task the repo can run** — the just modules for tests, coverage, OpenAPI, the
agent indexes, the build, the image stacks and the smoke suite, plus the benchmark scripts, local Docker
Compose and emulator state. CI calls these same recipes rather than keeping its own copy, so a workflow step
and a maintainer typing the command do the same thing. It is **not** a deployment template; user-facing
deployment starting points live in samples (`$samples`). For the quick "how do I run X" reference see
`$commands`; this doc describes what's in the directory.

## Scripts and `just` modules (run from the repo root)

| Script | What it does |
|---|---|
| `serve.just` | **Not a script** — the `serve` module for the root `justfile`, everything run **from source**. `just serve api [profile]` runs the API via `dotnet run -lp <profile>` (`Normal`/`WithServiceModuleOnly`/`WithUiModuleOnly`/`WithAllModules`, aliases `N/S/U/All`, default `Normal`); `just serve docs` and `just serve web` run jekyll + webpack watch together; `just serve services [-d]` / `just serve services-down` bring up what the API talks to |
| `tests.just` | **Not a script** — the `test` module for the root `justfile`. One recipe per suite, run with `just test <leaf>`; see `$commands` for the list |
| `performance.<slice>.sh` | `dotnet run -c Release` for the slice's `PerformanceTests`. Slices `lib`, `vipaq`. Writes to gitignored `PerformanceTests.Artifacts` |
| `benchmarks.<slice>.sh [alias]` | `dotnet run -c Release --filter <pattern>` from the slice's `Benchmarks` project. Slices `lib`, `vipaq`. No arg = all |
| `build.just` | **Not a script** — the `build` module for the root `justfile`. `just build publish` publishes the API (`-c Release -o artifacts/binacle-net --no-self-contained --runtime linux-x64`); `just build image [version]` publishes then `docker build -t binacle-net:<version>` (default `local`), applying the three per-build OCI labels. Neither starts compose, and neither needs `sudo`, so CI calls both as they stand — see `$ci-cd` |
| `coverage.just` | **Not a script** — the `coverage` module for the root `justfile`. Runs the test leaves with the collector attached and writes to gitignored `artifacts/tests/` + `artifacts/coverage/`; see `$commands` |
| `openapi.just` | **Not a script** — the `openapi` module for the root `justfile`. `just openapi generate [dir]` builds the v3/v4 documents into gitignored `artifacts/openapi/`, `just openapi lint [dir]` generates then Spectral-lints them against `.spectral.yaml` |
| `agents.just` | **Not a script** — the `agents` module for the root `justfile`. `just agents all` regenerates the `_index.md` manifest for `.agents/docs`, `.agents/design`, `.agents/plans`, `.agents/memory` and `.agents/ideas` (grouped by area); `just agents generate-index <name>` does one |
| `changelog.just` | **Not a script** — the `changelog` module for the root `justfile`. Reads `CHANGELOG.md` at the repo root. `just changelog extract <version\|Unreleased>` prints one release's section, with its headings promoted from `###` back to `##` for a release body; `just changelog check <version\|Unreleased>` exits 1 if that section is missing or empty. The release workflow calls both, so CI and a laptop parse the file the same way and the exact body can be previewed before a tag is pushed — see `$ci-cd/release-pipeline` |
| `image.just` | **Not a script** — the `image` module for the root `justfile`. Runs what `build.just` produced: `just image up [full\|volume\|bind]` (default `full`) and `just image down [name]`; extra arguments pass through to `docker compose`. `up` creates and opens the bind-mounted folders first, and every stack stops with a pointer to `just build image` if `binacle-net:local` is missing |
| `smoke.just` | **Not a script** — the `smoke` module for the root `justfile`. Tests the image rather than the code. `just smoke test-structure [image]` runs `container-structure-test` against `tooling/smoke/structure.yaml`; `just smoke test <profile> [image]` does up → hurl → down for one profile; `just smoke up`/`down` are the manual halves; `just smoke all [image]` builds, checks the structure once, then runs every profile. Every recipe takes the image last, default `binacle-net:local`, so a published tag can be smoked too |
| `tmux.sh` | Builds/re-attaches the `binacle` tmux session (windows `api`/`docs`/`web`/`tests`/`misc`/`bench_1..3`); panes are pre-`cd`'d, nothing auto-runs |

The launch profiles live in `serve.just`; the benchmark filters live inside the per-slice `benchmarks.*`
scripts. `tmux.sh` is standalone — it has no aliases.

The TS leaves (`just test shared-ts-unit`, `just test vipaq-ts-unit`) run jest from the repo root. Run
`just install` first — it does the root `npm install` (the packages are npm workspaces, so one install covers
them all), `bundle install` for both jekyll sites, and copies `assets/` into `docs/` and `web/`.

## Local Docker Compose

**One compose file supports an API run from source; the rest run an image**, and they live in different modules
for that reason. `docker-compose.yml` brings up what the app talks to and no binacle-net at all, so it belongs
to `serve`, alongside `just serve api` (and it is what the Postgres/AzureStorage test leaves need). The three
`docker-compose.*.yml` files follow `just build image` and answer a different question — does the shipped image
work — so they are the `image` module's stacks, and that is why only they check for `binacle-net:local`. The
five under `smoke/` answer a narrower question again — does it work *as configured* — and are driven entirely
by `just smoke`, never by hand.

| File | Module | Command | Project name | Runs |
|---|---|---|---|---|
| `docker-compose.yml` | `serve` | `just serve services` | `binacle-net-services` | **Backing services only** — `aspire-dashboard`, `azurite`, `postgres`. No API |
| `docker-compose.build.yml` | `image` | `just image up full` | `binacle-net-build` | **Full** — local image `binacle-net:local` + `azurite` + `postgres` + `aspire-dashboard`, all modules on; injects `JwtAuth.json` and `OpenTelemetry.Production.json` via compose `configs:`. All three storage backends run; pick one by moving the comment on the connection strings. The `image` module's default |
| `docker-compose.volume.yml` | `image` | `just image up volume` | `binacle-net-volume` | **Simple** — the local image alone, ServiceModule on SQLite, data in a named volume |
| `docker-compose.bind.yml` | `image` | `just image up bind` | `binacle-net-bind` | **Simple** — same, but data bind-mounted to a folder; `BINACLE_DATA_DIR` overrides where |
| `smoke/<profile>.yml` | `smoke` | `just smoke up <profile>` | `binacle-smoke-<profile>` | **Five throwaway stacks** — `minimal`, `quickstart`, `prod`, `service`, `full`, one per smoke profile, and each name is also a `samples/docker/` folder. Storage is a named volume dropped on teardown, so they need no `_prepare`. They take the image from `$BINACLE_IMAGE` (default `binacle-net:local`); `service`/`full` inline `JwtAuth.json` and raise `RateLimiter__ApiUsageAnonymous` so a second run inside the hour does not go red on 429s; `prod` mounts its own `Presets.json` so reading it back proves the config-mount path |

Each file carries its own `name:`, so no `--env-file` is needed to set the project name. Inside `image.just`
the stack name maps to a file in one place, so `up` and `down` cannot disagree about which one it means;
`smoke.just` gets the same guarantee for free, since the profile name **is** the filename.

The smoke stacks are separate files from `samples/` on purpose. They run the image under test and carry
test-only tweaks — a raised rate limit, disposable storage — that a sample a user copies must never have.

**`tooling/smoke/README.md` is the authority on that suite** — what each profile claims, the two rules that
decide whether a check belongs in it (`assert what the image contains and wires, never what the algorithm
computed`; `every check must be able to fail`), and the setup gotchas. It is written for a human, and it is
where the design rationale went when the smoke plan was deleted on 2026-08-07. Read it before changing an
assertion; do not re-derive any of it here.

Which folders `up` prepares: `serve services` needs `tooling/azurite`; `image up full` needs that plus
`tooling/data/logs` and `tooling/data/pack-logs`; `image up bind` needs `BINACLE_DATA_DIR` (default
`tooling/data`); `image up volume` needs none. It opens the **directory** only, never `-R` — the files inside
belong to whoever wrote them (the app as `APP_UID`, azurite as root) and stay writable to that writer, so a
recursive `chmod` would fail on them while making nothing more writable. `sudo` is used only for a directory
docker created itself, which the daemon makes as root. The few lines that do this are **copied** into both
modules rather than shared: a module reaching into another one restores the coupling the split removed.

## Emulator state
- `tooling/azurite/` holds Azurite emulator state (`__azurite_db_*__.json`).
- `tooling/tooling.proj` is a `Microsoft.Build.NoTargets` content project (no compile) that includes the config
  files in the solution — see `$build-topology`.
