---
description: Optional module system — feature flags, structure, available modules
---

# Modules

## Feature Flags

`Feature.Manager` reads from `appsettings.json` and env vars.
It is created once at startup, before the DI container is built.

Modules are turned on by flags in `Program.cs`:

```csharp
if (Feature.IsEnabled("SERVICE_MODULE")) builder.AddServiceModule();
if (Feature.IsEnabled("UI_MODULE"))      builder.AddUIModule();
```

UI flags (`SWAGGER_UI`, `SCALAR_UI`) work the same way.

See [configuration.md](../configuration.md) for the full feature-flag list, env-var conventions, and config file layout.

## Module Structure

Each module follows the same pattern:

```csharp
public static void AddXModule(this WebApplicationBuilder builder) { ... }
public static void UseXModule(this WebApplication app) { ... }
```

`AddXModule` handles: config, DI, validators, OpenAPI docs.
`UseXModule` wires up middleware and endpoints.

## Available Modules

<!-- sourced from docs site; verify against current code if behaviour changes -->

| Module | Env var to enable | Default | Adds |
|---|---|---|---|
| `DiagnosticsModule` | always on | always on | Logging, health checks, OpenTelemetry, packing logs |
| `ServiceModule` | `SERVICE_MODULE=True` | disabled | JWT auth, rate limiting, account management, subscriptions |
| `UIModule` | `UI_MODULE=True` | disabled | Razor/Blazor interactive packing demo |
| Swagger UI | `SWAGGER_UI=True` | disabled | Swagger UI at `/swagger` |
| Scalar UI | `SCALAR_UI=True` | disabled | Scalar UI at `/scalar` (alternative OpenAPI interface) |

See [configuration.md](../configuration.md) for full details on env-var conventions and config file layout.

## Launch Profiles

The `config/api.sh` script picks a launch profile:

- `Normal` — core API only
- `WithServiceModuleOnly`
- `WithUiModuleOnly`
- `WithAllModules`
