---
description: How to run the API, tests, benchmarks, and build the Docker image
verified: 2026-07-05
check: Aliases and scripts match config/*.sh; docker-compose.yml service list matches config/docker-compose.yml
---

# Commands

All scripts live in `config/` and are run from the repo root. For the `config/` directory anatomy (scripts, local
compose, env, emulator state) see [config/README.md](config/README.md).

## Run the API

```bash
./config/api.sh [N|S|U|All]
```

- `N` / `Normal` — core API only (default)
- `S` / `WithServiceModuleOnly` — with ServiceModule (auth, rate limiting)
- `U` / `WithUiModuleOnly` — with UIModule
- `All` / `WithAllModules` — everything

## Run Tests

```bash
./config/tests.sh <alias>
```

Aliases: `lib`, `api`, `api_service`, `vipaq`, `performance`

To run a single test project directly:
```bash
cd lib/test/<ProjectName> && dotnet run   # lib tests
cd api/test/<ProjectName> && dotnet run   # api tests
cd vipaq/test/<ProjectName> && dotnet run # vipaq tests
```

## Benchmarks

```bash
./config/benchmarks.sh [FastValidation|AlgorithmRacing]
# No argument = all benchmarks
```

## Backing services (Docker Compose)

```bash
docker compose -f config/docker-compose.yml up
```

This starts **only the backing services** — `aspire-dashboard` (OTel) and `azurite` (Azure Storage emulator).
It does **not** run the API. Run the API itself with `./config/api.sh`. (`postgres` and `minio` are present but
commented out in the file.)

To build the API image locally and run it with all modules on, use `./config/build.sh` — it publishes, builds
`binacle-net:local`, and brings up `config/docker-compose.build.yml` (the local image + azurite + aspire).

## Regenerate the agent-docs index

```bash
./config/docs.sh
```

Rewrites `.agents/docs/_index.md` from each doc's `description:` frontmatter. Run it after adding, renaming, or
re-describing any `.agents/docs/*.md` file.

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
