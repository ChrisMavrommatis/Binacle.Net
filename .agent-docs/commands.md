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
cd test/<ProjectName> && dotnet run
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
cd packages/binacle-vipaq && npm test
```
