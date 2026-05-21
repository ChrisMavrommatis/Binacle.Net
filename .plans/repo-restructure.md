# Repo Restructure Plan

Goal: reorganise the repo so each component owns its source and tests, sites group together,
and shared assets have a clear home. No behaviour changes — pure structure.

---

## Target Layout

```
lib/src  lib/test                    Binacle.Lib + Abstractions + all Lib tests
api/src  api/test  api/requests/     Binacle.Net + Kernel + Modules + integration tests + HTTP test files
vipaq/src  vipaq/test  vipaq/…       Binacle.ViPaq (C#) + binacle-vipaq (TS, self-contained)
shared/                              Binacle.TestsKernel + shared test data
packages/                            binacle-net-ui, cookies, theme-switcher (cross-cutting TS)
ruby/                                Jekyll gems (renamed from gems/)
docs/   web/   admin/               sites stay at root
results/                             benchmark + efficiency results (renamed from doc/)
assets/  samples/  config/          unchanged
```

---

## Phase 1 — Quick wins (no .slnx changes needed)

- [x] Rename `gems/` → `ruby/`
  - Update `docs/Gemfile`: 2 path references (`../gems/` → `../ruby/`)
  - Update `web/Gemfile`: 2 path references (`../gems/` → `../ruby/`)

- [x] Rename `doc/` → `results/`
  - Update any README or script references to `doc/`
  - See Phase 4 for migrating the content into the docs/web site

- [x] Move `res/or-library-packing-data/` → `shared/data/`
  - It's test input used by Lib benchmarks and performance tests — belongs with shared test infrastructure
  - Add a `README.md` explaining what the datasets are and where they come from

- [x] Move `res/http/` → `api/requests/`
  - All `.http` files test API or ServiceModule endpoints — everything lives in `api/`
  - Admin site calls ServiceModule endpoints, no separate `admin/requests/` needed
  - Add `v4/` folder (missing, branch is actively adding v4 endpoints)
  - Remove `res/` once empty

---

## Phase 2 — Main restructure (touches .slnx) ✓ done

Do this in one pass. Every .csproj path in `Binacle.Net.slnx` changes.
Check `Directory.Build.props` for relative paths that assume the old depth.

### lib/
```
lib/src/Binacle.Lib
lib/src/Binacle.Lib.Abstractions
lib/test/Binacle.Lib.UnitTests
lib/test/Binacle.Lib.Benchmarks
lib/test/Binacle.Lib.PerformanceTests
```

### api/
```
api/src/Binacle.Net
api/src/Binacle.Net.Kernel
api/src/Binacle.Net.DiagnosticsModule
api/src/Binacle.Net.ServiceModule
api/src/Binacle.Net.ServiceModule.Domain
api/src/Binacle.Net.ServiceModule.Infrastructure
api/src/Binacle.Net.UIModule
api/test/Binacle.Net.IntegrationTests
api/test/Binacle.Net.ServiceModule.IntegrationTests
```

### vipaq/
```
vipaq/src/Binacle.ViPaq
vipaq/test/Binacle.ViPaq.UnitTests
vipaq/binacle-vipaq/               TS npm package — keep its internal src/tests structure intact
```

### shared/
```
shared/Binacle.TestsKernel
```

### packages/
`binacle-net-ui`, `cookies`, `theme-switcher` stay at `packages/` — no move needed.
`binacle-vipaq` moves out of `packages/` into `vipaq/binacle-vipaq/` — remove from `packages/`.

---

## Phase 2b — Fix all references (do immediately after Phase 2)

Every path that pointed into `src/`, `test/`, `packages/`, or `gems/` needs updating.

### .NET
- [x] `Binacle.Net.slnx` — all `<Project>` paths (every project moves)
- [x] `Directory.Build.props` — no relative paths, no changes needed
- [x] Each `.csproj` with `<ProjectReference>` — paths to sibling projects change

### npm / TypeScript
- [x] Root `package.json` — added `vipaq/binacle-vipaq` as explicit workspace member
- [x] `gulpfile.js` — only references `assets/`, `docs/`, `web/` — no changes needed
- [x] `docs/webpack.config.js` — no package path aliases, no changes needed
- [x] `web/webpack.config.js` — updated `packages/binacle-vipaq` cache group → `vipaq/binacle-vipaq`
- [x] `docs/package.json` and `web/package.json` — no workspace references, no changes needed

### Ruby
- [x] `docs/Gemfile` — `../gems/` → `../ruby/` (done in Phase 1)
- [x] `web/Gemfile` — same

### Shell scripts
- [x] `config/api.sh` — `src/Binacle.Net/` → `api/src/Binacle.Net/`
- [x] `config/build.sh` — `src/Binacle.Net/Binacle.Net.csproj` → `api/src/Binacle.Net/Binacle.Net.csproj`
- [x] `config/tests.sh` — restructured: each alias now holds a full path from root
- [x] `config/benchmarks.sh` — `test/Binacle.Lib.Benchmarks/` → `lib/test/Binacle.Lib.Benchmarks/`

### CI/CD
- [x] `.github/workflows/release-docker-image.yml` — uses `${{ vars.API_PROJECT_PATH }}` (GitHub Actions variable).
  No file change needed — update the `API_PROJECT_PATH` repo variable in GitHub settings to `api/src/Binacle.Net/Binacle.Net.csproj`
- [x] `.github/workflows/deploy-binacle-net-docs.yml` — no path references, no changes needed
- [x] `.github/workflows/deploy-binacle-net-web.yml` — no path references, no changes needed

### Docker
- [x] `Dockerfile` — only copies from `build/output`, no source paths — no changes needed
- [x] `.dockerignore` — only `**/*.Development.json` — no changes needed
- [x] `config/docker-compose.yml` — infrastructure services only, no source paths — no changes needed
- [x] `samples/` docker-compose files — no build context paths referencing `src/` or `test/`

### IDE / project files
- ~~`Binacle.Net.sln.DotSettings.user`~~ — gitignored (`*.user`), local only, no action needed
- [x] `Properties/launchSettings.json` in each project — paths relative to project, survive the move
- [x] `*.proj` files (`http.proj`, `docs.proj`, `web.proj`, `benchmark-results.proj`,
  `packing-efficiency-results.proj`) — no sibling path references, no changes needed

### Jekyll config
- [x] `docs/_config.yml` and `docs/_config.prod.yml` — uses `source: ./` and `../build/docs`, no changes needed
- [x] `web/_config.yml` and `web/_config.prod.yml` — same

### Documentation
- [x] `README.md` — no path references, no changes needed
- [x] `.agent-docs/` — all path references updated across all 23 affected files:
  - `src/Binacle.*` → `lib/src/`, `api/src/`, `vipaq/src/`
  - `test/Binacle.*` → `lib/test/`, `api/test/`, `vipaq/test/`, `shared/`
  - `packages/binacle-vipaq` → `vipaq/binacle-vipaq`
  - `gems/` description text → `ruby/`
- [x] `CLAUDE.md` — no direct path references, no changes needed

---

## Phase 3 — Migrate results content into docs or web site

`results/` (was `doc/`) holds benchmark reports and packing efficiency analysis.
These are useful to publish but currently sit as raw markdown in a repo folder.

Options — decide before doing:
- Integrate into the **docs site** under a "Performance" or "Benchmarks" section
- Integrate into the **web site** as a data-driven page
- Keep as raw markdown in `results/` with better README structure (least effort)

Tasks once decided:
- [ ] Move or link benchmark markdown into the chosen site's collections or pages
- [ ] Add navigation entry in the site's `_data/` header/footer config
- [ ] Archive the raw BenchmarkDotNet JSON/MD result files somewhere (or keep in `results/raw/`)

---

## Phase 4 — Cleanup and annotations

- [x] Add `README.md` to `ruby/` explaining the gems and which sites use them

- [x] Add `README.md` to `shared/` explaining `Binacle.TestsKernel` and who depends on it

- [x] Annotate `api/src/Binacle.Net.ServiceModule/v0/` in the agent docs and README
  - v0 endpoints are fully documented in `.agent-docs/api/module-service.md` — no gaps

- [x] Add `spec/` test suites to both Ruby gems (`jekyll-filters`, `jekyll-gtm`)
  - `spec/spec_helper.rb`, `spec/capitalization_filters_spec.rb`, `spec/sanitization_filters_spec.rb`
  - `spec/spec_helper.rb`, `spec/gtm_tags_spec.rb`
  - Added `rspec ~> 3.0` as dev dependency in both gemspecs

- [x] Add `v4/` to `api/requests/` with HTTP test files for all new v4 endpoints
  - 6 files: List All Presets, Fit By Custom Bin, Fit By Preset Bin, Pack By Custom Bin, Pack By Preset Bin, Pack Smallest Bin

---

## Phase 5 — Tidy-ups

- [x] Collapse `api/requests/http/` — moved all contents up into `api/requests/` directly.
  - Renamed `http.proj` → `requests.proj`; globs unchanged (still relative, still correct)
  - Updated `Binacle.Net.slnx` project path

- [x] Renamed `.agent-docs/gems/` → `.agent-docs/ruby/`
  - Updated `[Gems](gems/README.md)` → `[Ruby](ruby/README.md)` in `.agent-docs/README.md`

---

## Pending Decisions

### ~~cookies + theme-switcher merge~~
Decided: keep separate. Cleaner dependency graph, easier to replace independently.

---

## Notes

- Build outputs (`build/`, `node_modules/`) are gitignored — no action needed
- Favicon and frontend lib files in `docs/`, `web/`, `assets/` are copied by npm tasks — not duplication
- `config/` stays as-is (local dev setup, docker-compose, scripts)
- `assets/` stays as-is (single source for branding, npm tasks copy to sites)
- `samples/` stays as-is
