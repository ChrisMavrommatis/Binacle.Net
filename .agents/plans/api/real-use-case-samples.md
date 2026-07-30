# Align the deployment samples with the real profiles

**Status:** Not started. Delegate to its own session. Surfaced 2026-07-30 while designing the image smoke tests.

## Where the samples are

User-facing deployment samples live in `/samples/` - `samples/docker/*` (compose) and `samples/kubernetes/*`.
The published docs snapshot them per version under `docs/collections/_versions/*/samples/`; **that copy is off
limits** - the docs site is written in its own session. Edit only `/samples/`, and write down anything the docs
pages must say so the docs session can pick it up.

## Current state

The docker samples already cover most shapes:

| Sample | Flags | Maps to |
|---|---|---|
| `docker/minimal-setup` | none (as shipped) | `zero` |
| `docker/ui-setup` | swagger + scalar + ui | `quickstart` |
| `docker/service-npgsql` | + service + health + packinglogs, Postgres | `full` (postgres) |
| `docker/service-azure` | same + Azure + telemetry | `full` (azure) |

Two problems:

- **No `prod` sample.** Every service sample turns `UI_MODULE` on. Nothing shows the production shape - service
  on, docs on, but `UI_MODULE` and `DEBUG_ENDPOINT` off. A first-time operator has no secure starting point:
  copying a service sample ships the web UI and sits one flag away from exposing `/_debug`, which echoes the
  caller's headers and connection address.
- **Samples pin `binacle/binacle-net:2.1.1`** - a frozen old patch. The release workflow now publishes a moving
  `{{major}}.{{minor}}` tag (e.g. `3.0`) so a copied sample inherits bug fixes and never a breaking change.
  Samples should pin that.

## The work

- [ ] **Add `samples/docker/service-prod`** (name to taste): swagger + scalar + service + health + packinglogs
      on; `UI_MODULE` and `DEBUG_ENDPOINT` off; `JwtAuth.json` injected via compose `configs:`; one backend
      (SQLite is simplest for a copyable sample). It is `service-npgsql` minus `UI_MODULE`, on a lighter
      backend. Comment that the UI and `/_debug` are deliberately off for production.
- [ ] **Repin every sample** to the moving `{{major}}.{{minor}}` tag instead of `2.1.1`.
- [ ] **Match flag-sets to the profiles exactly**, so a copied sample is a shape the smoke suite tests:
      `minimal-setup`=zero, `ui-setup`=quickstart, `service-*`=full, the new sample=prod.
- [ ] **List the prod sample in `samples/README.md` and `samples/docker/README.md`.**
- [ ] Consider a Kubernetes prod sample - today only `kubernetes/minimal-setup` exists.
- [ ] **Write down for the docs session** what the versioned docs snapshot needs: the new prod sample and the
      retag, for the release's sample pages.

## The decision for that session

Whether the smoke stacks (`config/docker-compose.smoke-*.yml`) **reuse** these samples or stay parallel. Reuse
means the tests exercise exactly what users copy; parallel keeps the tests isolated from sample churn. The smoke
plan builds parallel stacks by default; revisit once the samples line up with the profiles.

## Constraints

- Feature flags are exact-match `True`/`False`, case-sensitive. A lowercase flag is silently ignored and the
  surface just fails to appear - so every sample flag must be exact `True`.
- ServiceModule needs `JwtAuth.json` or it fails validation at startup; the service samples inject it via
  compose `configs:`. The prod sample must do the same and tell the user to replace the secret.
- The prod sample keeps the docs (`SWAGGER_UI`/`SCALAR_UI`) on but leaves `UI_MODULE` and `DEBUG_ENDPOINT`
  off - `/_debug` echoes caller headers and connection address and must not be exposed in production.

## Why separate

This edits the files users copy and feeds the published docs, so it wants its own focused session, not a change
made in passing while building the tests.
