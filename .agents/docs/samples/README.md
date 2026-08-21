---
id: samples
description: Deployment samples — Docker Compose (minimal, quickstart, prod, service, full) and Kubernetes (minimal); each folder name is a smoke profile name, feature flags, config wiring, and the keep-in-sync rule
verified: 2026-08-22
check: Sample folders, compose env vars, bind-mounted config paths, the k8s resource bounds, and the pinned image tag match samples/; the compose project name still comes from a top-level name: key and not a .env file; every samples/docker folder name has a tooling/smoke/<name>.yml with the same module set
also_update:
  - api/configuration
  - api/modules
paths:
  - "samples/**"
---

# Samples

Deployment examples under `samples/`. They run the published image at an **exact pinned version** and demonstrate
real module configurations. Index: `samples/README.md` (the in-tree one).

**`samples/` vs `tooling/`** — `samples/` are **starting points a user copies** to stand up their own deployment.
`tooling/` holds **every task the repo can run** (the `just` modules, run scripts, local compose, emulator state) — not a
deployment template. Don't conflate them: a change for local dev belongs in `tooling/`; a deployment-shape change
belongs in `samples/`.

> **Keep in sync with the code.** Samples encode actual feature flags, env vars, connection-string names, and
> config-file paths — they are not illustrative pseudo-config. When you add or change a feature flag, env var,
> default, or config path (e.g. adding `SCALAR_UI`, renaming a connection string, moving a `Config_Files` path),
> the affected samples must be updated to match, or they become wrong. The tables below show which knobs each
> sample sets, so you can find what to touch. Cross-check against `$api/configuration` and
> `$api/modules` (the feature-flag list).

## Docker samples (`samples/docker/`)

Common to all: host port `8080→8080`; a top-level compose `name:` key sets the project name; bind `./Presets.json` (read-only) →
`/app/Config_Files/Presets.json` and `./data` → `/app/data`.

| Sample | Demonstrates | Feature flags / key env | Extra services & config |
|---|---|---|---|
| `minimal` | Core fit/pack API, no modules | *(none — no `environment:` block)* | just `Presets.json` |
| `quickstart` | Docs + the web UI demo | `SWAGGER_UI`, `SCALAR_UI`, `UI_MODULE` | — |
| `prod` | **Self-hosted behind your own backend** — no docs, no UI, no auth, no database | `HealthChecks__Enabled`, `PackingLogs__Enabled` only | ships `ForwardedHeaders.json` and `OpenTelemetry.Production.json` **mounted-but-commented**, plus a commented `aspire-dashboard` service |
| `service` | **Binacle.Net offered to others** — accounts, JWT auth, rate limiting | `SWAGGER_UI`, `SCALAR_UI`, `SERVICE_MODULE`, `HealthChecks__Enabled`, `PackingLogs__Enabled`; `SQLITE_CONNECTION_STRING` active, `POSTGRES_`/`AZURESTORAGE_` commented; `BINACLE_ADMIN_CREDENTIALS` | binds `JwtAuth.json`; ships `Cors.json` mounted-but-commented |
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
| `minimal` | Core API on an existing cluster, internal only (`ClusterIP`) | `binacle-deployment.yaml` (1 replica, port 8080, presets from ConfigMap, data on PVC, resource requests/limits, `automountServiceAccountToken: false`), `binacle-net-service.yaml` (ClusterIP 8080), `binacle-presets-configmap.yaml` (`binacle-presets`), `binacle-pvc.yaml` (`binacle-data-pvc`, RWO 1Gi) |

**The deployment sets resource bounds and drops the service account token.** Requests are `100m` CPU / `128Mi`
memory / `256Mi` ephemeral-storage; limits are `512Mi` memory / `1Gi` ephemeral-storage. There is deliberately
**no CPU limit** — packing is CPU-bound, and a limit throttles a request mid-solve instead of letting it finish
and give the CPU back. `automountServiceAccountToken: false` because Binacle.Net never calls the Kubernetes API,
so the default token is only ever an extra credential in the pod. The numbers are starting values for a reader to
measure against, not a sizing recommendation; the manifest says so.

## Adding or modifying a sample

- **Naming is coupled**: folder name = sample name = `.dcproj`/`.proj` filename = the compose `name:` value
  (folder `service` → `service.dcproj` → `name: binacle-net-service`).
- **Register in the solution**: add the project to `Binacle.Net.slnx` under `/samples/docker/` (docker `.dcproj`,
  with `<Build />`) or `/samples/kubernetes/` (`.proj`, `Type="Shared"`). Generate a fresh `ProjectGuid`.
- **Baseline files**: `docker-compose.yml` (with its `name:` key), `Presets.json`, `README.md`, and a
  `.dcproj` (SDK `Microsoft.Docker.Sdk`). **No `.env`** — no sample has one. `JwtAuth.json` is required only with `SERVICE_MODULE=True`;
  `OpenTelemetry.Production.json` + `aspire-dashboard-config.json` only when shipping OTel/Aspire.
- Published samples bind config files read-only and use the pinned image tag (see below). The local build pipeline
  (`tooling/image.full.yml`, fed by `just build image`) instead uses `binacle-net:local` and injects config via
  compose `configs:` — see `$build-topology`.

## The image tag is pinned {#image-pin}

All six samples pin the same tag — `samples/docker/*/docker-compose.yml` and
`samples/kubernetes/minimal/binacle-deployment.yaml`. Never `latest`: a sample is copied once and lives for
years, so `latest` hands the reader the next major release on their next pull, with nothing in their config saying
what changed.

**Pin the minor line where one exists.** `release-docker-image.yml` publishes `{{major}}.{{minor}}` beside
`{{version}}`, so from v3.0.0 the pin is `binacle/binacle-net:3.0` and the sample inherits every later patch —
bug fixes flow, breaking changes never do, and the pin only changes when a new minor line opens. There is no
`{{major}}` tag on purpose: `3` crosses minor lines. An exact patch is the right pin only for a line that will get no
further ones, which is why v1.3.x and v2.x samples are pinned that way in the published docs snapshots.

**Until a minor tag exists, the pin sits on one prerelease and does not chase later ones.** All six currently
name the same beta, and they move once — straight to the minor tag when it opens. Read the value out of the
sample files rather than from here; a version named in a doc goes stale silently. The samples document v3-only
settings, so the old `2.1.1` would be wrong in a different way, and `3.0` does not resolve on Docker Hub until
v3.0.0 is published. **The rule that governs every move: a pin on `main` must name an image that already
exists**, so the pin follows a publish and never precedes one.

Three files outside the six carry the tag in prose and have to move with them: `README.md` at the repo root,
`samples/README.md` and `samples/docker/README.md`. Two more mention it as an example only —
`tooling/README.md` and `tooling/smoke.just`.

The published docs snapshots under `sites/docs/collections/_versions/v3.0.x/samples/` pin `3.0` directly,
because a
snapshot describes the released version rather than the working tree. They also carry a shorter comment above
the `image:` line: the repo copies explain our release order, which means nothing to a reader who downloaded
the file.
