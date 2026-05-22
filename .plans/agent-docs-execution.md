# Agent Docs Execution Plan

Tracking doc: `.plans/agent-docs-audit.md` — full findings and decisions.

Legend: `done` | `pending` | `deferred`

---

## Phase 1 — Structure

| # | Task | Status |
|---|------|--------|
| 1 | Move `api/v3.md` → `api/v3/README.md` | done |
| 2 | Move `api/v4.md`, `api/contracts.md`, `api/add-endpoint.md` → `api/v4/` | done |
| 3 | Move `api/modules.md`, `api/module-*.md` → `api/modules/` | done |
| 4 | Merge `concepts/` → `concepts.md`, delete `concepts/` | done |
| 5 | Create stub `api/v3/contracts.md` | done |

## Phase 2 — Cross-link Repair

| # | Task | Status |
|---|------|--------|
| 6 | Fix all stale cross-links after moves (all docs + CLAUDE.md) | done |

## Phase 3 — Inaccuracy Fixes

| # | Doc | Status |
|---|-----|--------|
| 7 | `README.md` — repo layout (ServiceModule 3 projects, test rows, missing paths) | done |
| 8 | `commands.md` — api.sh alias `A`→`All`, add docker-compose | done |
| 9 | `api/README.md` — startup order (AddBinacleServices before AddDiagnosticsModule), Feature.Manager, SWAGGER_UI/SCALAR_UI | done |
| 10 | `api/endpoints.md` — IGroupedEndpoint non-generic base, per-module IModuleMarker | done |
| 11 | `api/v3/README.md` — multiple bins per request, FittedItems in fit, field names, enum values, dead pack early-exit codes | done |
| 12 | `api/v4/README.md` — remove "or leave it out" (null fails NotNull()) | done |
| 13 | `api/v4/contracts.md` — null algorithm, CustomBinsRequestBase/PresetBinsRequestBase users, Unknown=-1 | done |
| 14 | `api/v4/add-endpoint.md` — RequireRateLimiting comment, 404 case for preset endpoints | done |
| 15 | `api/service.md` — remove "or when no algorithm is set", clarify dep map locations | done |
| 16 | `api/kernel.md` — IApiMarker vs IModuleMarker, all three modules define IModuleMarker | done |
| 17 | `api/presets.md` — test preset section (code not file), v3 route pattern, Presets.json required | done |
| 18 | `api/configuration.md` — add ServiceModule config files, add BINACLE_ADMIN_CREDENTIALS, Scalar mount path | done |
| 19 | `api/modules/README.md` — add Scalar mount path (/scalar) | done |
| 20 | `api/modules/diagnostics.md` — all config files have env variants (not just Serilog) | done |
| 21 | `api/modules/service.md` — fix routes (no /v0/), singular account/subscription, Npgsql→Postgres, three config files | done |
| 22 | `api/modules/ui.md` — fill out: pages, JS stack, API connection, config file, status code pages | done |
| 23 | `lib/README.md` — add Benchmarks to Related Tests, fix "custom exceptions" (only one: DimensionException) | done |
| 24 | `lib/models.md` — fix PackedBin description, add ResultItem base class | done |
| 25 | `lib/processors.md` — two-axes table: many-bins/multi-algo returns IMultiAlgorithmBinProcessor not IBinProcessor | done |
| 26 | `tests/scenarios.md` — fix "used by 2 projects" → 4 (Benchmarks + PerformanceTests) | done |
| 27 | `packages/README.md` — remove binacle-vipaq row from packages table | done |

## Phase 4 — Missing Content

| # | Task | Status |
|---|------|--------|
| 28 | `api/v3/contracts.md` — fill in: v3 field names, outer wrapper, fit/pack enum values | done |
| 29 | `api/README.md` — add v4 request flow trace (HTTP → endpoint → service → processor → algorithm → result builder → response mapper → ViPaq) | done |

## Phase 5 — User Review Items (present before writing)

| # | Item | Status |
|---|------|--------|
| 30 | Draft "Critical Rules" block for CLAUDE.md — present to user for approval | pending |
| 31 | Draft `verified: YYYY-MM-DD` frontmatter for each doc — present to user | pending |
| 32 | Draft "Done when:" verification lines — present to user | pending |
| 33 | Draft "Also update:" side-effect notes — present to user | pending |
| 34 | Draft `status:` stability markers (frozen/stable/active/planned) — present to user | pending |

## Phase 6 — Infrastructure

| # | Task | Status |
|---|------|--------|
| 35 | Update CLAUDE.md Common Tasks table — fix all paths to new locations | done (done in Phase 2 via .agent-docs/README.md) |
| 36 | Create `_index.md` — flat manifest of every doc path + one-line description | done |
| 37 | Create `config/docs.sh` — script to regenerate `_index.md` from frontmatter | done |
| 38 | Add `@.agent-docs/_index.md` reference to CLAUDE.md | done |

## Deferred Code Fixes (not docs — address separately)

- `config/api.sh` — `U` alias maps to `WithServiceModuleOnly`, should be `WithUiModuleOnly`
- `api/src/Binacle.Net.Kernel` — `LegacyBindingResult<T>` and `LegacyValidatedBindingResult<T>` are dead code
- `api/src/Binacle.Net/v3/Contracts/PackResponse.cs` — `EarlyFail_*` on `BinPackResultStatus` are dead code (confirm with user before touching)
