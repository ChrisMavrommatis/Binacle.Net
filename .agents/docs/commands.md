---
id: commands
description: How to set up a clone, run the API and the two sites, run tests and benchmarks, and build the Docker image
verified: 2026-07-28
check: Test leaves match config/tests.just; coverage recipes match config/coverage.just; openapi recipes match config/openapi.just; agents recipes match config/agents.just; serve recipes match config/serve.just; install/assets match the root justfile; aliases and scripts match config/*.sh; docker-compose.yml service list matches config/docker-compose.yml
---

# Commands

Setup, running things, tests, coverage, the OpenAPI documents and the agent indexes are `just` recipes; the
benchmarks, performance runs, the image build and the tmux session are still scripts in `config/`. All are run
from the repo root. `just` with no arguments lists everything. For the `config/` directory anatomy (scripts,
local compose, env, emulator state) see `$config`.

## Set up a fresh clone

```bash
just install                           # npm workspaces, both jekyll sites' gems, then the asset copy
just assets                            # only the asset copy - after changing anything under assets/
```

`assets` copies `assets/**` into `docs/` and `web/` via gulp. Both sites serve their copy, so a changed logo
does not show up until this runs.

## Run something locally

```bash
just serve api [N|S|U|All]             # the API
just serve docs                        # docs site: jekyll serve + webpack watch, one terminal
just serve web                         # marketing site: same
```

The API launch profiles:

- `N` / `Normal` — core API only (default)
- `S` / `WithServiceModuleOnly` — with ServiceModule (auth, rate limiting)
- `U` / `WithUiModuleOnly` — with UIModule
- `All` / `WithAllModules` — everything

`docs` and `web` run both halves under `concurrently --kill-others`, so one Ctrl-C stops the pair. They need
`just install` to have run first.

## Run Tests

Tests are `just` recipes, not scripts — one recipe per suite ("leaf"), defined in `config/tests.just` and
loaded as the `test` module. `just --list test` lists them; the same recipes are what CI calls, so a red step
is the line to paste here.

```bash
just test all                          # every leaf that needs nothing brought up
just test lib-unit                     # lib C# unit
just test shared-cs-unit               # compact-notation C#
just test shared-ts-unit               # compact-notation TS
just test vipaq-cs-unit                # vipaq C#
just test vipaq-ts-unit                # vipaq TS
just test api-core-unit                # Binacle.Net options validators, forwarded headers
just test api-kernel-unit              # Kernel features
just test api-diagnostics-unit         # DiagnosticsModule
just test api-service-unit             # ServiceModule config validators and policies
just test api-core-integration         # v3 + v4 HTTP endpoints
just test api-service-integration [Sqlite|Postgres|AzureStorage]   # no arg falls back to SQLite
```

`DOTNET_TEST_ARGS` is appended to every `dotnet test` leaf, which is how CI runs them all against one Release
build: `DOTNET_TEST_ARGS="--configuration Release --no-build" just test all`.

## Coverage

Coverage is not a second run — the collector rides along inside the test run, so these are the same leaves
`just test all` runs, asked for extra output. Needs nothing brought up: the ServiceModule leaf uses SQLite.

```bash
just coverage all                      # every suite + the table (cobertura)
just coverage all sonar                # the formats Sonar imports
just coverage report                   # merge the last run into build/coverage/html-report/index.html
just coverage table                    # re-print the table without re-running
```

The format names the consumer, not the file format — `cobertura` is what the table and the HTML report read,
`sonar` is Visual Studio xml for C# plus lcov for TS. Output is one flat file per suite, named after the project
or package:

| Path | Holds |
|---|---|
| `build/tests/<suite>.ctrf.json` | test results (jest packages write `<package>.jest.json`) |
| `build/coverage/cobertura/<suite>.xml` | coverage for the table and the HTML report |
| `build/coverage/sonar/<suite>.xml` | C# coverage for Sonar; TS is `<package>.info` (lcov) |
| `build/coverage/html-report/` | the merged report, written by `just coverage report` |

The table prints a row per suite (`Passed`/`Failed`/`Skipped`/`Coverage`) and its exit code is the run's verdict.

## OpenAPI documents

```bash
just openapi generate                  # build/openapi/Binacle.Net_v3.json + _v4.json
just openapi generate <dir>            # write them somewhere else (pass an absolute path)
just openapi lint [<dir>]              # generate, then lint with Spectral against .spectral.yaml
```

Nothing needs to be brought up — the documents come out of the build, not out of a running server:
`Microsoft.Extensions.ApiDescription.Server` starts the app host itself and dumps every registered
`IOpenApiDocument` (`$api/openapi`). The host it starts has no launch profile, so **ServiceModule is off** and
the documents carry no `/api/auth/token` path — the shape the committed specs assume.

Generation is off by default (`-p:GenerateOpenApi=true`, set by the recipe) so an ordinary build doesn't start
the app host. The destination is `-p:OpenApiDir`; MSBuild resolves a relative one against the **project**
directory, which is why the recipe passes an absolute path.

`just openapi lint` currently reports two `oas3-api-servers` warnings and no errors — that's the parked
`servers` decision, not a regression.

## Performance tests

Per slice; write reports to a gitignored scratch folder — see [results/README.md](../../results/README.md) for the
scratch-vs-curated convention:

```bash
./config/performance.lib.sh
./config/performance.vipaq.sh
```

## Benchmarks

Per slice; BenchmarkDotNet, markdown-only, output pinned next to the project:

```bash
./config/benchmarks.lib.sh [FastValidation|AlgorithmRacing|BischoffSuite|Parallelization|ResultSelection]
./config/benchmarks.vipaq.sh [Encode|Decode]
# No argument = all
```

## Backing services (Docker Compose)

```bash
docker compose -f config/docker-compose.yml up
```

This starts **only the backing services** — `aspire-dashboard` (OTel), `azurite` (Azure Storage emulator), and
`postgres`. It does **not** run the API. Run the API itself with `just serve api`.

To run the API image locally with all modules on, build it with `./config/build.sh` first, then bring up
`config/docker-compose.build.yml` yourself (the local image + azurite + postgres + aspire).

## Regenerate the agent indexes

```bash
just agents all                        # all five
just agents generate-index plans       # one [docs|design|plans|memory|ideas]
```

Rewrites the `_index.md` manifest for `.agents/docs`, `.agents/design`, `.agents/plans`, `.agents/memory` and
`.agents/ideas` (grouped by area). Each entry's description comes from the file's `description:` frontmatter,
falling back to its first heading. Run it after adding, renaming, or re-describing any
`.agents/{docs,design,plans,ideas,memory}/*.md` file. A name that isn't one of the five is rejected, so a typo
can't leave an index untouched and look like it worked.

## Dev session (tmux)

```bash
./config/tmux.sh
```

Builds (or re-attaches to) a tmux session named `binacle` with windows `api`, `docs`, `web`, `tests`, `misc`, and
`bench_1`/`bench_2`/`bench_3`. Each pane is pre-`cd`'d to the right folder but runs nothing automatically — it's a
staging layout for the `just` recipes and the remaining `config/*.sh` scripts. Requires `tmux`.

## Build (Docker image)

```bash
./config/build.sh                                            # publish + docker build -t binacle-net:local
docker compose -f config/docker-compose.build.yml up         # then run it
```

`build.sh` also creates `config/data/**` and `config/azurite/` and opens their permissions, because the
containers bind-mount them and write as their own users. The azurite `chmod` needs `sudo`, so the script
prompts. It does not start compose — that is the line above.

## JS Packages (npm workspaces at root)

`just install` covers them — it is the root `npm install`, so one install covers every workspace package.

## TypeScript packages

Both are test leaves — `just test shared-ts-unit` and `just test vipaq-ts-unit`. They run jest from the repo
root through the root `jest.config.js`, which is what keeps the workspace folder in coverage paths and applies
its `collectCoverageFrom`. Running `npm test` inside a package works but uses that package's own config, so its
numbers are not the ones CI or coverage report.

## Docker

<!-- sourced from docs site; verify against current code if behaviour changes -->

Image: `binacle/binacle-net:latest`. Default internal port: `8080`.

Basic run:

```bash
docker run -d --name binacle-net -p 8080:8080 binacle/binacle-net:latest
```

With all UIs and modules enabled:

```bash
docker run -d --name binacle-net -p 8080:8080 \
  -e SWAGGER_UI=True \
  -e SCALAR_UI=True \
  -e UI_MODULE=True \
  binacle/binacle-net:latest
```

Override preset file (read-only bind mount):

```bash
-v $(pwd)/Presets.json:/app/Config_Files/Presets.json:ro
```

Change the internal port (e.g. run on 80 inside the container, expose as 8080 on the host):

```bash
-e ASPNETCORE_HTTP_PORTS=80 -p 8080:80
```

Persist logs — bind a host path to `/app/data/logs` (or `/app/data` for all data):

```bash
-v $(pwd)/data/logs:/app/data/logs
```
