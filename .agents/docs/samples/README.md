---
id: samples
description: Deployment samples — Docker Compose (minimal, quickstart, prod, service, full) and Kubernetes (minimal); each folder name is a smoke profile name, feature flags, config wiring, and the keep-in-sync rule
verified: 2026-08-07
check: Sample folders, compose env vars, bind-mounted config paths, and the pinned image tag match samples/; every samples/docker folder name has a config/smoke/<name>.yml with the same module set
also_update:
  - api/configuration
  - api/modules
---

# Samples

Deployment examples under `samples/`. They run the published image at an **exact pinned version** and demonstrate
real module configurations. Index: `samples/README.md` (the in-tree one).

**`samples/` vs `config/`** — `samples/` are **starting points a user copies** to stand up their own deployment.
`config/` is the **maintainer's local-dev tooling** (run scripts, local compose, emulator state) — not a
deployment template. Don't conflate them: a change for local dev belongs in `config/`; a deployment-shape change
belongs in `samples/`.

> **Keep in sync with the code.** Samples encode actual feature flags, env vars, connection-string names, and
> config-file paths — they are not illustrative pseudo-config. When you add or change a feature flag, env var,
> default, or config path (e.g. adding `SCALAR_UI`, renaming a connection string, moving a `Config_Files` path),
> the affected samples must be updated to match, or they become wrong. The tables below show which knobs each
> sample sets, so you can find what to touch. Cross-check against `$api/configuration` and
> `$api/modules` (the feature-flag list).

## Docker samples (`samples/docker/`)

Common to all: host port `8080→8080`; `.env` sets only `COMPOSE_PROJECT_NAME`; bind `./Presets.json` (read-only) →
`/app/Config_Files/Presets.json` and `./data` → `/app/data`.

| Sample | Demonstrates | Feature flags / key env | Extra services & config |
|---|---|---|---|
| `minimal` | Core fit/pack API, no modules | *(none — no `environment:` block)* | just `Presets.json` |
| `quickstart` | Docs + the web UI demo | `SWAGGER_UI`, `SCALAR_UI`, `UI_MODULE`; `BINACLEAPI_CONNECTION_STRING` | — |
| `prod` | **Self-hosted behind your own backend** — no docs, no UI, no auth, no database | `HealthChecks__Enabled`, `PackingLogs__Enabled` only | ships `ForwardedHeaders.json` and `OpenTelemetry.Production.json` **mounted-but-commented**, plus a commented `aspire-dashboard` service |
| `service` | **Binacle.Net offered to others** — accounts, JWT auth, rate limiting | `SWAGGER_UI`, `SCALAR_UI`, `SERVICE_MODULE`, `HealthChecks__Enabled`, `PackingLogs__Enabled`; `SQLITE_CONNECTION_STRING` active, `POSTGRES_`/`AZURESTORAGE_` commented; `BINACLE_ADMIN_CREDENTIALS`, `BINACLEAPI_CONNECTION_STRING` | binds `JwtAuth.json`; ships `Cors.json` mounted-but-commented |
| `full` | Everything at once, demo box only | all of the above plus `UI_MODULE` and **`DEBUG_ENDPOINT`** | binds `JwtAuth.json` |

**`prod` and `service` are two different products, not two sizes.** ServiceModule is accounts, JWT auth, rate
limiting and a database — the shape for *hosting* Binacle.Net for other people. Most deployments call the API
from their own backend and need none of it, which is `prod`. Before this split every "serious" sample assumed
the hosted shape, so the commonest deployment had no starting point.

**`full` turns `DEBUG_ENDPOINT` on deliberately.** `/_debug` echoes the caller's whole request including their
`Authorization` header. It is a demo box; never present it as a deployment.

No sample bundles its own database any more. `service` points at infrastructure the reader already runs — the
old `service-npgsql` started a `postgres:17.6` container in the same file, which is a dev pattern, and its
connection string used `Host=localhost`, which inside a container means the container itself.

Fixed container config paths: `Presets.json` → `/app/Config_Files/Presets.json`; JWT →
`/app/Config_Files/ServiceModule/JwtAuth.json`; OTel →
`/app/Config_Files/DiagnosticsModule/OpenTelemetry.Production.json`; data → `/app/data`.

## Kubernetes samples (`samples/kubernetes/`)

| Sample | Demonstrates | Manifests |
|---|---|---|
| `minimal` | Core API on an existing cluster, internal only (`ClusterIP`) | `binacle-deployment.yaml` (1 replica, port 8080, presets from ConfigMap, data on PVC), `binacle-net-service.yaml` (ClusterIP 8080), `binacle-presets-configmap.yaml` (`binacle-presets`), `binacle-pvc.yaml` (`binacle-data-pvc`, RWO 1Gi) |

## Adding or modifying a sample

- **Naming is coupled**: folder name = sample name = `.dcproj`/`.proj` filename = the `COMPOSE_PROJECT_NAME`
  suffix (folder `service` → `service.dcproj` → `binacle-net-service`).
- **Register in the solution**: add the project to `Binacle.Net.slnx` under `/samples/docker/` (docker `.dcproj`,
  with `<Build />`) or `/samples/kubernetes/` (`.proj`, `Type="Shared"`). Generate a fresh `ProjectGuid`.
- **Baseline files**: `docker-compose.yml`, `.env` (project name only), `Presets.json`, `README.md`, and a
  `.dcproj` (SDK `Microsoft.Docker.Sdk`). `JwtAuth.json` is required only with `SERVICE_MODULE=True`;
  `OpenTelemetry.Production.json` + `aspire-dashboard-config.json` only when shipping OTel/Aspire.
- Published samples bind config files read-only and use the pinned image tag (see below). The local build pipeline
  (`config/docker-compose.build.yml`, fed by `just build image`) instead uses `binacle-net:local` and injects config via
  compose `configs:` — see `$build-topology`.

## The image tag is pinned {#image-pin}

All six samples pin `binacle/binacle-net:3.0.0-beta.1` for now — `samples/docker/*/docker-compose.yml` and
`samples/kubernetes/minimal/binacle-deployment.yaml`. Never `latest`: a sample is copied once and lives for
years, so `latest` hands the reader the next major release on their next pull, with nothing in their config saying
what changed.

**Pin the minor line where one exists.** `release-docker-image.yml` publishes `{{major}}.{{minor}}` beside
`{{version}}`, so from v3.0.0 the pin is `binacle/binacle-net:3.0` and the sample inherits every later patch —
bug fixes flow, breaking changes never do, and the pin only changes when a new minor line opens. There is no
`{{major}}` tag on purpose: `3` crosses minor lines. An exact patch is the right pin only for a line that will get no
further ones, which is why v1.3.x and v2.x samples are pinned that way in the published docs snapshots.

**`3.0` does not exist on Docker Hub until v3.0.0 is published.** The samples were rewritten to document v3-only
settings, so pinning the old `2.1.1` would have been wrong in a different way. They are pinned to
`3.0.0-beta.1` in the meantime, which does exist, and move to the `3.0` minor tag as the last change before the
tag - see `B5` in `release-v3.0.0.md`.
