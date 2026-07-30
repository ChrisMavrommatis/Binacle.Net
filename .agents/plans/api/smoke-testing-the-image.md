# Smoke test the built docker image over HTTP

**Status:** Designed, nothing built. The label prerequisite (below) is done - Dockerfile, `build.just` and the
release workflow now stamp OCI labels, verified on a local build. The suite itself - tools, config files,
stacks, recipes - is not built. Local by hand first, **no CI** until it has proved itself.

## The problem

Everything we test today runs **in process**. `Binacle.Net.IntegrationTests` boots the app with
`WebApplicationFactory<IApiMarker>` and replaces the presets with three test-only ones, so it never loads the
config files we ship. Nothing anywhere touches the image.

That leaves a class of failure invisible until someone pulls the tag: a config file that did not get copied or
landed at the wrong path; a module env var that no longer switches anything on; a connection string that works
on the host and not inside the container; the `VERSION` build arg never reaching the process; a wrong entry
point, port or runtime. All packaging and wiring, none of it C# logic, which is why the current suites cannot
see it. The mainstream .NET answer does not cover this: `WebApplicationFactory`, Testcontainers and Aspire all
host the API in-process and containerize only its dependencies. The common shape for testing the artifact is
build, run the image, poll an endpoint, assert over HTTP.

## The two lines - do not move these

> **Assert what the image contains and wires. Never assert what the algorithm computed.**

Re-running the integration assertions over HTTP buys the same coverage with worse diagnostics, ten times the
runtime, and a suite that goes red on every legitimate packing change. The integration tests own "is the answer
right", in-process, where a failure points at a line of C#. The one known-good value that **is** right here:
data that comes from the image's config files rather than the algorithm - the shipped presets are image
content, true when an algorithm changes, broken when a file does not get copied.

> **Every check must be able to fail, and must not be able to pass for a reason unrelated to what it claims.**

A profile whose assertions are all 404s satisfies the first line and is still worthless: a wrong image, a
container that read no config, and a typo'd flag all pass it. This is why the profiles that carry 404s pair them
with positive 200s that prove the right image read its config - `quickstart`'s docs and UI pages, `prod`'s
`Features` reading.

## Four profiles

Two profiles were the original design, on the argument that off-states do not interact. That argument is broken
in two verified places:

- `Program.cs:175` mounts `/openapi/{documentName}.json` when `swaggerEnabled || scalarEnabled`. Two profiles
  can never tell which of the two flags did it.
- `UIModule/ModuleDefinition.cs:85` calls `UseStatusCodePagesWithReExecute`, and the exclusion middleware at
  `:89-91` covers only `/api`, `/swagger`, `/scalar`. So the off-state of `/_health` and `/_debug` is shaped by
  whether `UI_MODULE` is on: with it on they 404 with an HTML error page, without it they 404 empty. Status
  survives, body and content type do not.

The four are real configurations, from nothing shipped to everything on. The key one is `prod`: it is `full`
minus the two things you never expose in production - the **web UI** (`UI_MODULE`) and the **debug endpoint**
(`DEBUG_ENDPOINT`). The API docs (`SWAGGER_UI`, `SCALAR_UI`) stay on in prod because they are documentation, not
a debug surface.

- **`zero`** - as shipped, no env. One claim: the image starts with no configuration and a core route answers.
  That single 200 proves the entry point ran, the runtime works, the port is right and the non-optional config
  files were found. Cheapest, highest value. Do not grow it into a list of 404s - that is what made the old
  `bare` profile unfalsifiable.
- **`quickstart`** - `SWAGGER_UI` + `SCALAR_UI` + `UI_MODULE` on, nothing else (the README `docker run`). The
  try-it-out shape: docs and the web UI, no auth, no backend. Proves the published getting-started command
  works - which exact-match feature flags make fragile - and that with service off the auth surface is absent.
  A single container.
- **`prod`** - `SWAGGER_UI` + `SCALAR_UI` + `SERVICE_MODULE` + `HealthChecks` + `PackingLogs` on; `UI_MODULE`
  and `DEBUG_ENDPOINT` **off**. The real deployment, which no example covers. Its defining assertion: **`/_debug`
  is 404** - the endpoint that echoes the caller's headers and connection address is not exposed in production -
  while docs, auth and openapi all work. Self-anchors on `/_health`: `Features` lists swagger, scalar and
  service but not ui or debug. Needs `JwtAuth.json` and a backend.
- **`full`** - everything on: all feature flags plus `DEBUG_ENDPOINT`, health and packing logs. The dev/demo
  shape the shipped example stacks turn on, where `/_debug` is deliberately 200 and the UI serves. Needs
  `JwtAuth.json` and a backend.

`zero` and `quickstart` are single containers; `prod` and `full` each need a storage backend. The off columns
are the security test: `/_debug` echoes the caller's headers and connection address, `/_health` exposes
internals, and the admin route in one request separates three states - 404 never mounted, 200 open to the
world, only 401 mounted **and** protected.

## Tooling: two binaries, and how to run them

Not a C# project. Nothing here needs a test framework: the assertions are "this file is in the image with these
permissions" and "this URL answers this status with this JSON". A C# project buys a `ProjectReference` risk, a
build step and xUnit1051 warnings for nothing.

- **`container-structure-test`** (Google, YAML) for image content - file existence, mode, owner, `docker
  inspect` metadata, command output. Actively maintained (v1.22 line, 2026).
- **`hurl`** (plain-text HTTP files) for the HTTP surface - the request, the expected status and JSONPath
  assertions, in the order you read them. Actively maintained (Orange, current release line). Pin the exact
  version at install time.

**Install the pinned binaries; do not run the tools as containers.** Both ship as a single static binary with a
pinned version, a non-zero exit on failure and JUnit output - CI-ready as they stand. The only container in
play is binacle: the subject, not the harness. Containerizing hurl would mean attaching it to the compose
network to reach the API; containerizing `container-structure-test` would mean mounting the docker socket. Both
add wiring for no gain when the binary drops into `~/.local/bin` (no sudo, which matters on this machine) and
onto a CI runner the same way. `just install` pins and installs both, so local and CI never drift.

**Outside-in only. Nothing gets copied into the image.** That rules out `goss`/`dgoss`, whose model is a binary
running inside the container - it would mean baking a test binary into a production image or mounting one in,
and either way the thing under test is no longer the thing we ship.

How each runs (syntax verified against current docs):

- **`container-structure-test` reads the image, not a running container.** Config is a YAML with
  `schemaVersion: 2.0.0`. The three test types we use are `fileExistenceTests`
  (`path`, `shouldExist: true|false`, `permissions`, `uid`, `gid`), `fileContentTests` (`expectedContents`,
  `excludedContents` - both lists of regexes), and `metadataTest` (`labels` with optional regex, `envVars`,
  `entrypoint`, `user`, `workdir`). Invoke:
  `container-structure-test test --image binacle-net:local --config config/smoke/image.yaml --output junit
  --test-report report.xml`. **Use `--driver docker`.** The `--driver tar` path (`docker save ... | test
  --driver tar --image img.tar`) is daemon-free and passes almost everything, but - measured 2026-07-30 - it
  **cannot read directory ownership**, so the `/app/data` uid-1654 check fails under tar and needs the docker
  driver. Since CI loads the image into the daemon anyway (for the hurl profiles), the docker driver is free
  there; tar is only the fallback when no daemon exists, and it drops that one check. `commandTests` (none
  here) also need docker. Non-zero exit on any failure; JUnit for CI. Verified: 31/31 on the docker driver
  against a fresh `binacle-net:local`, and the same file fails the licenses check on the old `2.1.1` image.
- **`hurl` needs binacle running.** Build, `docker compose up` the profile stack, then `hurl --test
  --retry <n> --retry-interval <ms> --report-junit report.xml file.hurl`, then `docker compose down`. Retry can
  also live per-file in an `[Options]` section, so readiness is expressed in the file that needs it rather than
  the recipe. **hurl does not follow redirects by default** (`-L`/`--location` opts in), so a stray HTTPS
  redirect surfaces as a 307 for free and `/scalar` shows its real 302 - just request `/scalar/` for the 200.

## Inventory: what to assert

### Static - `container-structure-test`

The static side is a complete declaration on purpose: it asserts every shipped file even where a dynamic test
would also catch a regression. It says "this is how the image is supposed to be shipped." All of it maps to
`fileExistenceTests`, `fileContentTests` and `metadataTest`. Built: `config/smoke/image.yaml`, 31 assertions,
green on `--driver docker`.

**1. Files present** - the nine shipped config files, plus the app and data dir. Most already throw at startup
if missing (`Optional=false`), so a dynamic test also catches them; the silent ones (no startup signal) are
`ForwardedHeaders.json` and `UiModule/ConnectionStrings.json`.

```
Config_Files/appsettings.json                       Config_Files/DiagnosticsModule/HealthChecks.json
Config_Files/ForwardedHeaders.json                  Config_Files/DiagnosticsModule/OpenTelemetry.json
Config_Files/Presets.json                           Config_Files/DiagnosticsModule/PackingLogs.json
Config_Files/ServiceModule/RateLimiter.json         Config_Files/DiagnosticsModule/Serilog.json
Config_Files/UiModule/ConnectionStrings.json        Binacle.Net.dll        /app/data (dir)
```

**2. Files absent** - nothing at runtime sees these leak in; static only.

| Must not exist | Why |
|---|---|
| any `**/*.Development.json` (7 in source) | dev overrides silently win in prod; the `.dockerignore` last line is the only guard |
| `Config_Files/Cors.json` | only `Cors.Development.json` exists in source |
| `Config_Files/ServiceModule/JwtAuth.json` | only `.Development` exists; ServiceModule gets this injected at deploy |
| `Config_Files/ServiceModule/ConnectionStrings.json` | only `.Development` exists in source |

**3. Permissions** - `/app/data` owned by uid **1654** (`$APP_UID`); `/app` not writable by the app user. The
container runs as non-root `app`; the Dockerfile chowns `/app/data` so a mounted volume is writable. Catches a
dropped `chown` that silently turns file logging into a no-op.

**4. Image metadata** (`docker inspect`, no filesystem):

| Field | Expected |
|---|---|
| Entrypoint | `["dotnet", "Binacle.Net.dll"]` (base-image muxer, not the self-contained apphost) |
| User / WorkDir | 1654 / `/app` |
| Env `BINACLE_VERSION` | set, non-empty |
| Exposed port | none declared - do not assert (served via `ASPNETCORE_HTTP_PORTS`, not `EXPOSE`) |
| OCI labels | see Image labels below |

**5. Content defaults** - the value is the toggles with no HTTP route: flipped to `true`, nothing at runtime
tells you.

| File | Assert | Note |
|---|---|---|
| `ForwardedHeaders.json` | `Enabled: false` | no route - static is the only assertion |
| `PackingLogs.json` | `Enabled: false` | no route |
| `OpenTelemetry.json` | `Otlp.Enabled: false`, `AzureMonitor.Enabled: false` | no route |
| `HealthChecks.json` | `Enabled: false` | also caught by `zero` (`/_health` 404) |
| `HealthChecks.json` | `RestrictedIPs: []` | "restricts nobody"; once on, readable from any host |
| `appsettings.json` | `AllowedHosts: "*"` | the established default |
| `UiModule/ConnectionStrings.json` | `BinacleApi: ""` | ships empty, falls back to request host |
| `Serilog.json` / `PackingLogs.json` | paths under `data/logs/`, `data/pack-logs/` | pair with the `/app/data` ownership check |
| `RateLimiter.json` | `ApiUsageAnonymous`, `AuthToken`, `ApiUsageDemoSubscription` present and non-empty | the key is packaging; the number is behaviour, left to integration |

### Dynamic - `hurl`, by profile

Lean, not complete: every check must be able to fail. Redirects off. Retry-until-200 for readiness. Read a
preset through to its bins by name and count, never dimensions.

**`zero`** - no env:

| Request | Assert |
|---|---|
| `GET /api/v3/presets` | 200 |
| `GET /api/v4/presets` | 200, contains `rectangular-cuboids` with 3 bins (Small/Medium/Large), not their sizes (an empty-dictionary 200 cannot fail, so read through) |

**`quickstart`** - `SWAGGER_UI` + `SCALAR_UI` + `UI_MODULE` on, nothing else (single container):

| Request | Assert |
|---|---|
| `GET /api/v3/presets` | 200 |
| `GET /swagger/`, `/scalar/` | 200 (scalar 302 to `/scalar/` first) |
| `GET /openapi/v3.json`, `/openapi/v4.json` | 200 (swagger/scalar on) |
| `GET /` (UI) | 200 HTML |
| `GET /_health` | 404 - health off, so no `Features` to read here |
| `GET /_debug`, `/openapi/service.json`, `POST /api/auth/token` | 404 - debug off, service off |

**`prod`** - `SWAGGER_UI`+`SCALAR_UI`+`SERVICE_MODULE`+`HealthChecks`+`PackingLogs` on; `UI_MODULE`+`DEBUG_ENDPOINT` off; `JwtAuth.json` injected, one backend:

| Request | Assert |
|---|---|
| `GET /api/v3/presets` | 200 |
| `GET /swagger/`, `/scalar/` | 200 |
| `GET /openapi/v3.json`, `/openapi/v4.json`, `/openapi/service.json` | 200 (swagger on, and service on for `service.json`) |
| `GET /_health` | 200, `Features` lists swagger, scalar, service - **not** ui or debug; `.Version != "Unknown"` |
| `GET /_debug` | **404** - the production security assertion: the endpoint echoing caller internals is not exposed |
| `GET /` (UI) | 404 - `UI_MODULE` off |
| `POST /api/auth/token` | 200, has `accessToken` - the storage-path proof in the real shape |
| `GET /api/admin/account/{real-guid}` unauthenticated | 401 (not 404, not 422) |

**`full`** - all six flags on plus `DEBUG_ENDPOINT`, `JwtAuth.json` injected, one backend:

| Request | Assert |
|---|---|
| `GET /_health` | 200, `Features` lists every module |
| `GET /_debug` | 200, echoes request |
| `GET /swagger/`, `/scalar/` | 200 (scalar 302 to `/scalar/` first) |
| `GET /openapi/v3.json`, `/openapi/v4.json`, `/openapi/service.json` | 200 |
| `GET /` (UI) | 200 HTML |
| `POST /api/auth/token` | 200, has `accessToken` (seeded admin `admin@binacle.net` / `B1n4cl3Adm!n`) |
| `GET /api/admin/account/{real-guid}` unauthenticated | 401 (not 404, not 422) |

Primary oracle: `SystemHealthCheck.cs:41` puts `EnabledFeatures.Order().ToArray()` straight into the response,
so `$.entries.System.data.Features` is authoritative. **Cross-check it against route presence.** If health says
a module is on but its route is 404, `AddX` ran and `UseX` did not - a failure no in-process test can see.
Assert `Version != "Unknown"` (`Metadata.cs:18-21` falls back to `"Unknown"`, which is what a build arg that
never reached the process looks like), not the exact version - works in every profile with no env plumbing.

The three OpenAPI documents are `v3` and `v4` (core, always registered) and `service` (ServiceModule on), all
behind the one `swagger || scalar` gate.

### Gotchas when building the stacks

Not assertions - things that make the setup itself correct. Miss one and you get a green that means nothing or a
red that reads as a flake.

- **Feature flags are exact-match `True`/`False`, case-sensitive** (`EnvironmentVariableFeatureProvider`, via
  `bool.TrueString == variable`). A lowercase `SWAGGER_UI=true` is silently ignored and 404s - the same 404 a
  broken image gives. Config keys (`HealthChecks__Enabled`) are case-insensitive; feature flags are not.
  `quickstart` sets three feature flags, so all three must be exact `True`.
- **`full` sets `DEBUG_ENDPOINT` and `UI_MODULE`; `prod` leaves both off** - that debug-off, UI-off state is the
  point of `prod`, while the docs (`SWAGGER_UI`/`SCALAR_UI`) stay on in both. `DEBUG_ENDPOINT` is in no compose
  file today, only `launchSettings.json`.
- **`full` and `prod` must inject `JwtAuth.json`** - absent on the image; with `SERVICE_MODULE=True` a missing
  one throws `OptionsValidationException` (Issuer/Audience/TokenSecret/ExpirationInSeconds) and the container
  exits non-zero.
- **`full` and `prod` each pick one backend and raise the anonymous rate limit** - `ApiUsageAnonymous` is
  `SlidingWindow::60/3600-30`, 60 anonymous requests an hour in a shared bucket that decays; two runs ten
  minutes apart and the second goes red with 429s. Raise it in the smoke compose, and say why - values are
  behaviour, presence is packaging.
- **Container serves plain HTTP on 8080** - no TLS. Redirects off so a bad HTTPS redirect shows as 307, not a
  connection error.
- **Use a real GUID for the admin 401 check** - `Guid.Empty` is rejected as invalid (422) before lookup.
- The token request doubles as a storage-path check: startup tasks run to completion before the port opens
  (`Program.cs:225-226`), so once anything answers the admin is seeded and its tables exist. Getting a token
  proves the task wrote the account and the endpoint read it back, in one request.

## Image labels

The pushed image is labelled by `metadata-action`; a local `just build image` was unlabelled. Both are fixed
now (see Status). What each label holds, per the OCI image-spec, and the value for this repo:

| Label | Value | Applied by |
|---|---|---|
| `title` | `Binacle.Net` | Dockerfile (CI: metadata-action) |
| `description` | `Binacle.Net is an API created to address the 3D Bin Packing Problem in real time.` | Dockerfile (CI: metadata-action, from GitHub About) |
| `source` | `https://github.com/ChrisMavrommatis/Binacle.Net` | Dockerfile (CI: metadata-action) |
| `url` | `https://www.binacle.net` | Dockerfile; CI: pinned in metadata-action |
| `documentation` | `https://docs.binacle.net` | Dockerfile only |
| `vendor` | `Chris Mavrommatis` | Dockerfile only |
| `licenses` | `GPL-3.0-only AND CC-BY-SA-4.0` (dual: code GPL, content CC-BY-SA) | Dockerfile; CI: pinned in metadata-action |
| `base.name` | `mcr.microsoft.com/dotnet/aspnet:10.0` | Dockerfile only |
| `version` | the stripped tag | `--label` in build.just; CI: metadata-action |
| `revision` | git SHA | `--label` in build.just; CI: metadata-action |
| `created` | RFC 3339 timestamp | `--label` in build.just; CI: metadata-action |

Mechanic: labels applied at build time (`metadata-action` via `build-push-action`) override same-key Dockerfile
`LABEL`s, so in CI the Dockerfile's `title`/`description`/`source`/`url`/`licenses` are replaced by
metadata-action's, while `documentation`/`vendor`/`base.name` survive. `licenses` and `url` are pinned in the
workflow because metadata-action's auto values are wrong (`NOASSERTION` from dual-license detection) or
undesired (the repo URL). `created`/`revision` stay per-build via `--label`, never Dockerfile `LABEL`s, so they
add no layer and never bust the cache. `ref.name=ubuntu` is inherited from the base image and left as-is.

**Smoke asserts:** `title == "Binacle.Net"`, `source ==` the repo URL, `version` and `revision` non-empty, and
**`licenses != "NOASSERTION"`** - the check that catches the exact regression the live 2.1.1 image ships today.

## The boundary with integration

A second plan covers integration tests for what the in-process harnesses miss - the modules are off there, so
rate limiting and CORS are exercised nowhere. One line keeps the two from duplicating:

> **Behaviour goes to the integration suite, in process. Packaging stays here, in a container.**

"Does rate limiting return 429 when on" is behaviour - fast in process, failure points at a line of C#. "Is
`RateLimiter.json` in the image" is packaging. "Does CORS echo a configured origin" is behaviour. "The image
has no `Cors.json`" is packaging.

## What it must not do

- **No project reference to the app, and no C# project at all.**
- **Nothing copied into the image.** No test binary, no `HEALTHCHECK` added for the tests' benefit.
- **No numeric assertions from a packing run** - never a coordinate or a bin count.
- **No validation or error-path cases** - wrong-input handling is logic.
- **Keep it fast.** If it takes longer than the image takes to build, it is doing too much.

## Build checklist

**Tools**

- [ ] Add pinned `container-structure-test` and `hurl` downloads to the root `just install` (today: `npm
      install`, `docs`/`web` `bundle install`, `assets`). Both are single static binaries, installed to
      `~/.local/bin` (already on `PATH`, no sudo). Pin hard versions, not `latest`, so local and CI install the
      same bytes. Verified working on 2026-07-30 (arch `x86_64`):

      - `container-structure-test` **v1.22.1** - one binary:
        `https://github.com/GoogleContainerTools/container-structure-test/releases/download/v1.22.1/container-structure-test-linux-amd64`
        -> `chmod +x` into `~/.local/bin/container-structure-test`.
      - `hurl` **8.0.1** - tarball, binary under `bin/`:
        `https://github.com/Orange-OpenSource/hurl/releases/download/8.0.1/hurl-8.0.1-x86_64-unknown-linux-gnu.tar.gz`
        -> extract, `install -m0755 .../bin/hurl ~/.local/bin/hurl`.

      Verify with `container-structure-test version` and `hurl --version`. Per the note already on `install`,
      if this makes a third separately-installable thing, promote it to its own module. Bump the pins when a
      new session builds this - check the latest releases first.

**Image content**

- [x] `config/smoke/image.yaml` - one `container-structure-test` file covering the Static inventory above:
      the nine config files; no `*.Development.json`; no `Cors.json`; no `JwtAuth.json`; `/app/data` owned by
      1654; `BINACLE_VERSION` set; the content defaults; the label assertions. 31 assertions, green on
      `--driver docker` against `binacle-net:local` (2026-07-30). Run manually until the recipe lands:
      `container-structure-test test --image binacle-net:local --config config/smoke/image.yaml`.
- [x] OCI labels stamped on every build - Dockerfile block, `build.just` `--label`s, workflow `licenses`/`url`
      pins. (The suite still has to assert them.)

**HTTP surface**

- [x] `config/smoke/zero.hurl`, `quickstart.hurl`, `prod.hurl`, `full.hurl` written. `full.hurl` is verified
      against a live container (curl, 2026-07-30); the other three are written from the same shapes and the
      feature-flag mechanism but **not yet run** - `hurl` is not installed, and a stuck container (below) blocked
      a clean bring-up of the others. Corrections found live, already folded in: `/swagger/` 301s to
      `/swagger/index.html` (assert the index, not `/swagger/`); the token body is `{tokenType, accessToken,
      expiresIn}` (no `refreshToken`); `/api/v4/presets` is `$.presets['<name>']` (3 bins each); the `Features`
      names are `SwaggerUI`/`ScalarUI`/`UIModule`/`ServiceModule`/`DebugEndpoint`/`RateLimiter`; `GET
      /api/auth/token` is 405 when service is on (POST-only) and 404 when off.

**Stacks** - their own files under `config/smoke/`, flat (matching the flat `config/` dev stacks), not the
sample files. Smoke runs `binacle-net:local` and needs test-only tweaks the samples must never carry; the
samples pin the published image. Storage is throwaway (a named volume, no `./data` bind), so smoke needs no
`_prepare` step.

- [x] `config/smoke/zero.yml`, `quickstart.yml`, `prod.yml`, `full.yml` written. `prod`/`full` inline
      `JwtAuth.json` via compose `configs:`, use SQLite, and raise `RateLimiter__ApiUsageAnonymous`. All set
      `image: binacle-net:local`. `prod` leaves `UI_MODULE`/`DEBUG_ENDPOINT` unset. All four brought up cleanly
      via `docker compose` (the container starts and serves); only the parallel probing tripped over the port.

**Recipes** - a dedicated `config/smoke.just` module (`mod smoke`), not the `image` module: smoke stacks are
ephemeral and skip `image`'s `_prepare` (bind-dir setup), and smoke is one end-to-end concern.

- [x] `config/smoke.just` written (`mod smoke` added to the root justfile). `just smoke up|down|test <profile>`
      iterate one stack; `just smoke all` builds `binacle-net:local` then loops up/test/down with a `trap` so a
      failing test still tears the stack down. `test` runs `container-structure-test` (docker driver) then the
      profile's `.hurl`. A `case` rejects an unknown profile. Not wired into `just test all`. **Not yet run end
      to end** - blocked on `hurl` and the stuck container.
- [ ] `just smoke all` shell wants a real run once `hurl` is in and the port is free - watch that the `trap`
      actually tears each stack down (the manual probing showed a leaked container is easy to cause).

**Docs**

- [ ] Repo-layout row in the agent docs README; a Smoke section in `config/README.md`; the leaf in the commands
      doc.

## CI - later, in the release workflow

Local by hand first; a gate nobody trusts gets disabled. The suite is CI-ready (pinned binaries, exit codes,
JUnit); the workflow is the blocker:

- `release-docker-image.yml:50-55` uses `build-push-action` with `push: true` and no `load:`, so the image
  never lands in the runner's daemon - **there is nothing to smoke.** Needs `load: true` first (cheap, one
  platform).
- `release-docker-image.yml:28` inlines its own `dotnet publish`, duplicating `build.just`. They match today by
  coincidence. Until the workflow builds via `just build publish`, the smoke path and the release path build
  the image two different ways, and a green local smoke says little about what shipped. **Worth more than the
  suite itself.**

The path when it goes in: same `just install` on the runner; build with `load: true` (ideally via the `build`
recipes); run `container-structure-test` against the loaded image (or a `docker save` tar with `--driver tar`);
bring up `zero` and `quickstart` (single containers), hurl them; push only if green. `prod` and `full` each
need a storage backend, so add them once the cheap two have earned trust. Place it in the **release** workflow,
not the PR gate.

## Samples do not cover production - a separate plan

The user-facing samples in `/samples/docker/` already map to most profiles: `minimal-setup` is `zero`,
`ui-setup` is `quickstart`, and `service-npgsql`/`service-azure` are `full` (everything on, with a backend).
The one shape with **no sample is `prod`** - service on with `UI_MODULE` and `DEBUG_ENDPOINT` off. The samples
also still pin `binacle/binacle-net:2.1.1` rather than the moving `{{major}}.{{minor}}` tag. Aligning them -
adding the prod sample, repinning, matching flag-sets to the profiles - is its own piece of work. **Do not
touch the sample files from this plan** - the smoke stacks are separate files under `config/`. A separate plan
covers it.
