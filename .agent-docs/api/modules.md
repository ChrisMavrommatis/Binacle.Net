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

## Module Structure

Each module follows the same pattern:

```csharp
public static void AddXModule(this WebApplicationBuilder builder) { ... }
public static void UseXModule(this WebApplication app) { ... }
```

`AddXModule` handles: config, DI, validators, OpenAPI docs.
`UseXModule` wires up middleware and endpoints.

## Available Modules

| Module | Flag | Adds |
|---|---|---|
| `ServiceModule` | `SERVICE_MODULE` | JWT auth, rate limiting, account management, subscriptions |
| `UIModule` | `UI_MODULE` | Razor/Blazor interactive packing demo |
| `DiagnosticsModule` | always on | Diagnostics middleware and services |

## Launch Profiles

The `config/api.sh` script picks a launch profile:

- `Normal` — core API only
- `WithServiceModuleOnly`
- `WithUiModuleOnly`
- `WithAllModules`
