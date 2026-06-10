---
description: Binacle.Net.Kernel — shared patterns used by all API projects and modules
verified: 2026-06-10
check: IApiMarker and registration helpers match api/src/Binacle.Net.Kernel/
also_update:
  - api/endpoints.md
---

# Kernel

`api/src/Binacle.Net.Kernel` is the foundation every other project references.
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

Defined in the Kernel project at `Dependencies/Services/IOptionalDependency.cs`, but its namespace is
**`Binacle.Net.Services`** (not `Binacle.Net.Kernel.*`) — that is the `using` you need. The interface and
`OptionalDependency<T>` class are both `public`.

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
See [modules/README.md](modules/README.md) for what each flag enables and how modules use them.

## IModuleMarker / IApiMarker

Each module defines its own `IModuleMarker` — DiagnosticsModule, ServiceModule, and UIModule each have one.
The core API (`Binacle.Net`) uses `IApiMarker` instead (a separate interface, same pattern).
Neither interface has members — they exist only as assembly anchors for scanning:

```csharp
services.AddValidatorsFromAssemblyContaining<IModuleMarker>(...);
services.AddOpenApiDocumentsFromAssemblyContaining<IModuleMarker>();
app.RegisterEndpointsFromAssemblyContaining<IModuleMarker>();
```

This keeps each module's validators, OpenAPI docs, and endpoints isolated to its own assembly.
All three modules use this pattern: [DiagnosticsModule](modules/diagnostics.md), [ServiceModule](modules/service.md), [UIModule](modules/ui.md).

## IOpenApiDocument

Each module registers its own OpenAPI document by implementing `IOpenApiDocument`.
`Program.cs` scans for all registered documents and wires them into SwaggerUI / Scalar at startup.
The transformers, the group-level 500 wiring, and the external `OpenApiExamples` package are covered in
[openapi.md](openapi.md).

## IStartupTask

Post-build async initialization. Registered via `services.AddStartupTask<T>()`, run via `app.RunStartupTasksAsync()`.
Used by Infrastructure to create database schemas before the app starts serving requests — see [ServiceModule](modules/service.md).

## IConfigurationOptions

Base interface for strongly-typed config classes loaded from JSON files.
Provides: `FilePath`, `SectionName`, `Optional`, `ReloadOnChange`, and `GetEnvironmentFilePath(env)`.
Config is loaded relative to `Config_Files/` (set as base path in `Program.cs`).

Register a validated options class with `services.AddValidatableJsonConfigurationOptions<TOptions>()`
(`Configuration/ExtensionsMethods/ConfigurationExtensions.cs`): it adds the JSON file + env override + env vars,
binds the section, and runs FluentValidation at startup (`ValidateFluently().ValidateOnStart()`). Used in
`Program.cs` for `BinPresetOptions` and `CorsOptions`, and by each module's `ModuleDefinition`.

## Validation

`BindingResult<T>` handles **request-body** validation (above). The same FluentValidation machinery validates
**options** at startup and provides reusable helpers:

- `FluentValidationOptions<TOptions>` — an `IValidateOptions<TOptions>` that runs the registered
  `IValidator<TOptions>`; wired via the `ValidateFluently()` options-builder extension.
- `RuleBuilderValidationExtensions.MustNotThrow(...)` — a custom rule that passes unless the given action throws.
  Use it to assert "this construction/conversion succeeds" (e.g. a volume calc that could overflow).
- `ValidationExtensions.GetValidationSummary()` — groups a `ValidationResult` into `Dictionary<string, string[]>`,
  the shape fed to `HttpValidationProblemDetails` (the 422 body).

## Logging

Timed operations (in `Kernel/Logging`):

- `logger.BeginTimedOperation("template", args)` → returns an `IDisposable`; on dispose it logs
  `"{template} completed in {OperationDurationMs}ms"`. Default level Information.
- `logger.BeginTimedActivityOperation("message")` → same, and also starts an `Activity` (tracing span) named
  `message` on `Binacle.Net.Diagnostics.ActivitySource`. One `using` gives both a timed log and a span.
- `logger.EnrichState(...)` — wraps a dictionary / string set into a `BeginScope` for structured-log enrichment.

Packing-log channel (producer side; the consumer/`LogsProcessor` is in [modules/diagnostics.md](modules/diagnostics.md)):

- `AlgorithmOperationLogChannelRequest` is the channel message — built via its static `From<TBin,TItem,TParams>(...)`
  (`TParams : ILogConvertible`). `ILogConvertible.ConvertToLogObject()` lets parameters render themselves for logs.
- `BinacleService` is the live producer: it injects `IOptionalDependency<Channel<AlgorithmOperationLogChannelRequest>>`
  and writes via `WriteToChannelAsync(...)`, which **no-ops when the channel isn't registered** (i.e. when the
  DiagnosticsModule packing-log feature is off). Register the processor with `AddLogProcessor<TChannelRequest>(...)`.
