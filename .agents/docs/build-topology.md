---
id: build-topology
description: Build & workspace topology — the .slnx solution, npm workspaces, gulp asset copy, Directory.Build.props (including the SonarQubeTestProject rule for support projects), central package management, the global.json test-runner opt-in, the publish/Dockerfile chain, and the NoTargets content projects
verified: 2026-08-21
check: Every solution folder and project count matches Binacle.Net.slnx (43 projects); Directory.Build.props, Directory.Packages.props, global.json and Dockerfile match the repo root; the content .proj list resolves to files that exist; the root package.json scripts and devDependencies match
also_update:
  - commands
  - samples
  - ci-cd
paths:
  - "Binacle.Net.slnx"
  - "Directory.*.props"
  - "global.json"
  - "**/*.csproj"
  - "Dockerfile"
---

# Build Topology

How the pieces fit together — the solution, the JS workspaces, the asset copy, the shared MSBuild props, and the
Docker build. For the commands themselves see `$commands`.

## Solution — `Binacle.Net.slnx`

The repo uses the XML `.slnx` solution format. **43 projects**, grouped by solution folder, mirroring the repo
slices:

- `/lib/src/`, `/lib/test/` — `Binacle.Lib` (the only src project) + four lib test projects, one of them `Binacle.Lib.TestsKernel`
- `/api/src/`, `/api/test/` — `Binacle.Net`, `Binacle.Net.Kernel`, the three modules (+ ServiceModule.Domain/.Infrastructure), two integration-test projects and four unit-test projects (one per source project that has unit tests: `Binacle.Net`, `Kernel`, `DiagnosticsModule`, `ServiceModule`)
- `/vipaq/src/`, `/vipaq/test/`, `/shared/src/`, `/shared/test/` — ViPaq + its tests + `Binacle.Geometry`, `Binacle.CompactNotation` and `Binacle.Packing` (in `shared/src`) + `Binacle.TestsKernel`, `Binacle.TestReporting` and `Binacle.CompactNotation.UnitTests` (in `shared/test`)
- `/vipaq/tools/` (`Binacle.ViPaq.VectorGenerators`, `Binacle.ViPaq.PackedDataGenerator`), `/shared/tools/` (`Binacle.OrLibrary.Converter`) — standalone generators, not referenced by the shipped projects
- `/samples/`, `/samples/docker/` (5 `.dcproj` — quickstart, minimal, full, service, prod), `/samples/kubernetes/` (one `.proj`), `/api/` (requests), `/artifacts/`
- Top-level content projects: `assets/assets.proj`, `tooling/tooling.proj`, `sites/docs/docs.proj`,
  `sites/demo/demo.proj`
- `/_root/` — loose files (`.dockerignore`, `.editorconfig`, `Directory.Build.props`, `Directory.Packages.props`, `Dockerfile`, `global.json`, `gulpfile.js`, `package.json`, README)

## Shared C# props — `Directory.Build.props`

Applies to **every** C# project in the repo. Four unconditional properties:

```xml
<TargetFramework>net10.0</TargetFramework>
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
<NoWarn>$(NoWarn);AD0001</NoWarn>
```

So all C# is .NET 10, nullable-enabled, implicit-usings on. (No `LangVersion` or version props are set here.)

`AD0001` is suppressed because `Xunit.Analyzers`' `MemberDataShouldReferenceValidMember` crashes on valid member
data in xunit 3.2.2 (referenced as `xunit.v3.mtp-v2`, see below) — an analyzer bug, and AD0001 is raised by the
analyzer driver so editorconfig severity cannot reach it. The file carries the full reasoning.

### `SonarQubeTestProject` — the support projects {#sonar-test-projects}

A fifth property is set **conditionally**: any project whose directory path contains `/test/` or `/tools/` gets
`<SonarQubeTestProject>true</SonarQubeTestProject>`. The path is normalised to forward slashes first, because
`MSBuildProjectDirectory` is separator-native and the match would miss on Linux otherwise.

The Scanner for .NET identifies a test project by its `Microsoft.NET.Test.Sdk` reference. That finds the xunit
suites but **not** the eleven support projects that have no such reference — all three test kernels,
`TestReporting`, the two benchmark projects, the two performance suites, and the three generator/converter
tools. Without the property the scanner reads all eleven as product code, which put 1203 lines into the coverage
denominator that no test will ever cover (measured when there were ten, before `Binacle.Lib.TestsKernel` was
split out, so the real figure is now a little higher) and ran the product rule set over them (`S101` on benchmark class names, `S2223` on the
TestsKernel key holders). Deriving it from the folder means a new support project is classified by where it
lives, with nothing to remember.

The property is read only by the scanner's own targets, which are injected during a Sonar run and absent
otherwise, so a normal `dotnet build` never sees it. Sonar still applies **test-scope** rules to these files —
the `[AssertionMethod]` markers that answer `S2699` stay load-bearing.

## Package versions — `Directory.Packages.props`

The repo uses **Central Package Management**. Every NuGet version lives in this one file as a `PackageVersion`;
a csproj writes `<PackageReference Include="Serilog" />` with **no** `Version`. NuGet fails the restore with
**NU1008** if a project names a version anyway, so the file cannot be bypassed by accident.

That guard is the point. Several packages are referenced by 9 projects, and before this the version was written
out 9 times - a bump that missed one file is exactly how the xunit/CodeCoverage platform mismatch got in.

Two entries are referenced by nobody and exist only to constrain the graph: `Microsoft.OpenApi` and
`SQLitePCLRaw.lib.e_sqlite3`. Both carry a floor (a transitive dependency would otherwise resolve to a version
with a known advisory) and a ceiling (the next major breaks). The reasoning sits next to each version, and the
csproj that names the package points here rather than repeating it.

Adding a package is two edits: the `PackageVersion` here, the `PackageReference` in the project.

## Test runner — `global.json`

Root `global.json` holds one key and no SDK pin:

```json
{ "test": { "runner": "Microsoft.Testing.Platform" } }
```

That is the .NET 10 opt-in to the Microsoft.Testing.Platform (MTP) `dotnet test`. It is not optional here: the
test projects run on **MTP v2**, and the .NET 10 SDK dropped the VSTest bridge those used to fall back to, so
without this file every C# leaf fails with "Testing with VSTest target is no longer supported". The opt-in is
repo-wide - once set, every test project must be an MTP one.

Two consequences for anything that shells out to `dotnet test`:

- The project comes from `--project`, never a bare path. A bare directory is now an error.
- Runner options go straight on the command line, **not** after a `--`. See `_dotnet_test` in
  `tooling/tests.just`.

The xunit reference is `xunit.v3.mtp-v2`, not plain `xunit.v3`. Same xunit version, different platform adapter:
`xunit.v3` pins `xunit.v3.mtp-v1`, which is MTP 1.x. `Microsoft.Testing.Extensions.CodeCoverage` moved to MTP 2.x
in 18.1.0, so the two cannot both be current - mixing them loads MTP 2.x under a v1 adapter and throws
`TypeLoadException` on `IDataConsumer` before a single test runs.

There is no `xunit.runner.visualstudio` and no `TestingPlatformDotnetTestSupport` property. Both belong to the
VSTest path this repo no longer has: the adapter bridged xunit to VSTest, and the property was the MTP **v1**
opt-in that `global.json` replaced. The xunit project template still emits both, so a new test project needs them
stripped. The one property that stays is `UseMicrosoftTestingPlatformRunner`, which makes the project a
standalone runner executable.

## JS workspaces & asset copy

Root `package.json` (name `binacle-net`, `private`) declares five workspace entries: `packages/*`,
`vipaq/packages/binacle-vipaq`, `api/src/Binacle.Net.UIModule`, `sites/docs` and `sites/demo`.

**Every javascript build in the repo is a member, and `package-lock.json` at the root is the only lock file.**
One `npm ci` installs all of them. That is what keeps a single copy of `three` in the tree: `binacle-net-ui`
imports it and so does each host, and two copies in one bundle make a mesh from one fail `instanceof` against
the other.

Root dev dependencies are `gulp` and `concurrently` (the latter is what lets one `just serve` run jekyll and
webpack together under a single Ctrl-C), and its only scripts are the asset-copy tasks:

- `npm run copy-assets-to-docs` → `gulp copy-assets-to-docs`
- `npm run copy-assets-to-demo` → `gulp copy-assets-to-demo`
- `npm run copy-assets-to-uimodule` → `gulp copy-assets-to-uimodule`

`just assets` runs all three, and `just install` runs it after the npm and bundler installs.

`gulpfile.js` copies shared `assets/` (images, js, css, fonts) into the two Jekyll sites and the UI module's
`wwwroot/`. One `IGNORE` block holds what each target skips, with the weight it saves beside each line. Each
of the three runs its own webpack build — see docs site (`$sites/docs`), demo site (`$sites/demo`) and the UI
module (`$api/modules/ui`).

**To re-measure that `IGNORE` block**, grep each target for `lib/<name>` outside its own `lib/` folder; what
nothing references is what can be skipped. The copy is one-way and never deletes, so a target that stopped
loading a library still has the files until someone removes them by hand.

## Docker build chain

The Dockerfile is **single-stage** — the publish happens outside it, in the `build` just module
(`tooling/build.just`):

1. `just build publish` runs `dotnet publish -c Release -o artifacts/binacle-net --no-self-contained --runtime
   linux-x64` of `api/src/Binacle.Net/Binacle.Net.csproj`. **Framework-dependent** — the runtime comes from the
   base image, so the app layer is ~18 MB rather than ~123 MB.
2. `Dockerfile` (`mcr.microsoft.com/dotnet/aspnet:10.0`) does `COPY ["artifacts/binacle-net", "."]`, sets
   `ARG VERSION → ENV BINACLE_VERSION`, `USER $APP_UID`, `ENTRYPOINT ["dotnet", "Binacle.Net.dll"]`.
3. `just build image [version]` does step 1 then `docker build --build-arg VERSION=<version>
   -t binacle-net:<version> .` (default `local`), plus the three per-build OCI labels (version, revision,
   created). It stops there — run it with `just image up full`. CI builds the same image the same way; see
   CI/CD (`$ci-cd`).

`artifacts/binacle-net` is not configurable: the Dockerfile hardcodes it in its `COPY` and `.dockerignore`
allowlists that one path, so the publish has to land exactly there.

There is no `EXPOSE`/`ASPNETCORE_HTTP_PORTS` in the Dockerfile — the aspnet:10.0 base defaults to port 8080;
compose/k8s map it. `artifacts/` is **output only** (generated `binacle-net/`, `docs/`, `demo/`, `openapi/`, plus
`tests/` and `coverage/` from a test run) — never edit it. Each folder is named after what produced it, so a
look at `artifacts/` says which artifact is which.

## Content projects (`Microsoft.Build.NoTargets`)

Several `.proj` files don't compile anything — they use the `Microsoft.Build.NoTargets` SDK to pull non-code files
into the solution (and travel with build output). There are six: `assets/assets.proj`, `tooling/tooling.proj`,
`sites/docs/docs.proj`, `sites/demo/demo.proj`, `api/requests/requests.proj` and
`samples/kubernetes/minimal/minimal.proj`.
The Docker samples use `Microsoft.Docker.Sdk` `.dcproj` files instead. None of these affect the C# build.

**`results/` is deliberately not in the solution.** The curated benchmark vault is read and hand-edited, never
built, so it carries no `.proj` and has no solution folder — open the markdown directly.

## `tooling/` vs `samples/`

`tooling/` holds **every task the repo can run**, CI included — the `tests.just`, `coverage.just`, `openapi.just`,
`agents.just`, `serve.just` and `build.just` modules for `just`, the scripts that have not moved yet (the
per-slice `performance.*`, `benchmarks.*`, `tmux.sh`), local compose files, and emulator state. `samples/` are
**user-facing deployment starting points** to copy and run the published image. See `$commands` for
the scripts and samples (`$samples`) for the deployment examples.
