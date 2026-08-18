---
id: commands
description: How to set up a clone, run the API and the two sites, run tests and benchmarks, and build the Docker image
verified: 2026-08-18
check: Test leaves match tooling/tests.just; coverage recipes match tooling/coverage.just; openapi recipes match tooling/openapi.just; agents recipes match tooling/agents.just; regen recipes match tooling/regen.just; serve recipes match tooling/serve.just; smoke recipes match tooling/smoke.just; build recipes match tooling/build.just; check recipes match tooling/check.just; install/assets match the root justfile; aliases and scripts match tooling/*.sh; compose service list matches tooling/serve.services.yml; the Prerequisites section still only points at DEVELOPMENT.md and repeats no versions or install commands
paths:
  - "justfile"
  - "tooling/**"
---

# Commands

Setup, running things, tests, coverage, the OpenAPI documents, the image build, the smoke suite, the agent
indexes and the committed generated data are `just` recipes; only the benchmarks, the performance runs and the
tmux session are still scripts in
`tooling/`. All are run
from the repo root. `just` with no arguments lists everything. For the `tooling/` directory anatomy (scripts,
local compose, env, emulator state) see `$tooling`.

## Prerequisites

**`DEVELOPMENT.md` at the repo root is the single source for this** — the tools, their versions, their pin
files, and the commands that install each one, docker and the two smoke binaries included. It is written for a
human setting up a machine.

Do not repeat any of it here, and do not answer a setup question from memory: read that file, or point the user
at it. This section exists only to say where it is.

The short version, for judging whether a command in this doc can run at all: .NET SDK 10.x, Node 22 (`.nvmrc`),
Ruby 3.4.7 (`docs/.ruby-version`, `web/.ruby-version` — **both** sites need it), `just`, and docker for anything
touching the image.

## Set up a fresh clone

```bash
just install                           # npm workspaces, both jekyll sites' gems, then the asset copy
just assets                            # only the asset copy - after changing anything under assets/
```

`assets` copies `assets/**` into `docs/` and `web/` via gulp. Both sites serve their copy, so a changed logo
does not show up until this runs.

## Run from source

```bash
just serve api [N|S|U|All]             # the API
just serve docs                        # docs site: jekyll serve + webpack watch, one terminal
just serve web                         # marketing site: same
just serve services-up [-d]            # what the API talks to: aspire-dashboard, azurite, postgres
just serve services-down [-v]          # only needed after -d; Ctrl-C is enough otherwise
```

`services-up` runs **no** binacle-net — it is what `just serve api` talks to, and what the Postgres and
AzureStorage test leaves need. Running the *built image* is a different job; see "Run the image" below.

The API launch profiles:

- `N` / `Normal` — core API only (default)
- `S` / `WithServiceModuleOnly` — with ServiceModule (auth, rate limiting)
- `U` / `WithUiModuleOnly` — with UIModule
- `All` / `WithAllModules` — everything

`docs` and `web` run both halves under `concurrently --kill-others`, so one Ctrl-C stops the pair. They need
`just install` to have run first.

## Run Tests

Tests are `just` recipes, not scripts — one recipe per suite ("leaf"), defined in `tooling/tests.just` and
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
just coverage report                   # merge the last run into artifacts/coverage/html-report/index.html
just coverage table                    # re-print the table without re-running
```

The format names the consumer, not the file format — `cobertura` is what the table and the HTML report read,
`sonar` is Visual Studio xml for C# plus lcov for TS. Output is one flat file per suite, named after the project
or package:

| Path | Holds |
|---|---|
| `artifacts/tests/<suite>.ctrf.json` | test results (jest packages write `<package>.jest.json`) |
| `artifacts/coverage/cobertura/<suite>.xml` | coverage for the table and the HTML report |
| `artifacts/coverage/sonar/<suite>.xml` | C# coverage for Sonar; TS is `<package>.info` (lcov) |
| `artifacts/coverage/html-report/` | the merged report, written by `just coverage report` |

The table prints a row per suite (`Passed`/`Failed`/`Skipped`/`Coverage`) and its exit code is the run's verdict.

## OpenAPI documents

```bash
just openapi generate                  # artifacts/openapi/Binacle.Net_v3.json + _v4.json
just openapi generate <dir>            # write them somewhere else (pass an absolute path)
just openapi lint [<dir>]              # generate, then lint with Spectral against tooling/openapi.spectral.yaml
```

Nothing needs to be brought up — the documents come out of the build, not out of a running server:
`Microsoft.Extensions.ApiDescription.Server` starts the app host itself and dumps every registered
`IOpenApiDocument` (`$api/openapi`). The host it starts has no launch profile, so **ServiceModule is off** and
the documents carry no `/api/auth/token` path — the shape the committed specs assume.

Generation is off by default (`-p:GenerateOpenApi=true`, set by the recipe) so an ordinary build doesn't start
the app host. The destination is `-p:OpenApiDir`; MSBuild resolves a relative one against the **project**
directory, which is why the recipe passes an absolute path.

`just openapi lint` is clean — no errors and no warnings. Both documents carry a `servers` entry with a single
relative `/`, set in the shared document transform, so a run that reports `oas3-api-servers` means something
removed it.

## Performance tests

Per slice; write reports to a gitignored scratch folder — see [results/README.md](../../results/README.md) for the
scratch-vs-curated convention:

```bash
./tooling/performance.lib.sh
./tooling/performance.vipaq.sh
```

## Benchmarks

Per slice; BenchmarkDotNet, markdown-only, output pinned next to the project:

```bash
./tooling/benchmarks.lib.sh [FastValidation|AlgorithmRacing|BischoffSuite|Parallelization|ResultSelection]
./tooling/benchmarks.vipaq.sh [Encode|Decode]
# No argument = all
```

## Run the image

Build it first with `just build image`, then:

```bash
just image up                          # same as `up full`
just image up full                     # all modules, all three backends + dashboard
just image up volume                   # the image alone, SQLite, data in a named volume
just image up bind                     # the image alone, SQLite, data in a folder you can open
just image down [name] [-v]            # -v drops the named volumes, postgres included
```

Extra arguments go straight through to `docker compose`. The name is positional, so pass it whenever you pass
a flag — `just image up -d` reads `-d` as the stack name and is rejected.

All three check for `binacle-net:local` and tell you to build it if it is missing. `up` also creates the
bind-mounted folders and opens their permissions, which docker will not do for you. See `$tooling` for which
stack needs which folder.

The backing services for an API run from source are a different thing — that is `just serve services-up`.

## Verify a published image

```bash
just image verify 3.0.0                 # all four checks
just image verify 3.0.0 signature       # one: tags, signature, attestations or metadata
```

Reads Docker Hub, builds nothing, **never logs in**. The version is required and never defaults — a default
rots into a tag nobody meant to check. All four checks run even when one fails, so a failure comes with the
three answers that explain it; exit 1 if any failed.

Only `signature` needs `cosign` (`DEVELOPMENT.md`, pinned) and it fails with that pointer rather than passing
quietly. See `$tooling` for what each check proves.

## Smoke the image

Tests the image rather than the code: what it contains, and what its HTTP surface does with the modules
switched on and off. Needs `container-structure-test` and `hurl` (see `DEVELOPMENT.md`) plus docker.

```bash
just smoke all                         # build binacle-net:local, check its structure, then every profile
just smoke test-structure [image]      # static content only — reads the image, no container, no stack
just smoke test <profile> [image]      # one profile end to end: up -> hurl -> down
just smoke up <profile> [image]        # bring one up and leave it   [minimal|quickstart|prod|service|full]
just smoke down <profile> [-v]         # stop it
```

Every recipe takes the image last and defaults to `binacle-net:local`, so the same suite runs against a local
build or a published tag — `just smoke all binacle/binacle-net:<tag>`. Given anything but the local tag,
`all` pulls instead of building. The stacks read it as `$BINACLE_IMAGE` with the same default.

The static check reads the image, not a container, so `all` runs it **once** rather than once per profile. The
four profiles are declared in one place, the `profiles` variable at the top of `tooling/smoke.just`.

While editing a `.hurl`, skip the up/down cycle: `just smoke up prod`, then `just smoke::_test_profile prod` as
many times as needed, then `just smoke down prod -v`.

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

## Regenerate committed data

```bash
just regen all                         # every generator, in dependency order
just regen or-lib-scenarios            # OR-Library text -> shared/data/bischoff-suite
just regen vipaq-packed-data           # those + custom-problems, packed -> vipaq/data/packed
just regen vipaq-interop-vectors       # the interop pair + header bytes -> vipaq/test-vectors
just regen check                       # regenerate, then fail if any of it changed
```

Four tools whose output is committed. None takes an argument: each runs every generator in its list, so it
cannot half-run and leave the data inconsistent.

`or-lib-scenarios` writes what `vipaq-packed-data` reads, which is why `all` exists — the ordering is the part
that is easy to get wrong by hand. `vipaq-interop-vectors` is one recipe because the C# and TS halves write
`interop/cs` and `interop/ts` from the same `input.json`, and regenerating one alone is the drift the interop
integrity tests catch.

Every run is deterministic, so `check` runs everything and then asks whether the tree moved; it diffs only the
`.json` the generators write, because those folders also hold their own README and `vipaq/test-vectors` holds
hand-authored vectors. **No workflow calls `check`** — it is for the maintainer who edited a tool or a source
problem.

## Dev session (tmux)

```bash
./tooling/tmux.sh
```

Builds (or re-attaches to) a tmux session named `binacle` with windows `api`, `docs`, `web`, `tests`, `misc`, and
`bench_1`/`bench_2`/`bench_3`. Each pane is pre-`cd`'d to the right folder but runs nothing automatically — it's a
staging layout for the `just` recipes and the remaining `tooling/*.sh` scripts. Requires `tmux`.

## Build

```bash
just build publish                     # dotnet publish -> artifacts/binacle-net
just build image [version]             # publish, then docker build -t binacle-net:<version> (default local)
just build docs                        # the documentation site -> artifacts/docs
just build web                         # the marketing site -> artifacts/web
```

`image` always re-publishes first — `docker build` copies whatever is in `artifacts/binacle-net`, so skipping the
publish is how a stale image gets tagged. The version becomes both the image tag and `BINACLE_VERSION` inside
the container, which is what the running app reports.

Then run it with `just image up`, which prepares the bind-mounted folders first.

`docs` and `web` are the build half of `just serve docs` / `just serve web` — same site, built once instead of
served and watched. Three steps in a fixed order: copy the assets, run webpack over `_js/`, then
`jekyll build` with `_config.yml,_config.prod.yml`. **Skipping any of them still produces a site**, just one
with no scripts and no logo, because `js/`, `lib/` and `media/` are gitignored and filled by the first two
steps. The prod config overrides the three values that differ off localhost: the site url, the api url and the
analytics container.

## Checks

```bash
just check links                       # internal links in both built sites
just check links docs                  # one of them
just check links-external docs         # every link, other people's servers included
just check workflows                   # actionlint over .github/
```

`workflows` takes no arguments — actionlint finds `.github/workflows` from the repo root and follows
`uses: ./.github/actions/*` into the composite actions. It catches what a workflow file cannot be tested for
without running it: a bad expression, an undefined `needs`, an invalid runner label, an unquoted shell
variable. **It needs shellcheck installed as well as actionlint** — actionlint hands every `run:` block to it
when it can find one, and the runners already have it, so a laptop without it checks less than CI does.

Needs `lychee` (pinned, installed from `DEVELOPMENT.md` like the smoke tools) and needs the site built first —
the recipe stops with `No artifacts/docs` rather than checking nothing and passing. It reads the built output
because a source `href` is still Liquid at that point.

`links` passes `--offline`, so it checks only the links that resolve inside the site — the ones a renamed page
breaks. Both sites together answer in about a fifth of a second, which is why the deploy workflows run it as a
pre-flight. `links-external` makes a real request per unique URL, takes ten seconds, and can fail on somebody
else's outage; that is why it is a separate recipe rather than a flag, and why it is not a gate.

`tooling/lychee.toml`, named with `--config`, holds the URLs it must never check — the `localhost` samples in
the quickstart pages, which are correct on the page and unreachable from anywhere else.

## JS Packages (npm workspaces at root)

`just install` covers them — it is the root `npm install`, so one install covers every workspace package.

## TypeScript packages

Both are test leaves — `just test shared-ts-unit` and `just test vipaq-ts-unit`. They run jest from the repo
root through the root `jest.config.js`, which is what keeps the workspace folder in coverage paths and applies
its `collectCoverageFrom`. Running `npm test` inside a package works but uses that package's own config, so its
numbers are not the ones CI or coverage report.

## Docker

<!-- sourced from docs site; verify against current code if behaviour changes -->

Image: `binacle/binacle-net:latest`. Default internal port: `8080`. `latest` is right for an ad-hoc run of the
newest image; a **sample** never uses it (`$samples#image-pin` has the pinning rule).

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
