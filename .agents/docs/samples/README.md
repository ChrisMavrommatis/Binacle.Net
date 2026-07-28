---
id: samples
description: Deployment samples — Docker Compose (minimal, ui, service-npgsql, service-azure) and Kubernetes (minimal); feature flags, config wiring, and the keep-in-sync rule
verified: 2026-07-15
check: Sample folders, compose env vars, and bind-mounted config paths match samples/
also_update:
  - api/configuration
  - api/modules
---

# Samples

Deployment examples under `samples/`. They run the published image `binacle/binacle-net:latest` and demonstrate
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
| `minimal-setup` | Core fit/pack API, no modules | *(none — no `environment:` block)* | just `Presets.json` |
| `ui-setup` | UI module | `SWAGGER_UI`, `SCALAR_UI`, `UI_MODULE`; `BINACLEAPI_CONNECTION_STRING` | — |
| `service-npgsql` | ServiceModule on PostgreSQL (+ UI) | `SWAGGER_UI`, `SCALAR_UI`, `UI_MODULE`, `SERVICE_MODULE`; `POSTGRES_CONNECTION_STRING`, `BINACLE_ADMIN_CREDENTIALS`, `PackingLogs__Enabled`, `HealthChecks__Enabled` | `postgres:17.6` service; binds `JwtAuth.json` → `/app/Config_Files/ServiceModule/JwtAuth.json` |
| `service-azure` | ServiceModule on Azure Storage + OTel/Aspire | same flags + `SERVICE_MODULE`; `AZURESTORAGE_CONNECTION_STRING`, `BINACLE_ADMIN_CREDENTIALS`, `OTEL_EXPORTER_OTLP_HEADERS` | `azurite` + `aspire-dashboard`; binds `JwtAuth.json` and `OpenTelemetry.Production.json` → DiagnosticsModule path |

Fixed container config paths: `Presets.json` → `/app/Config_Files/Presets.json`; JWT →
`/app/Config_Files/ServiceModule/JwtAuth.json`; OTel →
`/app/Config_Files/DiagnosticsModule/OpenTelemetry.Production.json`; data → `/app/data`.

## Kubernetes samples (`samples/kubernetes/`)

| Sample | Demonstrates | Manifests |
|---|---|---|
| `minimal-setup` | Core API on an existing cluster, internal only (`ClusterIP`) | `binacle-deployment.yaml` (1 replica, port 8080, presets from ConfigMap, data on PVC), `binacle-net-service.yaml` (ClusterIP 8080), `binacle-presets-configmap.yaml` (`binacle-presets`), `binacle-pvc.yaml` (`binacle-data-pvc`, RWO 1Gi) |

## Adding or modifying a sample

- **Naming is coupled**: folder name = sample name = `.dcproj`/`.proj` filename = the `COMPOSE_PROJECT_NAME`
  suffix (folder `service-npgsql` → `service-npgsql.dcproj` → `binacle-net-service-npgsql`).
- **Register in the solution**: add the project to `Binacle.Net.slnx` under `/samples/docker/` (docker `.dcproj`,
  with `<Build />`) or `/samples/kubernetes/` (`.proj`, `Type="Shared"`). Generate a fresh `ProjectGuid`.
- **Baseline files**: `docker-compose.yml`, `.env` (project name only), `Presets.json`, `README.md`, and a
  `.dcproj` (SDK `Microsoft.Docker.Sdk`). `JwtAuth.json` is required only with `SERVICE_MODULE=True`;
  `OpenTelemetry.Production.json` + `aspire-dashboard-config.json` only when shipping OTel/Aspire.
- Published samples bind config files read-only and use `binacle/binacle-net:latest`. The local build pipeline
  (`config/docker-compose.build.yml`, fed by `just build image`) instead uses `binacle-net:local` and injects config via
  compose `configs:` — see `$build-topology`.
