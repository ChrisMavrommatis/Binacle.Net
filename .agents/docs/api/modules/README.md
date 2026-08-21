---
id: api/modules
description: Optional module system — feature flags, structure, available modules
verified: 2026-08-21
check: The Add/Use pair and IModuleMarker of each Binacle.Net.*Module* project exist where stated; the flag reading rules match Kernel/Features/ (both providers, FeatureManager, FeatureManagerConfiguration); the launch profiles match api/src/Binacle.Net/Properties/launchSettings.json
also_update:
  - api
  - api/configuration
paths:
  - "api/src/Binacle.Net.*Module*/**"

---

# Modules

## Feature Flags

`Feature.Manager` is created once at startup, before the DI container is built. Modules are turned on by flags
in `Program.cs`:

```csharp
if (Feature.IsEnabled("SERVICE_MODULE")) builder.AddServiceModule();
if (Feature.IsEnabled("UI_MODULE"))      builder.AddUIModule();
```

UI flags (`SWAGGER_UI`, `SCALAR_UI`) work the same way. See `$api/configuration` for the full flag list,
env-var conventions, and config file layout.

Three rules decide what a flag reads as, and each one bites:

- **A flag is spelled `True` or `False`, capitalised.** Both providers compare against `bool.TrueString` and
  `bool.FalseString` as strings. `SERVICE_MODULE=true` is neither: it reads as *not found*, falls through to the
  next source, and ends at the default. No warning is logged.
- **Two sources, and the first one that answers wins.** `ReadFrom.Configuration(...)` is registered before
  `ReadFrom.EnvironmentVariables()`, so a `Features` section in a config file beats the environment variable of
  the same name. Nothing else in the app works this way (`$api/configuration`) — this is the one inversion.
  The two do not collide by accident: the config provider reads the **`Features` section**, so it takes
  `"Features": { "SERVICE_MODULE": true }`, while the environment provider reads the bare variable name.
- **Not found means off.** `DefaultNotFoundBehavior` is `Disabled`, and before `Feature.Manager` is assigned
  every flag reads false, so a flag checked too early is silently off rather than a crash.

`CreateManager()` throws if called twice.

## Module Structure

Each module follows the same pattern:

```csharp
public static void AddXModule(this WebApplicationBuilder builder) { ... }
public static void UseXModule(this WebApplication app) { ... }
```

`AddXModule` handles: config, DI, validators, OpenAPI docs.
`UseXModule` wires up middleware and endpoints.

If you add a new module, create its own `IModuleMarker` in that module's assembly — it is the marker used for
endpoint registration. All three put it in `Properties/IModuleMarker.cs`; ServiceModule's is `internal`, the
other two are `public`.

## Available Modules

Three, and only two of them are optional.

| Module | Env var to enable | Default | Adds |
|---|---|---|---|
| `DiagnosticsModule` | none — always compiled in and always added | on | Logging, health checks, OpenTelemetry, packing logs |
| `ServiceModule` | `SERVICE_MODULE=True` | disabled | JWT auth, rate limiting, account management, subscriptions |
| `UIModule` | `UI_MODULE=True` | disabled | Razor Pages demo host — packing demo and ViPaq decoder |

`SWAGGER_UI`, `SCALAR_UI` and `DEBUG_ENDPOINT` are flags over features inside the core API and the
DiagnosticsModule, not modules of their own. `$api/configuration` lists all five in one table.

## Reserved paths

`ReservedPathOptions` (`Binacle.Net.Kernel`) is a set of path prefixes that must never answer with a web page.
**Whoever maps a path declares it**, because some are configurable and only the owning module knows where one
ended up.

| Declared in | Prefixes |
|---|---|
| `Program.cs` | `/api`, `/openapi`, `/swagger`, `/scalar` |
| `DiagnosticsModule` | `/_debug`, and the health path from `HealthCheckConfigurationOptions` |
| `UIModule` | `/_content` |

The core four are declared unconditionally, so they hold with Swagger and Scalar switched off. The health path
is bound through `AddOptions<ReservedPathOptions>().Configure<IOptions<HealthCheckConfigurationOptions>>(...)`
so it resolves lazily and follows the config file.

**The only reader is the UIModule** (`$api/modules/ui`), which uses it to decide whether a bare status becomes
an error page. A running instance lists what it reserved: `ReservedPaths` in the `/_health` payload, and
`[reservedPaths]` in `/_debug`.

**Map a path outside the UIModule and forget to declare it, and its 404s start rendering as HTML.**

## Launch Profiles

`just serve api [N|S|U|All]` picks a launch profile:

- `Normal` — core API only
- `WithServiceModuleOnly`
- `WithUiModuleOnly`
- `WithAllModules`
