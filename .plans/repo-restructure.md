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

- [ ] Rename `gems/` → `ruby/`
  - Update `docs/Gemfile`: 2 path references (`../gems/` → `../ruby/`)
  - Update `web/Gemfile`: 2 path references (`../gems/` → `../ruby/`)

- [ ] Rename `doc/` → `results/`
  - Update any README or script references to `doc/`
  - See Phase 4 for migrating the content into the docs/web site

- [ ] Move `res/or-library-packing-data/` → `shared/data/`
  - It's test input used by Lib benchmarks and performance tests — belongs with shared test infrastructure
  - Add a `README.md` explaining what the datasets are and where they come from

- [ ] Move `res/http/` → `api/requests/`
  - All `.http` files test API or ServiceModule endpoints — everything lives in `api/`
  - Admin site calls ServiceModule endpoints, no separate `admin/requests/` needed
  - Add `v4/` folder (missing, branch is actively adding v4 endpoints)
  - Remove `res/` once empty

---

## Phase 2 — Main restructure (touches .slnx)

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
- [ ] `Binacle.Net.slnx` — all `<Project>` paths (every project moves)
- [ ] `Directory.Build.props` — check for relative paths that assume old folder depth
- [ ] Each `.csproj` with `<ProjectReference>` — paths to sibling projects change

### npm / TypeScript
- [ ] Root `package.json` — workspace globs: `packages/*` is unchanged, but `vipaq/binacle-vipaq`
  is now a workspace member at a new path — add it explicitly if not already
- [ ] `gulpfile.js` — any copy task source paths referencing `packages/` or `src/`
- [ ] `docs/webpack.config.js` and `web/webpack.config.js` — check for package path aliases
- [ ] `docs/package.json` and `web/package.json` — check workspace package references

### Ruby
- [ ] `docs/Gemfile` — `../gems/` → `../ruby/` (2 lines, already in Phase 1)
- [ ] `web/Gemfile` — same

### Shell scripts
- [ ] `config/api.sh` — likely references `src/Binacle.Net`
- [ ] `config/build.sh` — likely references `src/` paths
- [ ] `config/tests.sh` — likely references `test/` paths
- [ ] `config/benchmarks.sh` — likely references `test/Binacle.Lib.Benchmarks`

### CI/CD
- [ ] `.github/workflows/release-docker-image.yml` — check Dockerfile context path
- [ ] `.github/workflows/deploy-binacle-net-docs.yml` — check working directory
- [ ] `.github/workflows/deploy-binacle-net-web.yml` — check working directory

### Docker
- [ ] `Dockerfile` — `COPY` instructions reference `src/` project paths
- [ ] `.dockerignore` — exclusion patterns may reference `src/` or `test/`
- [ ] `config/docker-compose.yml` — volume mounts or build context paths
- [ ] `samples/` docker-compose files — check build context paths

### IDE / project files
- ~~`Binacle.Net.sln.DotSettings.user`~~ — gitignored (`*.user`), local only, no action needed
- [ ] `Properties/launchSettings.json` in each project — paths are relative to the project so
  should survive the move, but verify each one runs correctly after
- [ ] `*.proj` files (`http.proj`, `docs.proj`, `web.proj`, `benchmark-results.proj`,
  `packing-efficiency-results.proj`) — check if any reference sibling paths

### Jekyll config
- [ ] `docs/_config.yml` and `docs/_config.prod.yml` — check `source`, `plugins_dir`,
  or any hardcoded paths
- [ ] `web/_config.yml` and `web/_config.prod.yml` — same

### Documentation
- [ ] `README.md` — any path references to `src/`, `test/`, `packages/`, `gems/`, `doc/`
- [ ] `.agent-docs/` — extensive path references throughout; update all docs that mention:
  - `src/Binacle.*` → `lib/src/`, `api/src/`, `vipaq/src/`
  - `test/Binacle.*` → `lib/test/`, `api/test/`, `vipaq/test/`, `shared/`
  - `packages/binacle-vipaq` → `vipaq/binacle-vipaq`
  - `gems/` → `ruby/`
  - `doc/` → `results/`
  - `res/http/` → `api/requests/`
  - `res/or-library-packing-data/` → `shared/data/`
- [ ] `CLAUDE.md` — check if it references any paths directly

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

- [ ] Add `README.md` to `ruby/` explaining the gems and which sites use them

- [ ] Add `README.md` to `shared/` explaining `Binacle.TestsKernel` and who depends on it

- [ ] Annotate `api/src/Binacle.Net.ServiceModule/v0/` in the agent docs and README
  - Currently undocumented alongside v2/v3/v4 — at minimum note its existence and purpose

- [ ] Add `spec/` test suites to both Ruby gems (`jekyll-filters`, `jekyll-gtm`)
  - Convention: RSpec, `spec/` folder, `spec_helper.rb`

- [ ] Add `v4/` to `api/requests/` with HTTP test files for all new v4 endpoints

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
