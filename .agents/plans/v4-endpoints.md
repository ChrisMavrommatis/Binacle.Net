# v4 API — Endpoint Buildout

**Status:** In progress — 6 implemented, more planned
**Goal:** Track which v4 endpoints exist and which are still to build, while v4 is under construction
(not yet published in the docs site).

The reference contract lives in the agent docs:
- `/.agents/docs/api/v4/README.md` — endpoint tables (implemented + planned), algorithm selection, response shape
- `/.agents/docs/api/v4/add-endpoint.md` — step-by-step guide for adding one
- `/.agents/docs/api/v4/contracts.md` — request/response types and validators

Keep the planned list below in sync with the "Planned Endpoints" table in `api/v4/README.md`.

## Conventions (apply to every new endpoint)

- Goes in **v4 only** — never modify v3.
- Auto-registered — no `Program.cs` change. Implements `IGroupedEndpoint<ApiV4EndpointGroup>`.
- Bind with `BindingResult<T>` + `ValidateAsync` — never bind the body directly.
- Add `.RequireRateLimiting("ApiUsage")` on user compute endpoints (fit/pack). Read-only list endpoints
  (like `GET /api/v4/presets`) do **not** get it.
- Add `.RequireCors(CorsPolicy.CoreApi)` where CORS protection is needed.
- Do **not** add `.ProducesProblem(500)` per endpoint — `ApiV4EndpointGroup` sets it for all v4 endpoints.

## Implemented

- [x] `GET  /api/v4/presets` — list all preset names (not rate-limited)
- [x] `POST /api/v4/fit/bin` — fit-check a custom bin
- [x] `POST /api/v4/fit/bin/{preset}/{bin}` — fit-check one specific preset bin
- [x] `POST /api/v4/pack/bin` — pack a custom bin
- [x] `POST /api/v4/pack/bin/{preset}/{bin}` — pack one specific preset bin
- [x] `POST /api/v4/pack/smallest-bin` — custom bins → smallest successful pack

## Planned (not yet implemented)

Base request types exist (`CustomBinsRequestBase`, `PresetBinsRequestBase`); no concrete request or endpoint
classes yet. Start from step 1 of `add-endpoint.md`.

- [ ] `POST /api/v4/pack/compare` — pack all custom bins, return all results
- [ ] `POST /api/v4/pack/compare/{preset}` — pack all preset bins, return all results
- [ ] `POST /api/v4/fit/compare` — fit-check all custom bins, return all results
- [ ] `POST /api/v4/fit/compare/{preset}` — fit-check all preset bins, return all results
- [ ] `POST /api/v4/pack/smallest/{preset}` — preset bins → smallest successful pack
- [ ] `POST /api/v4/pack/best-fit` — custom bins → highest utilization
- [ ] `POST /api/v4/pack/best-fit/{preset}` — preset bins → highest utilization
- [ ] `POST /api/v4/pack/first-fit` — custom bins → first success
- [ ] `POST /api/v4/pack/first-fit/{preset}` — preset bins → first success
- [ ] `POST /api/v4/fit/smallest` — custom bins → smallest viable fit
- [ ] `POST /api/v4/fit/smallest/{preset}` — preset bins → smallest viable fit
- [ ] `GET  /api/v4/presets/{preset}` — get bin definitions for a preset

## When endpoints land

After each batch of new endpoints:
1. Tick the box above and move the row from "Planned" to "Implemented" in `api/v4/README.md`.
2. Update `api/v4/contracts.md` if new contract types were added; bump each edited doc's `verified:` date.
3. Regenerate the docs-site swagger spec for the version (see below) so the interactive Swagger UI is current.

## Docs-site Swagger spec

The per-version interactive Swagger UI loads `docs/collections/_versions/<version>/swagger/<doc>.json`.
Generate these by running the API and fetching `/openapi/{documentName}.json` (needs `SWAGGER_UI` or
`SCALAR_UI` enabled). **Run on the `Normal` profile (ServiceModule OFF)** so the spec matches the existing
committed convention — the committed `v3.json` has no `/api/auth/token` path. A ServiceModule-on run adds it.

- [ ] When v4 is ready to publish: add `docs/.../latest/swagger/v4.json` + a `swagger/v4.md` stub
      (`layout: versions/swagger`, `swagger: 'v4'`), plus the `api/v4.md` prose page and nav wiring.
