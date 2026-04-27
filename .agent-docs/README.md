---
description: Repo overview and index of agent documentation
---

# Binacle.Net — Agent Docs

Binacle.Net is an API that checks if items fit in boxes (fit) and packs them with position data (pack).
Built with ASP.NET Core (.NET 10) Minimal APIs. Main code is C#.

## Repo Layout

| Path | What it is |
|---|---|
| `src/Binacle.Net` | Main API — entry point, versioned endpoints, `Program.cs` |
| `src/Binacle.Net.Kernel` | Shared tools: endpoint registration, OpenAPI, feature flags, validation |
| `src/Binacle.Lib` | Core bin-packing algorithms and processors |
| `src/Binacle.Lib.Abstractions` | Interfaces shared between `Binacle.Lib` and the API layer |
| `src/Binacle.Net.ServiceModule` | Optional: JWT auth, rate limiting, account management |
| `src/Binacle.Net.UIModule` | Optional: Blazor/Razor interactive packing demo |
| `src/Binacle.Net.DiagnosticsModule` | Diagnostics middleware, always on |
| `src/Binacle.ViPaq` | Compact binary format for encoding packing results |
| `test/` | All test projects |
| `packages/` | TypeScript packages (npm workspaces) |
| `gems/` | Ruby gems (Jekyll plugins) |
| `docs/` | Jekyll documentation site |
| `web/` | Jekyll marketing/web site |

## Commands

All scripts live in `config/` and are run from the repo root.

### Run the API

```bash
./config/api.sh [N|S|U|A]
```

- `N` / `Normal` — core API only (default)
- `S` / `WithServiceModuleOnly` — with ServiceModule (auth, rate limiting)
- `U` / `WithUiModuleOnly` — with UIModule
- `A` / `WithAllModules` — everything

### Run Tests

```bash
./config/tests.sh <alias>
```

Aliases: `lib`, `api`, `api_service`, `vipaq`, `performance`

To run a single test project directly:
```bash
cd test/<ProjectName> && dotnet run
```

### Benchmarks

```bash
./config/benchmarks.sh [FastValidation|AlgorithmRacing]
# No argument = all benchmarks
```

### Build (Docker image)

```bash
./config/build.sh
```

### JS Packages (npm workspaces at root)

```bash
npm install
npm run copy-assets-to-docs
npm run copy-assets-to-web
```

### ViPaq TypeScript tests

```bash
cd packages/binacle-vipaq && npm test
```

## Slice Docs

- [Concepts](concepts/README.md) — overarching ideas that span multiple slices
- [API](api/README.md) — `Binacle.Net` and `Binacle.Net.Kernel`
- [Lib](lib/README.md) — `Binacle.Lib` and `Binacle.Lib.Abstractions`
- [Tests](tests/README.md) — all test projects
- [ViPaq](vipaq/README.md) — `Binacle.ViPaq` and `packages/binacle-vipaq`
- [Packages](packages/README.md) — TypeScript packages
- [Gems](gems/README.md) — Ruby gems
- [Docs Site](docs/README.md) — Jekyll docs site
- [Web Site](web/README.md) — Jekyll web site
