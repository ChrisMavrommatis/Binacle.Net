---
description: Binacle.Net.Kernel — shared patterns used by all API projects and modules
---

# Kernel

`src/Binacle.Net.Kernel` is the foundation every other project references.
It provides no business logic — only patterns, infrastructure, and helpers.

## BindingResult\<T\>

Replaces the default model binding for endpoint handlers. It does two things in one step: deserialise the
JSON body and run FluentValidation. Handlers always receive a `BindingResult<T>` and call `ValidateAsync()`:

```csharp
internal async Task<IResult> HandleAsync(
    BindingResult<MyRequest> bindingResult, ...)
{
    return await bindingResult.ValidateAsync(async request => {
        // request is the validated, typed model
    });
}
```

What `ValidateAsync()` returns before calling your handler:

| Condition | Response |
|---|---|
| JSON parse error | `400` with `"Invalid JSON Format"` problem details |
| Null body | `400` with `"Malformed Request"` problem details |
| Validation failure | `422` with FluentValidation field errors |
| Other exception | `500` (exception details exposed in Development only) |

`BindingResult<T>` is registered automatically via `BindAsync()` — no DI setup needed.

## Endpoint Interfaces

| Interface | Used for |
|---|---|
| `IEndpointGroup` | Defines a route group prefix and shared metadata |
| `IGroupedEndpoint<TGroup>` | One endpoint inside a group (most common) |
| `IEndpoint` | One standalone endpoint, not in a group |

All are discovered and registered automatically via `RegisterEndpointsFromAssemblyContaining<TMarker>()`.

## IOptionalDependency\<T\>

Wraps a service that may not be registered (e.g., a channel from a module that might be off).

```csharp
public OptionalDependency(IServiceProvider serviceProvider) {
    Value = serviceProvider.GetService<T>(); // null if not registered
}
```

Registered as open generic in `Program.cs`:

```csharp
services.AddTransient(typeof(IOptionalDependency<>), typeof(OptionalDependency<>));
```

Used by `BinacleService` to optionally write to the packing log channel without failing if DiagnosticsModule
packing logs are disabled.

## Feature.Manager

Created **before the DI container** in `Program.cs`. Reads from `IConfiguration` and environment variables.

```csharp
Feature.Manager = new FeatureManagerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.EnvironmentVariables()
    .CreateManager();
```

After this point, `Feature.IsEnabled("FLAG_NAME")` is available anywhere.
Used in `Program.cs` to conditionally call `AddServiceModule()` and `AddUIModule()`.

Flags checked in `Program.cs`: `SERVICE_MODULE`, `UI_MODULE`, `SWAGGER_UI`, `SCALAR_UI`.

## IModuleMarker

Each module (and the core API) defines its own `IModuleMarker` interface.
It's used purely as an assembly anchor for scanning:

```csharp
services.AddValidatorsFromAssemblyContaining<IModuleMarker>(...);
services.AddOpenApiDocumentsFromAssemblyContaining<IModuleMarker>();
app.RegisterEndpointsFromAssemblyContaining<IModuleMarker>();
```

This keeps each module's validators, OpenAPI docs, and endpoints isolated to its own assembly.

## IOpenApiDocument

Each module registers its own OpenAPI document by implementing `IOpenApiDocument`.
`Program.cs` scans for all registered documents and wires them into SwaggerUI / Scalar at startup.

## IStartupTask

Post-build async initialization. Registered via `services.AddStartupTask<T>()`, run via `app.RunStartupTasksAsync()`.
Used by Infrastructure to create database schemas before the app starts serving requests.

## IConfigurationOptions

Base interface for strongly-typed config classes loaded from JSON files.
Provides: `FilePath`, `SectionName`, `Optional`, `ReloadOnChange`, and `GetEnvironmentFilePath(env)`.
Config is loaded relative to `Config_Files/` (set as base path in `Program.cs`).
