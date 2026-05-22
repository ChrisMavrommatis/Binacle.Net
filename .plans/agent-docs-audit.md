# Agent Docs Audit

Goal: review every doc in `.agent-docs/` against the code, flag inaccuracies and gaps.
No fixes until cataloguing is complete and execution is planned.

Status legend: `pending` | `in-review` | `done`

---

## Sections

| # | Doc | Status | Notes |
|---|-----|--------|-------|
| 1 | `README.md` (root) | done | |
| 2 | `concepts/README.md` | done | |
| 3 | `concepts/fit-vs-pack.md` | done | |
| 4 | `commands.md` | done | |
| 5 | `api/README.md` | done | |
| 6 | `api/contracts.md` | done | |
| 7 | `api/endpoints.md` | done | |
| 8 | `api/v3.md` | done | |
| 9 | `api/v4.md` | done | |
| 10 | `api/add-endpoint.md` | done | |
| 11 | `api/service.md` | done | |
| 12 | `api/kernel.md` | done | |
| 13 | `api/presets.md` | done | |
| 14 | `api/modules.md` | done | |
| 15 | `api/module-diagnostics.md` | done | |
| 16 | `api/module-service.md` | done | |
| 17 | `api/module-ui.md` | done | |
| 18 | `api/configuration.md` | done | |
| 19 | `lib/README.md` | done | |
| 20 | `lib/models.md` | done | |
| 21 | `lib/algorithms.md` | done | |
| 22 | `lib/algorithm-factory.md` | done | |
| 23 | `lib/processors.md` | done | |
| 24 | `lib/result-building.md` | done | |
| 25 | `lib/result-selection.md` | done | |
| 26 | `tests/README.md` | done | |
| 27 | `tests/scenarios.md` | done | |
| 28 | `vipaq/README.md` | done | |
| 29 | `packages/README.md` | done | |
| 30 | `docs/README.md` | done | |
| 31 | `web/README.md` | done | |
| 32 | `ruby/README.md` | done | |

---

## Findings

Findings are recorded here after each section is reviewed.
Format per section:

### 1. `README.md` (root)
- **Inaccurate:**
  - `api/src/Binacle.Net.ServiceModule` row should note it has 3 sub-projects:
    `Binacle.Net.ServiceModule`, `Binacle.Net.ServiceModule.Domain`, `Binacle.Net.ServiceModule.Infrastructure`.
    They are one module split across 3 projects.
  - `api/test/` should be two rows: `Binacle.Net.IntegrationTests` (tests base API)
    and `Binacle.Net.ServiceModule.IntegrationTests` (tests endpoints the ServiceModule adds).
- **Missing (oversight, should be added):**
  - `api/requests/` — HTTP request files for manual testing, subfolders: v2, v3, v4, Service.
  - `samples/` — Docker and Kubernetes deployment samples.
  - `shared/data/` — OR-library packing data.

### 2. `concepts/README.md`
- **Inaccurate:** nothing.
- **Structural decision:** collapse `concepts/` folder entirely.
  Both `concepts/README.md` and `concepts/fit-vs-pack.md` merge into a single `concepts.md` at the root of `.agent-docs/`.
  The `concepts/` folder is deleted.

### 3. `concepts/fit-vs-pack.md`
- **Inaccurate:** nothing.
- **Missing:** doc implies pack is the operation that tells you "what packed / what didn't."
  In reality both fit and pack return the same result shape — packed items and unpacked items.
  Fit just exits early, so the result reflects where it stopped.
  This distinction matters for understanding the API response shape.
- **Do not add:** pre-loop early-exit checks (volume/dimension exceeded) — implementation detail.
- **Do not add:** position/coordinate data — implementation detail.

### 4. `commands.md`
- **Inaccurate (script is source of truth):**
  - `api.sh` alias for `WithAllModules`: doc says `A`, script has `All`. No `A` alias exists.
  - `api.sh` alias `U`: script maps `U` → `WithServiceModuleOnly` (likely a bug in the script).
    Doc says `U` → UIModule. Script needs fixing, not the doc.
- **Missing:**
  - `docker-compose.yml` and `docker-compose.build.yml` in `config/` — not mentioned.
    Worth a brief mention as the way to run the full stack with dependencies locally.
- **Intentionally omitted:**
  - `tmux.sh` — personal dev workflow shortcut, not useful for agents or contributors.

### 5. `api/README.md`
- **Inaccurate:**
  - Startup order: `AddBinacleServices()` is shown after modules but in `Program.cs` it runs
    before `AddDiagnosticsModule()` (line 79 vs 127).
- **Missing from startup diagram (add these):**
  - `Feature.Manager` init — reads from config + env vars, happens before any module registration.
    Controls all feature flags. Agents need to know it exists to understand conditional wiring.
  - `SWAGGER_UI` and `SCALAR_UI` feature flags — same conditional pattern as module flags,
    should be shown so an agent knows the pattern when adding similar flags.
- **Intentionally omit from startup diagram:**
  - CORS setup, `UseHttpsRedirection`, `UseExceptionHandler` — boilerplate, not project-specific.
- **Structural decision (pending):** consider splitting `api/` docs into `api/v3/` and `api/v4/`
  subfolders, each with their own `contracts.md` and `endpoints.md`.
  Currently `contracts.md` is v4-focused with a hand-wave at v3.
  Separate `v3.md` / `v4.md` files already exist — this would formalise that split.
  Decision deferred to execution planning.
### 6. `api/contracts.md`
- **Inaccurate:**
  - "`Best` and `null` both trigger multi-algorithm path" — from the API, `null` fails the
    `NotNull()` validator. Only `Best` is a valid caller input. The null-path in
    `GetAlgorithm()` is internal and not reachable via the API.
  - `CustomBinsRequestBase` "Used by" says "multi-bin requests" — only `PackCustomSmallestBinRequest`
    uses it currently.
  - `PresetBinsRequestBase` "Used by" says "preset multi-bin requests" — nothing uses it yet.
    It is planned for a future endpoint.
- **Missing:**
  - `Unknown = -1` on both `BinFitResultStatus` and `BinPackResultStatus` — not mentioned.
    Matters for client-side deserializers and edge case handling.
- **Structural decision (pending):** see note under `api/README.md` — consider `api/v3/` and
  `api/v4/` subfolder split for contracts and endpoints docs.

### 7. `api/endpoints.md`
- **Inaccurate:**
  - `IGroupedEndpoint<TGroup>` — doc says it "implements `DefineEndpoint(RouteGroupBuilder)`"
    but that method is on the non-generic `IGroupedEndpoint` base.
    `IGroupedEndpoint<TGroup>` only adds the group type constraint.
    The non-generic `IGroupedEndpoint` is not mentioned at all.
- **Missing:**
  - Each module has its own `IModuleMarker` — doc implies it's a single shared interface.
    In reality each module (`DiagnosticsModule`, `ServiceModule`, `UIModule`) defines its own.
    An agent adding a new module needs to know to create one.
- **Confirmed accurate:**
  - Route prefixes, group metadata, validation flow, rate limiting behaviour, tags.

### 8. `api/v3.md`
- **Inaccurate:**
  - Endpoint table says "a custom bin" (singular) — v3 custom endpoints take `List<Bin> Bins`.
    v3 operates on **multiple bins per request** and returns a result per bin.
    This is a key difference from v4 (single bin per request).
  - "Fit returns pass/fail status and volume percentages only" — also returns
    `FittedItems` and `UnfittedItems` (without coordinates). The "only" is wrong.
  - Early exit table: "Pack — no" is correct. But `BinPackResultStatus` has
    `EarlyFail_ContainerVolumeExceeded` / `EarlyFail_ContainerDimensionExceeded` —
    these are dead code. Pack never early-exits. Worth noting in the doc to avoid confusion.
- **Missing:**
  - v3 uses different field names than v4 — none of this is described, only implied by example:
    - `Result` (not `Status`) per bin result
    - `FittedItems` / `UnfittedItems` for fit (not `PackedItems`)
    - `FittedBinVolumePercentage` / `FittedItemsVolumePercentage` for fit
    - Outer `{ "result": "Success/Failure", "data": [...] }` wrapper on all responses
  - v3 Fit status enum values not documented: `AllItemsFit`, `NotAllItemsFit`,
    `EarlyFail_TotalVolumeExceeded`, `EarlyFail_ItemDimensionExceeded`.
  - v3 Pack status enum values not documented: `Unknown`, `NotPacked`, `PartiallyPacked`,
    `FullyPacked`, `EarlyFail_ContainerVolumeExceeded`, `EarlyFail_ContainerDimensionExceeded`
    (last two are dead code — pack never triggers early exit).

### 9. `api/v4.md`
- **Inaccurate:**
  - Algorithm Selection: "Use `Best` (or leave it out)" — null fails the `NotNull()` validator.
    You cannot omit the field. Should say "Use `Best`" only.
- **Confirmed accurate:**
  - All implemented endpoint routes, planned endpoints section, response shape table.

### 10. `api/add-endpoint.md`
- **Inaccurate:**
  - Template comments say `.RequireRateLimiting("ApiUsage") // only when ServiceModule is active`
    — misleading. These should always be added unconditionally (no-ops without ServiceModule).
    Wording implies conditional inclusion. Should match `endpoints.md`: "safe to include,
    only active when the module is enabled."
- **Missing:**
  - No mention of the 404 case for preset endpoints. Adding a `{preset}/{bin}` route requires
    handling "preset or bin not found" with `Results.NotFound()` and
    `.Produces(StatusCodes.Status404NotFound)`. `PresetBin` does this but the guide doesn't
    call it out.
- **Confirmed accurate:**
  - All steps, paths, handler signature, `BinResponseBase.From<T>()` pattern, auto-registration.
- **Structural (ties to v3/v4 split decision):**
  - This doc is v4-specific — should move to `api/v4/add-endpoint.md` in the split.
  - `api/v3/` (or `api/v3.md`) should explicitly tell agents: do NOT add endpoints here.

### 15. `api/module-diagnostics.md`
- **Inaccurate:** nothing.
- **Missing:**
  - Config files table only shows `Serilog.{Environment}.json` as having an environment variant.
    In reality all config files have environment overrides:
    `HealthChecks.Development.json`, `OpenTelemetry.Development.json`,
    `PackingLogs.Development.json` all exist alongside their base files.
- **Not verified (too deep):** "Two channels, one per operation type" for packing logs —
  config has two sections (Fitting/Packing) but single channel type registered.
  Internal split unclear without deeper investigation.
- **Confirmed accurate:**
  - All config file paths, PackingLogs defaults, BootstrapLogger config,
    registration order, health check, OpenTelemetry structure.

### 14. `api/modules.md`
- **Inaccurate:** nothing.
- **Missing:** Scalar UI has no mount path listed (Swagger UI lists `/swagger`).
  An agent enabling Scalar should know where it mounts.
- **Confirmed accurate:** all flags, defaults, Add/Use pattern, launch profile names,
  DiagnosticsModule always-on behaviour.
- **Structural decision (pending):**
  - `modules.md` content is short — could fold into `api/README.md`.
  - `module-diagnostics.md`, `module-service.md`, `module-ui.md` could move to
    per-module subfolders: `api/modules/diagnostics/`, `api/modules/service/`,
    `api/modules/ui/`.

### 13. `api/presets.md`
- **Inaccurate:**
  - "Adding a preset for tests" section is entirely wrong. There is no test `Presets.json`.
    Tests configure presets in code via `services.Configure<BinPresetOptions>()` inside
    `BinacleApi.ConfigureWebHost` — defaults are cleared and test presets are added
    programmatically from scenario providers.
    Correct steps:
    1. Add a constant to `PresetKeys.cs`
    2. Add entry in `BinacleApi.ConfigureWebHost` via `options.Presets.Add(PresetKeys.YourKey, ...)`
    3. Reference via `PresetKeys.YourKey` in tests
- **Missing:**
  - v3 preset endpoints use only `{preset}` (no `{bin}`) and try all bins in the preset.
    The doc only describes the v4 `{preset}/{bin}` pattern.
  - Preset file is required (`Optional => false`) — app fails to start without `Presets.json`.
- **Confirmed accurate:**
  - Config structure, section name, default preset dimensions, route params, caching behavior.

### 12. `api/kernel.md`
- **Inaccurate:**
  - `IModuleMarker` section: "each module **(and the core API)** defines its own `IModuleMarker`"
    — the core API uses `IApiMarker`, not `IModuleMarker`. Separate interfaces.
  - Same section: "Used by DiagnosticsModule and ServiceModule" — UIModule also has one.
    All three modules define their own `IModuleMarker`.
- **Note (not a doc issue):** `LegacyBindingResult<T>` and `LegacyValidatedBindingResult<T>`
  exist in Kernel but are unused — dead code, candidate for cleanup.
- **Confirmed accurate:**
  - `BindingResult<T>` all four conditions and responses, `IOptionalDependency<T>`,
    `Feature.Manager`, `IConfigurationOptions`, `IOpenApiDocument`, `IStartupTask`.

### 11. `api/service.md`
- **Inaccurate:**
  - Calling pattern note: `GetAlgorithm()` returns null "for `Best` **or when no algorithm is set**"
    — null fails the `NotNull()` validator; you cannot omit the field.
    "Or when no algorithm is set" should be removed. Same issue as contracts.md and v4.md.
- **Confirmed accurate:**
  - All 6 method signatures, result selection strategies (`BestAlgorithm`, `SmallestBin`),
    calling pattern, `.ForFittingOperation()` / `.ForPackingOperation()`, mutation warning.
- **Confirmed accurate:**
  - `UseServiceModule()` comment "v0 endpoints" — ServiceModule has a real `v0/` folder.
  - Dependency map references are correct.
  - Module listing and flag names (`SERVICE_MODULE`, `UI_MODULE`) correct.
- **Dependency map — clarity gap:**
  - The map lists `Binacle.Lib.*`, `Binacle.Net.*`, and `Binacle.ViPaq` flat with no indication
    they live in different top-level directories (`lib/`, `api/`, `vipaq/`).
    An agent reading it could assume they're all under `api/src/`.
    The map should make their physical location clear.

### 16. `api/module-service.md`
- **Inaccurate:**
  - Endpoint routes all have `/v0/` in the URL — wrong. `v0` is code organisation only, not part of the URL.
    Actual routes: `/api/auth/token`, `/api/admin/...` (no `/v0/` segment anywhere).
  - Route table uses `accounts` (plural) — wrong. Singular: `/api/admin/account`, `/api/admin/account/{id}`.
  - Route table uses `subscriptions` (plural) and `{subId}` param — wrong.
    Actual: `/api/admin/account/{id}/subscription` (singular, no subscription ID in URL).
  - Npgsql connection string name: doc says `Npgsql` — wrong. Provider uses `"Postgres"`.
  - Config files table says `ConnectionStrings.json` holds rate limiter rules and JWT options — wrong.
    There are three separate config files:
    - `Config_Files/ServiceModule/ConnectionStrings.json` — DB connection strings only (optional)
    - `Config_Files/ServiceModule/RateLimiter.json` — rate limiter rules (required)
    - `Config_Files/ServiceModule/JwtAuth.json` — JWT options (optional; loaded via `AddValidatableJsonConfigurationOptions`)
  - "Rate limiter config loaded from ConnectionStrings.json" — wrong file. It's `RateLimiter.json`.
  - "JWT secret, issuer, and audience are in JwtAuthOptions (loaded from same file)" — wrong. Loaded from `JwtAuth.json`.
- **Missing:**
  - `RateLimiter.json` has three rate limit configs: `ApiUsageAnonymous`, `AuthToken`, `ApiUsageDemoSubscription`.
    The doc says "two policies" — that's correct (only `ApiUsage` and `AuthToken` are registered as policies),
    but the `ApiUsageDemoSubscription` config is used internally by the `ApiUsage` policy to differentiate users.
    Not mentioned at all — worth a note.
- **Confirmed accurate:**
  - Three projects, dependency direction (ServiceModule → Domain ← Infrastructure).
  - Registration pattern (`AddServiceModule` / `UseServiceModule`).
  - Domain entities, patterns, repository interfaces.
  - Infrastructure DB backend selection order: Azure Storage → Postgres → SQLite.
  - Password hashers: PlainText, Sha256, Pbkdf2 — all registered as `IPasswordHasher`.
  - Startup tasks: schema task per provider + `EnsureDefaultAdminAccountExistsStartupTask`.
  - Admin policy: authenticated + `ClaimTypes.Role == "Admin"`.
  - `BINACLE_ADMIN_CREDENTIALS` env var for default admin credentials.
  - Development user secrets loaded via `IModuleMarker` assembly.
  - "Two policies registered" — names `ApiUsage` and `AuthToken` are correct.

### 17. `api/module-ui.md`
- **Inaccurate:**
  - "Blazor Server" — in .NET 10 this is Blazor Web App with Interactive Server rendering mode
    (`AddInteractiveServerComponents` / `AddInteractiveServerRenderMode`). "Blazor Server" is an older term.
- **Missing (fill this doc out — future API and JS work derives from it):**
  - **Pages:** `/` (Home), `/PackingDemo`, `/ProtocolDecoder`, `/Error`, `/Error/{ErrorCode}`
  - **PackingDemo** — form where users enter bins/items, calls the pack API, shows results in a 3D viewer.
  - **ProtocolDecoder** — paste a ViPaq-encoded pack result; decodes and renders it in the 3D viewer without calling the API.
  - **JS stack:**
    - Three.js (loaded from CDN via importmap: `https://cdn.jsdelivr.net/npm/three@0.176.0/...`)
    - `binacle/addons/` importmap alias → `wwwroot/js/addons/` (local)
    - Custom JS: `PackingVisualizer.js` (3D scene), `PackingVisualizer.utils.js` (Three.js helpers),
      `cookies.js`, `themeswitcher.js`
    - Vendor (bundled): BeerCSS, material-dynamic-colors
  - **API connection:** named `HttpClient` ("BinacleApi") posts to the same host by default.
    Config file `Config_Files/UiModule/ConnectionStrings.json` (optional) can override with `BinacleApi` connection string.
  - **Services (all scoped — per connection/tab):** `ThemeService`, `MessagingService`,
    `BinacleVisualizerService`, `LocalStorageService`, `SampleDataService`.
    `AppletsService` is singleton.
  - **Status code pages disabled for:** `/api`, `/swagger`, `/scalar` — so Blazor error middleware
    does not intercept API or OpenAPI responses.
- **Confirmed accurate:**
  - Feature flag `UI_MODULE` ✓
  - Path `api/src/Binacle.Net.UIModule` ✓

### 18. `api/configuration.md`
- **Structural decision:** Option B — `configuration.md` owns file tree + precedence rules; per-key details
  stay in each module's doc. Add a one-line description per file in the tree so an agent knows where to look.
- **Inaccurate:**
  - Config file tree is missing the entire ServiceModule subtree. Missing files:
    - `Config_Files/ServiceModule/ConnectionStrings.json` — DB connection strings (optional)
    - `Config_Files/ServiceModule/RateLimiter.json` — rate limiter rules (required)
    - `Config_Files/ServiceModule/JwtAuth.json` — JWT options (optional)
  - Override pattern is documented as `.Production.json` — wrong. Code uses `.{EnvironmentName}.json`
    (any ASP.NET environment name: Development, Production, Staging, etc.).
  - DiagnosticsModule file tree is incomplete — all four files have environment overrides, not just Serilog:
    `HealthChecks.{Environment}.json`, `OpenTelemetry.{Environment}.json`,
    `PackingLogs.{Environment}.json`, `Serilog.{Environment}.json` all exist.
- **Missing:**
  - Scalar UI mounts at `/scalar` — not mentioned (Swagger is mentioned in feature flags table but no mount path
    for either; this is a gap in the feature flags table).
  - `BINACLE_ADMIN_CREDENTIALS` env var — used by ServiceModule to seed the default admin account on first run.
    Not a feature flag, not a config file — a standalone env var worth documenting here.
- **Confirmed accurate:**
  - Env-var `__` separator convention ✓
  - Connection-string fallback pattern (`<NAME>_CONNECTION_STRING`) ✓
  - Precedence table order ✓
  - Feature flags table (SERVICE_MODULE, UI_MODULE, SWAGGER_UI, SCALAR_UI) ✓
  - `ASPNETCORE_HTTP_PORTS` default 8080 ✓

### 19. `lib/README.md`
- **Inaccurate:** nothing.
- **Missing:**
  - `lib/test/Binacle.Lib.Benchmarks` — not in Related Tests table. BenchmarkDotNet project (not xUnit),
    run via `config/benchmarks.sh` with filters `AlgorithmRacing` and `FastValidation`.
    An agent touching algorithms needs to know it exists.
- **Minor note:** "Custom exceptions" (plural) — currently only one exists: `DimensionException`.
  Not wrong as guidance, but worth knowing when writing the fix.
- **Confirmed accurate:**
  - Two source projects, all doc links, GuardClauses list, `Exceptions/` path.
  - Test aliases `lib` → `Binacle.Lib.UnitTests`, `performance` → `Binacle.Lib.PerformanceTests` ✓

### 20. `lib/models.md`
- **Inaccurate:**
  - `PackedBin` description says "Bin reference used in result output" — misleading.
    It extends `ResultItem` (same base as `PackedItem` and `UnpackedItem`), copying ID and dimensions,
    computing volume. Not a reference to the input `Bin` object.
- **Missing:**
  - `ResultItem` abstract base class — not documented. All three result types (`PackedItem`,
    `UnpackedItem`, `PackedBin`) inherit from it. Carries `ID`, `Dimensions` (copied as value), and
    `Volume` (computed). An agent writing result-processing code needs to know it exists.
- **Confirmed accurate:**
  - `Bin` and `Item` input models, namespaces, file paths, interface implementations ✓
  - All 10 `IWith*` interfaces and their file paths ✓
  - `PackedItem` (ID, dimensions, Coordinates), `UnpackedItem` (ID, dimensions, quantity) ✓
  - `Dimensions`, `Coordinates`, `AlgorithmInfo` value types ✓
  - Note about API-level `IWith*` being a separate set ✓

### 21. `lib/algorithms.md`
- **Inaccurate:** nothing.
- **Confirmed accurate:**
  - Three heuristics × two versions, folder paths ✓
  - `AlgorithmFactory.cs` path — registered public factory at `lib/src/Binacle.Lib/AlgorithmFactory.cs`
    uses v2 for all three. `AlgorithmFactory_v1.cs` / `AlgorithmFactory_v2.cs` in `AlgorithmFactories/`
    are kept for benchmarking only ✓
  - FFD/WFD/BFD trade-off descriptions, guarantee section, operation types ✓

### 22. `lib/algorithm-factory.md`
- **Inaccurate:** nothing.
- **Confirmed accurate:**
  - `IAlgorithmFactory` signature and type constraints ✓
  - `IPackingAlgorithm` exposes `Algorithm`, `Version`, `Execute(parameters)` ✓
  - DI registration: `AddSingleton<IAlgorithmFactory, AlgorithmFactory>()` in
    `ServiceCollectionExtensions.AddBinacleServices()` ✓
  - `AlgorithmFactory_v1`/`v2` are `internal` ✓
  - Test `AlgorithmFactories.cs` — all six delegate factories match code exactly ✓

### 23. `lib/processors.md`
- **Inaccurate:**
  - Two-axes table: "Many bins, many algorithms → `IBinProcessor` → `LoopMultiAlgorithmBinProcessor`" — wrong.
    `BinProcessorFactory.CreateMultiAlgorithm` returns `IMultiAlgorithmBinProcessor`, not `IBinProcessor`.
    They are separate interfaces: `IBinProcessor.Process` takes an explicit `Algorithm` argument;
    `IMultiAlgorithmBinProcessor.Process` does not. Doc implies they share the same type.
- **Confirmed accurate:**
  - `IAlgorithmProcessor.Process` signature ✓
  - `AlgorithmProcessorFactory` creates `LoopAlgorithmProcessor` with FFD + WFD + BFD ✓
  - `BinProcessorFactory.CreateMultiAlgorithm` uses FFD + BFD only (no WFD) ✓
  - `LoopMultiAlgorithmBinProcessor` applies `BestAlgorithm` per bin ✓
  - Parallel variants (`ParallelAlgorithmProcessor`, `ParallelBinProcessor`,
    `ParallelMultiAlgorithmBinProcessor`) exist but are not wired up ✓
  - Result selection quick reference table, Diagnostics activity names ✓

### 24. `lib/result-building.md`
- **Inaccurate:** nothing.
- **Confirmed accurate:**
  - Builder usage pattern; `EarlyExit` calls `Complete()` then overwrites status ✓
  - `EarlyExitReason` values (`None`, `ContainerVolumeExceeded`, `ContainerDimensionExceeded`) ✓
  - All four status rules and conditions ✓
  - `WithUnpackedItems` grouping by ID ✓
  - Both volume percentage formulas, rounded to 2dp ✓
  - Integrity checks (count + volume mismatch) ✓
  - `OperationResult` has internal constructor, defined in `OperationResultStatus.cs` ✓

### 25. `lib/result-selection.md`
- **Inaccurate:** nothing.
- **Confirmed accurate:**
  - `IResultSelector` three methods, `IResultSelectionStrategy.Select` ✓
  - `ArgumentException` if empty, single-entry fast path ✓
  - DI registration (`BestBin_v2`, `SmallestBin_v2`, `BestAlgorithm_v2`) matches code exactly ✓
  - `BestAlgorithm_v2` scoring (`1000 + PackedItemsVolumePercentage`) ✓
  - `SmallestBin_v2` three-level priority (FullyPacked > smaller volume > higher PackedItemsVolumePercentage) ✓
  - `BestBin_v2` not currently called by service ✓
  - Test data path `Binacle.TestsKernel/ResultSelection/Data/{BestAlgorithm,BestBin,SmallestBin}` ✓

### 26. `tests/README.md`
- **Inaccurate:** nothing.
- **Structural decision:** `tests/README.md` and `tests/scenarios.md` could move into each slice's docs
  (`lib/`, `api/`, `vipaq/`) to make each slice self-contained. The current `tests/` folder duplicates
  what `lib/README.md` already has in its "Related Tests" section. Decision for execution planning.
- **Confirmed accurate:**
  - Stack: xUnit v3 + Shouldly + Bogus ✓
  - All 6 test projects with correct aliases ✓
  - Integration tests organized under `Tests/v3/` and `Tests/v4/` ✓
  - Benchmarks correctly distinguished from PerformanceTests ✓

### 27. `tests/scenarios.md`
- **Inaccurate:**
  - "Used by `Binacle.Lib.UnitTests` and `Binacle.Net.IntegrationTests`" — wrong.
    `Binacle.TestsKernel` is also referenced by `Binacle.Lib.Benchmarks` and `Binacle.Lib.PerformanceTests`.
    Four consumers, not two.
- **Confirmed accurate:**
  - Bischoff Suite + Custom Problems suites ✓
  - Compact string format (Bin `"LxWxH"`, Items `"LxWxH-Qty"`, Metrics, Result) matches actual JSON ✓
  - Three algorithm providers confirmed ✓
  - `CommonTestingFixture` and `ResultSelectionTestingFixture` exist ✓
  - Result selection data under `ResultSelection/Data/` ✓

### 28. `vipaq/README.md`
- **Inaccurate:** nothing.
- **Note:** doc acknowledges its own gaps (byte offsets, version negotiation, full API surface) — leave as-is.
- **Confirmed accurate:**
  - `ViPaqData` field exists on both v3 and v4 pack contracts behind `IncludeViPaqData` flag ✓
  - TypeScript mirror at `vipaq/binacle-vipaq` ✓
  - Gzip encoding confirmed in `ViPaqSerializer` ✓
  - Test project `vipaq/test/Binacle.ViPaq.UnitTests` ✓

### 29. `packages/README.md`
- **Inaccurate:** `binacle-vipaq` listed in the table with a "moved to `vipaq/binacle-vipaq/`" note — slightly confusing.
  The three actual packages under `packages/` are `binacle-net-ui`, `cookies`, `theme-switcher`.
  The table should drop `binacle-vipaq` and just reference `vipaq/binacle-vipaq` from the ViPaq doc.
- **Note:** Explicit Gap section — no further expansion expected here.
- **Confirmed accurate:** three remaining packages exist ✓

### 30. `docs/README.md`
- **Inaccurate:** nothing.
- **Note:** Explicit Gap section. "Jekyll + webpack + TypeScript" confirmed (`webpack.config.js`, `package.json` present) ✓

### 31. `web/README.md`
- **Inaccurate:** nothing.
- **Note:** Explicit Gap section. Same structure as docs — webpack confirmed ✓

### 32. `ruby/README.md`
- **Inaccurate:** nothing.
- **Confirmed accurate:** `jekyll-filters` and `jekyll-gtm` gems exist ✓

---

## Confirmed Structure (post-audit)

All structural decisions resolved. Target layout for `.agent-docs/`:

```
.agent-docs/
├── _index.md                     NEW — flat manifest: every doc path + one-line description.
│                                       Eager-loaded via @ in CLAUDE.md alongside root README.
│                                       Gives agents a full doc surface scan without reading each file.
├── README.md                     UPDATE — absorb concepts/README.md content, fix repo layout table,
│                                          add missing paths (api/requests/, samples/, shared/data/).
├── commands.md                   UPDATE — fix api.sh aliases, add docker-compose mention.
├── concepts.md                   NEW — merge concepts/README.md + concepts/fit-vs-pack.md.
│                                        Delete concepts/ folder. Fix missing: both fit and pack
│                                        return same result shape; fit exits early.
├── api/
│   ├── README.md                 UPDATE — fix startup order, add Feature.Manager + SWAGGER_UI/SCALAR_UI
│   │                                      to startup diagram, update all cross-links.
│   ├── service.md                UPDATE — fix "or when no algorithm is set" (null fails NotNull()),
│   │                                      clarify dependency map physical locations.
│   ├── kernel.md                 UPDATE — fix IApiMarker vs IModuleMarker, all three modules have IModuleMarker.
│   ├── presets.md                UPDATE — fix test preset section (code not file), add v3 route pattern,
│   │                                      add "Presets.json is required" note.
│   ├── configuration.md          UPDATE — add ServiceModule config files to tree, fix .{Environment}.json
│   │                                      override pattern, add BINACLE_ADMIN_CREDENTIALS,
│   │                                      add Scalar mount path to feature flags table.
│   ├── v3/
│   │   ├── README.md             RENAME/REWRITE from api/v3.md — fix "multiple bins per request",
│   │   │                                  add v3 field names, add enum values, note dead early-exit codes.
│   │   │                                  Add explicit: "do NOT add endpoints to v3."
│   │   └── contracts.md          NEW — v3-specific contracts, field names, response wrapper,
│   │                                    enum values for fit and pack.
│   ├── v4/
│   │   ├── README.md             RENAME from api/v4.md — fix "or leave it out" (null fails NotNull()).
│   │   ├── contracts.md          MOVE/UPDATE from api/contracts.md — fix null algorithm claim,
│   │   │                                  fix CustomBinsRequestBase/PresetBinsRequestBase "Used by",
│   │   │                                  add Unknown = -1 on status enums.
│   │   └── add-endpoint.md       MOVE/UPDATE from api/add-endpoint.md — fix RequireRateLimiting comment,
│   │                                      add 404 case for preset endpoints.
│   └── modules/
│       ├── README.md             MOVE/UPDATE from api/modules.md — add Scalar mount path (/scalar).
│       ├── diagnostics.md        RENAME from api/module-diagnostics.md — add all-files-have-env-variants note.
│       ├── service.md            RENAME/UPDATE from api/module-service.md — fix all routes (no /v0/ prefix,
│       │                                  singular account/subscription), fix Npgsql→Postgres connection
│       │                                  string name, fix config files table (three separate files).
│       └── ui.md                 RENAME/UPDATE from api/module-ui.md — fill out: pages, JS stack,
│                                          API connection, config file, status code page behaviour.
├── lib/
│   ├── README.md                 UPDATE — add Binacle.Lib.Benchmarks to Related Tests table,
│   │                                      fix "Custom exceptions" (only one: DimensionException).
│   ├── models.md                 UPDATE — fix PackedBin description, add ResultItem base class.
│   ├── algorithms.md             (no changes needed)
│   ├── algorithm-factory.md      (no changes needed)
│   ├── processors.md             UPDATE — fix two-axes table: many-bins/multi-algo returns
│   │                                      IMultiAlgorithmBinProcessor not IBinProcessor.
│   ├── result-building.md        (no changes needed)
│   └── result-selection.md       (no changes needed)
├── tests/
│   ├── README.md                 (no changes needed)
│   └── scenarios.md              UPDATE — fix "used by 2 projects" → 4 (add Benchmarks + PerformanceTests).
├── vipaq/
│   └── README.md                 (no changes needed)
├── packages/
│   └── README.md                 UPDATE — remove binacle-vipaq from packages table (already moved).
├── api/endpoints.md              UPDATE — fix IGroupedEndpoint<TGroup> ownership, add per-module IModuleMarker note.
│                                          (stays flat in api/ — shared across v3 and v4)
└── (docs/, web/, ruby/ — no changes needed)
```

### CLAUDE.md changes
- Add `@.agent-docs/_index.md` alongside the existing `@.agent-docs/README.md`.
- Update "Common Tasks" table links to reflect new paths
  (e.g. `api/contracts.md` → `api/v4/contracts.md`, `api/v3.md` → `api/v3/README.md`, etc.).
- Add "Critical Rules" block — **draft for user review before writing**.
  Present the proposed rules to the user and get sign-off before adding to CLAUDE.md.

### New content (additions, not just fixes)
- `_index.md` — flat manifest of every doc path + one-line description from frontmatter.
  Regenerable via `config/docs.sh` script.
- `api/README.md` — add v4 request flow trace (HTTP → endpoint → service → processor → algorithm →
  result builder → response mapper → ViPaq). Shows how layers connect end-to-end.
- Each doc — add `verified: YYYY-MM-DD` to frontmatter. **Draft for user review before writing.**
- `config/docs.sh` — script to regenerate `_index.md` from frontmatter descriptions.
- Each task doc — add "Done when:" verification line. **Draft for user review before writing.**
- Each doc — add "Also update:" side effect note. **Draft for user review before writing.**
- Each doc — add `status:` stability marker to frontmatter (frozen/stable/active/planned).
  **Draft for user review before writing.**

### Cross-link fixes (after rename/move)
All `../concepts/fit-vs-pack.md` references → `../concepts.md`.
All `contracts.md` → `api/v4/contracts.md` or `api/v3/contracts.md` as appropriate.
All `add-endpoint.md` → `api/v4/add-endpoint.md`.
All `module-*.md` → `api/modules/*.md`.
All `modules.md` → `api/modules/README.md`.

### Deferred code fixes (not doc work — address separately)
- `config/api.sh` — `U` alias maps to `WithServiceModuleOnly`, should be `WithUiModuleOnly`. One-line fix.
- `api/src/Binacle.Net.Kernel` — `LegacyBindingResult<T>` and `LegacyValidatedBindingResult<T>` are
  dead code. Remove them.
- `api/src/Binacle.Net/v3/Contracts/PackResponse.cs` — `EarlyFail_ContainerVolumeExceeded` and
  `EarlyFail_ContainerDimensionExceeded` on `BinPackResultStatus` are dead code (pack never early-exits).
  Remove or leave with a comment — confirm with user before touching.
