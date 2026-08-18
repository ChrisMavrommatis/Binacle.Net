---
id: api/openapi
description: OpenAPI wiring — IOpenApiDocument, the Kernel transformers (JWT, 429, response descriptions, enum-as-string), what endpoint groups auto-wire, and the external OpenApiExamples package
verified: 2026-08-14
check: IOpenApiDocument, transformers, and OpenApiOptions extensions match api/src/Binacle.Net.Kernel/OpenApi/; group 500 wiring matches v4/ApiV4EndpointGroup.cs; RateLimiterResponseOperationTransformer checks the endpoint metadata only, and RateLimitedEndpointConvention in the ServiceModule is the only thing that attaches it
also_update:
  - api/v4/add-endpoint
  - api/kernel
paths:
  - "api/src/**/OpenApi/**"

---

# OpenAPI

How the OpenAPI documents are defined, transformed, discovered, and served. Most of this is automatic — know it
so you don't re-add things by hand.

## IOpenApiDocument

Each API version / module registers one document by implementing `IOpenApiDocument`
(`Binacle.Net.Kernel/OpenApi/IOpenApiDocument.cs`):

```csharp
string Name { get; }          // e.g. "v4" — also the {documentName} in the JSON route
string Title { get; }         // e.g. "Binacle.Net API v4"
string Version { get; }       // e.g. "4.0"
bool IsDeprecated { get; }
bool IsExperimental { get; }
void Configure(OpenApiOptions options);
```

Implementers: `ApiV3Document` (`v3`), `ApiV4Document` (`v4`), `ServiceModuleApiDocument`. Each has a
`const string DocumentName`. `Configure()` calls the transformers below.

## Kernel transformers (OpenApiOptions extensions)

These live in Kernel (`OpenApi/ExtensionsMethods/OpenApiOptionsExtensions.cs`) and are called from each
document's `Configure()`:

| Extension | Wires | Effect |
|---|---|---|
| `AddResponseDescription()` | `ResponseDescriptionOperationTransformer` | Applies the per-endpoint response description set via the `.ResponseDescription(status, text)` endpoint extension, formatted by `ResponseDescription.Format` |
| `AddJwtAuthentication()` | JWT document + operation transformers | Adds a `"Bearer"` security scheme and marks `[Authorize]` operations. No-ops when there's no auth scheme (ServiceModule off) — driven by `IOptionalDependency<IAuthenticationSchemeProvider>` |
| `AddRateLimiterResponse()` | `RateLimiterResponseOperationTransformer` | Adds a `429` response to operations with `[EnableRateLimiting]`, which only the ServiceModule attaches (see below) |
| `AddEnumStringsSchema()` | `EnumStringsSchemaTransformer` | Renders enum properties as `string` schemas (and fills enum names for the nullable converters) |

### The `429` follows the metadata, and the metadata follows the module

`RateLimiterResponseOperationTransformer` checks one thing: the operation carries `[EnableRateLimiting]`. That
is enough because **only `AddServiceModule` ever attaches it**, off the `.RateLimited()` marker the core
endpoint places. A build with the module off has no metadata to find, so the `429` cannot get into its document.

The core endpoints name no policy, so there is no inert `[EnableRateLimiting]` to mistake for a live limiter.

This matters most for the **generated** documents. `just openapi generate` builds them from a host with no launch
profile, so ServiceModule is off — that document already has no `v0` ServiceModule paths and no
`/api/auth/token`, and it must have no `429` either, or it describes a shape that exists nowhere. A live instance
running with the module on serves a document that does carry the `429`, correctly.

Helpers in Kernel: `ResponseDescription.Format`, `HttpStatusDescriptions` (int → status name map),
`OpenApiValidationProblemExample`, `ResponseDescriptionMetadata`, and the `.ResponseDescription(status, text)`
endpoint-builder extension (`OpenApiRouteHandlerBuilderExtensions`).

## What the endpoint group wires for you

`ApiV4EndpointGroup` (and `ApiV3EndpointGroup`) applies to **every** endpoint in the group:

```csharp
.ProducesProblem(500)
.ResponseDescription(500, ResponseDescription.For500InternalServerError)
.ResponseExample<Status500ResponseExample>(500, "application/problem+json")
```

So **never add `.ProducesProblem(500)` per endpoint** — it's already there. `ResponseDescription.For500…` is the
version-local constant in `v4/Constants.cs` (and `v3/Constants.cs`).

## Discovery and serving (Program.cs)

- `builder.Services.AddOpenApiDocumentsFromAssemblyContaining<IApiMarker>()` reflects over the assembly, finds the
  `IOpenApiDocument` implementations, and registers each via `AddOpenApi(doc.Name, doc.Configure)`.
- The JSON endpoint and UIs are gated on the `SWAGGER_UI` / `SCALAR_UI` features:
  `app.MapOpenApi("/openapi/{documentName}.json")`, then one Swagger endpoint and/or one Scalar document per
  registered `IOpenApiDocument`. With both off, there is **no** `/openapi/*.json` route.

## Kernel vs the external OpenApiExamples package — don't confuse them

`OpenApiExamples` is an **external NuGet package** (referenced in `Binacle.Net.csproj`), not Kernel. From it:

- Interfaces `ISingleOpenApiExamplesProvider<T>` / `IMultipleOpenApiExamplesProvider<T>` (request/response example classes implement these)
- `OpenApiExample.Create(...)`, `OpenApiOptions.AddExamples()`, `services.AddOpenApiExamples(...)`
- Endpoint-builder helpers `.RequestExample<T>(...)`, `.ResponseExample<T>(...)`, `.ResponseExamples<T>(...)`

From **Kernel**: `IOpenApiDocument`, the four `OpenApiOptions` extensions + their transformers,
`.ResponseDescription(...)` (endpoint metadata), `AddOpenApiDocumentsFromAssemblyContaining<T>`, and the helpers
listed above.

Watch the name collision: Kernel's `.ResponseDescription(status, text)` (sets description text) is **different**
from the package's `.ResponseExample` / `.ResponseExamples` (attach example payloads).
