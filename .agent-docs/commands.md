---
description: How to run the API, tests, benchmarks, and build the Docker image
---

# Commands

All scripts live in `config/` and are run from the repo root.

## Run the API

```bash
./config/api.sh [N|S|U|A]
```

- `N` / `Normal` — core API only (default)
- `S` / `WithServiceModuleOnly` — with ServiceModule (auth, rate limiting)
- `U` / `WithUiModuleOnly` — with UIModule
- `A` / `WithAllModules` — everything

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
cd vipaq/binacle-vipaq && npm test
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
