---
id: api/kernel
description: Binacle.Net.Kernel — shared patterns used by all API projects and modules
verified: 2026-08-13
check: IApiMarker and registration helpers match api/src/Binacle.Net.Kernel/
also_update:
  - api/endpoints
paths:
  - "api/src/Binacle.Net.Kernel/**"

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
See `$api/modules` for what each flag enables and how modules use them.

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
All three modules use this pattern: DiagnosticsModule (`$api/modules/diagnostics`), ServiceModule (`$api/modules/service`), UIModule (`$api/modules/ui`).

## IOpenApiDocument

Each module registers its own OpenAPI document by implementing `IOpenApiDocument`.
`Program.cs` scans for all registered documents and wires them into SwaggerUI / Scalar at startup.
The transformers, the group-level 500 wiring, and the external `OpenApiExamples` package are covered in
`$api/openapi`.

## IStartupTask

Post-build async initialization. Registered via `services.AddStartupTask<T>()`, run via `app.RunStartupTasksAsync()`.
Used by Infrastructure to create database schemas before the app starts serving requests — see ServiceModule (`$api/modules/service`).

## IConfigurationOptions

Base interface for strongly-typed config classes loaded from JSON files.
Provides: `FilePath`, `SectionName`, `Optional`, `ReloadOnChange`, and `GetEnvironmentFilePath(env)`.
Config is loaded relative to `Config_Files/` (set as base path in `Program.cs`).

Register a validated options class with `services.AddValidatableJsonConfigurationOptions<TOptions>()`
(`Configuration/ExtensionsMethods/ConfigurationExtensions.cs`): it adds the JSON file + env override + env vars,
binds the section, and runs FluentValidation at startup (`ValidateFluently().ValidateOnStart()`). Used in
`Program.cs` for `BinPresetOptions`, `CorsOptions`, and `ForwardedHeadersConfigurationOptions`, and by each
module's `ModuleDefinition`.

## Validation

`BindingResult<T>` handles **request-body** validation (above). The same FluentValidation machinery validates
**options** at startup and provides reusable helpers:

- `FluentValidationOptions<TOptions>` — an `IValidateOptions<TOptions>` that runs the registered
  `IValidator<TOptions>`; wired via the `ValidateFluently()` options-builder extension.
- `RuleBuilderValidationExtensions.MustNotThrow(...)` — a custom rule that passes unless the given action throws.
  Use it to assert "this construction/conversion succeeds" (e.g. a volume calc that could overflow).
- `ValidationExtensions.GetValidationSummary()` — groups a `ValidationResult` into `Dictionary<string, string[]>`,
  the shape fed to `HttpValidationProblemDetails` (the 422 body).

## Network

`Kernel/Network/IPEntry` reads an IP entry as an operator writes it in configuration: **a single address, or
CIDR notation**. Anywhere a module takes a list of addresses from a config file, it parses them through here, so
one spelling means one thing across the app.

```csharp
if (!IPEntry.TryParse(entry, out var network)) { /* refuse it */ }
var caller = IPEntry.Normalize(context.Connection.RemoteIpAddress);
```

The slash picks the form; nothing else is attempted. Two behaviours are worth knowing before you use it:

- **An entry must read as the host it admits.** `IPAddress.TryParse` still accepts the inet_aton forms -
  `010.10.10.10` is octal and lands on `8.10.10.10`, `0x0A.10.10.10` is hex, `10.1` and `167772161` both become
  `10.0.0.1` - and it drops an IPv6 scope id. `IPEntry` refuses anything that does not survive a round trip
  through `IPAddress.ToString()`, which is one rule instead of a table. IPv6 is held to the same rule, so
  `2001:0db8::1` must be written `2001:db8::1`.
- **Host bits are masked off**, as they are everywhere else in .NET: `192.168.1.1/24` is the whole
  `192.168.1.0/24`. The BCL does this silently and the docs claim it throws - it does not. **This is a caller
  obligation, not just a fact about this type:** a caller that hands the list to an operator has to say what
  each entry resolved to, because 256 addresses is not what the operator wrote.
  `HealthChecksProtectionMiddleware` is the one caller that honours it today.

`TryParse` never throws, including on an `AddressFamily` it does not know - it refuses instead. That is
deliberate: the input comes from a config file, and a config file must not be able to crash startup.

`Normalize` unmaps an IPv4-mapped IPv6 address, which a dual-mode socket produces for every IPv4 caller. The
entry side is normalised during parse, so an IPv4-mapped CIDR entry (`::ffff:192.168.1.0/120`) is refused rather
than parsed into something that matches no caller. That refusal exists because `IPNetwork.Contains` is not
symmetric: an unmapped network contains a mapped caller, but a mapped network does not contain an unmapped one,
so such an entry would match a container's caller and not a real IPv4 one. Carrying the prefix over instead
(taking 96 off it, so `/120` becomes `/24`) works and was measured, if a reason to accept the form ever appears.

Tested in `api/test/Binacle.Net.Kernel.UnitTests/Network/`.

## Logging

Timed operations (in `Kernel/Logging`):

- `logger.BeginTimedOperation("template", args)` → returns an `IDisposable`; on dispose it logs
  `"{template} completed in {OperationDurationMs}ms"`. Default level Information.
- `logger.BeginTimedActivityOperation("message")` → same, and also starts an `Activity` (tracing span) named
  `message` on `Binacle.Net.Diagnostics.ActivitySource`. One `using` gives both a timed log and a span.
- `logger.EnrichState(...)` — wraps a dictionary / string set into a `BeginScope` for structured-log enrichment.

## Logs (generic pipeline)

`Binacle.Net.Kernel/Logs/` holds a **generic, feature-agnostic** log pipeline. It has no packing types and no
reference to `Binacle.CompactNotation` — a feature plugs in its own request and entry types.

- `ILogEntryConvertible<TLogEntry>` (`Logs/Models`) — a channel request implements it:
  `TLogEntry ToLogEntry(DateTimeOffset timestamp)`.
- `LogsProcessor<TRequest, TLog>` (`Logs/Services`, `where TRequest : ILogEntryConvertible<TLog>`) — a generic
  `BackgroundService`. Drains a `Channel<TRequest>`, calls `request.ToLogEntry(timeProvider.GetUtcNow())`,
  JSON-serialises the entry, and appends one line to a dated file. Knows nothing about any feature's types.
- `LogsProcessorOptions<TChannelRequest>` (`Logs/Models`) — `Path` / `FileNameFormat` / `DateFormat` +
  `MaxConsecutiveAllowedExceptions` (default 10). The type param only keys the DI registration.
- `ILogParametersProvider` (`Logs/Models`) — `IReadOnlyList<string> ToLogParameters()`. A request's parameter type
  implements it so the background converter can project loose parameter strings without seeing the API's enums.
- `AddLogProcessor<TChannelRequest, TLog>(optionsFactory, channelFactory)` (`Logs/ExtensionMethods`, namespace
  `Binacle.Net`) — registers the channel, options, and hosted processor. The owning feature supplies the types + factories.

The concrete packing feature (the request/entry types and their registration) lives in DiagnosticsModule — see
`$api/modules/diagnostics`.

`BinacleService` is the live producer: it injects `IOptionalDependency<Channel<AlgorithmOperationLogChannelRequest>>`
and writes via `WriteToChannelAsync(...)`, which **no-ops when the channel isn't registered** (i.e. when the
DiagnosticsModule packing-log feature is off).
