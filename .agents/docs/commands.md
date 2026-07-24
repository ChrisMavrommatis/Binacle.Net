---
id: commands
description: How to run the API, tests, benchmarks, and build the Docker image
verified: 2026-07-24
check: Aliases and scripts match config/*.sh; docker-compose.yml service list matches config/docker-compose.yml
---

# Commands

All scripts live in `config/` and are run from the repo root. For the `config/` directory anatomy (scripts, local
compose, env, emulator state) see `$config`.

## Run the API

```bash
./config/api.sh [N|S|U|All]
```

- `N` / `Normal` — core API only (default)
- `S` / `WithServiceModuleOnly` — with ServiceModule (auth, rate limiting)
- `U` / `WithUiModuleOnly` — with UIModule
- `All` / `WithAllModules` — everything

## Run Tests

Unit + integration tests, one script per slice (`tests.<slice>.sh`):

```bash
./config/tests.lib.sh                  # lib C# unit
./config/tests.vipaq.sh [cs|ts]        # vipaq C# unit and/or TS (no arg runs both)
./config/tests.api.sh [core|service]   # api integration (no arg runs both)
./config/tests.shared.sh [cs|ts]       # compact-notation C# and/or TS
```

## Coverage

Runs every C# suite and both TS packages, then merges them into one report. Needs Azurite up — without it the
service suite fails and the script writes no report.

```bash
./config/coverage.sh                   # -> CoverageArtifacts/html/index.html
```

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
`postgres`. It does **not** run the API. Run the API itself with `./config/api.sh`.

To build the API image locally and run it with all modules on, use `./config/build.sh` — it publishes, builds
`binacle-net:local`, and brings up `config/docker-compose.build.yml` (the local image + azurite + postgres +
aspire).

## Regenerate the agent indexes

```bash
./config/agents-index.sh
```

Rewrites the `_index.md` manifest for `.agents/docs`, `.agents/design`, `.agents/plans`, `.agents/ideas`, and
`.agents/memory` (grouped by area). Each entry's description comes from the file's `description:` frontmatter,
falling back to its first heading. Run it after adding, renaming, or re-describing any
`.agents/{docs,design,plans,ideas,memory}/*.md` file.

## Dev session (tmux)

```bash
./config/tmux.sh
```

Builds (or re-attaches to) a tmux session named `binacle` with windows `api`, `docs`, `web`, `tests`, `misc`, and
`bench_1`/`bench_2`/`bench_3`. Each pane is pre-`cd`'d to the right folder but runs nothing automatically — it's a
staging layout for the `config/*.sh` scripts. Requires `tmux`.

## Build (Docker image)

```bash
./config/build.sh
```

## JS Packages (npm workspaces at root)

```bash
npm install
npm run copy-assets-to-docs
npm run copy-assets-to-web
```

## ViPaq TypeScript tests

```bash
cd vipaq/packages/binacle-vipaq && npm test
```

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
