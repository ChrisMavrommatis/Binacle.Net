---
description: Build & workspace topology — the .slnx solution, npm workspaces, gulp asset copy, Directory.Build.props, the Dockerfile/build.sh chain, and the NoTargets content projects
verified: 2026-07-06
check: Solution structure, Directory.Build.props, Dockerfile, and content .proj files match the repo root
also_update:
  - commands.md
  - samples/README.md
---

# Build Topology

How the pieces fit together — the solution, the JS workspaces, the asset copy, the shared MSBuild props, and the
Docker build. For the commands themselves see [commands.md](commands.md).

## Solution — `Binacle.Net.slnx`

The repo uses the XML `.slnx` solution format. Projects are grouped by solution folder, mirroring the repo slices:

- `/lib/src/`, `/lib/test/` — `Binacle.Lib(.Abstractions)` + the three lib test projects
- `/api/src/`, `/api/test/` — `Binacle.Net`, `Binacle.Net.Kernel`, the three modules (+ ServiceModule.Domain/.Infrastructure), and the two integration-test projects
- `/vipaq/src/`, `/vipaq/test/`, `/shared/src/`, `/shared/test/` — ViPaq + its tests + `Binacle.Geometry` and `Binacle.CompactNotation` (in `shared/src`) + `Binacle.TestsKernel` and `Binacle.CompactNotation.UnitTests` (in `shared/test`)
- `/samples/docker/` (4 `.dcproj`), `/samples/kubernetes/` (`.proj`), `/results/`, `/api/` (requests), `/build/`
- Top-level content projects: `assets/assets.proj`, `config/config.proj`, `docs/docs.proj`, `web/web.proj`
- `/_root/` — loose files (`.dockerignore`, `.editorconfig`, `Dockerfile`, `gulpfile.js`, `package.json`, README)

## Shared C# props — `Directory.Build.props`

Applies to **every** C# project in the repo. Only three properties:

```xml
<TargetFramework>net10.0</TargetFramework>
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
```

So all C# is .NET 10, nullable-enabled, implicit-usings on. (No `LangVersion` or version props are set here.)

## JS workspaces & asset copy

Root `package.json` (name `binacle-net`, `private`) declares npm workspaces `packages/*` and `vipaq/packages/binacle-vipaq`.
Its only dev dependency is `gulp`, and its only scripts are the asset-copy tasks:

- `npm run copy-assets-to-docs` → `gulp copy-assets-to-docs`
- `npm run copy-assets-to-web` → `gulp copy-assets-to-web`

`gulpfile.js` copies shared `assets/` (images, js, css, fonts) into the `docs/` and `web/` Jekyll sites. The sites
do their own webpack bundling separately (see [docs](docs/README.md) / [web](web/README.md)).

## Docker build chain

The Dockerfile is **single-stage** — the publish happens outside it, in `config/build.sh`:

1. `build.sh` runs `dotnet publish -c Release -o build/output --self-contained --runtime linux-x64` of
   `api/src/Binacle.Net/Binacle.Net.csproj`.
2. `Dockerfile` (`mcr.microsoft.com/dotnet/aspnet:10.0`) does `COPY ["build/output", "."]`, sets
   `ARG VERSION → ENV BINACLE_VERSION`, `USER $APP_UID`, `ENTRYPOINT ["dotnet", "Binacle.Net.dll"]`.
3. `build.sh` then `docker build -t binacle-net:local .` and brings up `config/docker-compose.build.yml`.

There is no `EXPOSE`/`ASPNETCORE_HTTP_PORTS` in the Dockerfile — the aspnet:10.0 base defaults to port 8080;
compose/k8s map it. `build/` is **output only** (generated `output/`, `obj/`, `bin/`) — never edit it.

## Content projects (`Microsoft.Build.NoTargets`)

Several `.proj` files don't compile anything — they use the `Microsoft.Build.NoTargets` SDK to pull non-code files
into the solution (and travel with build output): `config/config.proj`, `docs/docs.proj`, `web/web.proj`,
`assets/assets.proj`, `api/requests/requests.proj`, `results/*/*.proj`, `samples/kubernetes/*/*.proj`. The Docker
samples use `Microsoft.Docker.Sdk` `.dcproj` files instead. None of these affect the C# build.

## `config/` vs `samples/`

`config/` is the **maintainer's local-dev tooling** — the run scripts (`api.sh`, the per-slice `tests.*`,
`performance.*`, `benchmarks.*`, `build.sh`, `agents-index.sh`, `tmux.sh`), local compose files, `.env`/`.env.build`,
and emulator state. `samples/` are
**user-facing deployment starting points** to copy and run the published image. See [commands.md](commands.md) for
the scripts and [samples](samples/README.md) for the deployment examples.
