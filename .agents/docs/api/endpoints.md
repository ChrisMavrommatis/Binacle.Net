---
id: api/endpoints
description: Endpoint pattern, registration, request validation flow, and route groups for v3 and v4
verified: 2026-08-14
check: IGroupedEndpoint hierarchy matches api/src/Binacle.Net.Kernel/Endpoints/
also_update:
  - api/kernel
paths:
  - "api/src/Binacle.Net/v3/**"
  - "api/src/Binacle.Net/v4/**"

---

# Endpoints

## Registration

Endpoints are found and registered automatically — no manual wiring needed.

```csharp
app.RegisterEndpointsFromAssemblyContaining<IApiMarker>();
```

Same pattern for each module:

```csharp
app.RegisterEndpointsFromAssemblyContaining<IModuleMarker>();
```

## Interfaces (in `Binacle.Net.Kernel`)

```
IEndpointGroup           — defines a route group prefix and shared metadata
IGroupedEndpoint         — non-generic base; declares DefineEndpoint(RouteGroupBuilder)
IGroupedEndpoint<TGroup> — adds the group type constraint; use this in your endpoint class
IEndpoint                — standalone endpoint, not part of a group
```

`RegisterEndpointsFromAssemblyContaining<T>` scans the assembly for all three and wires them up.

### IModuleMarker

Each module (DiagnosticsModule, ServiceModule, UIModule) defines its own `IModuleMarker` inside that module's assembly.
It has no members — just a marker for `RegisterEndpointsFromAssemblyContaining<T>`.
If you add a new module, create your own `IModuleMarker` in that module's project.

## Route Groups

| Group class | Route prefix | Used by |
|---|---|---|
| `ApiV3EndpointGroup` | `/api/v3` | All v3 endpoints |
| `ApiV4EndpointGroup` | `/api/v4` | All v4 endpoints |

`ApiV4EndpointGroup` sets shared metadata for every v4 endpoint:
- `ProducesProblem(500)` — do **not** add this per-endpoint
- 500 `ResponseDescription` and `ResponseExample`

## Endpoint Anatomy

A typical v4 endpoint:

```csharp
internal class MyEndpoint : IGroupedEndpoint<ApiV4EndpointGroup>
{
    public void DefineEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("my-route", HandleAsync)
            .WithTags("MyTag")
            .WithSummary("...")
            // ... OpenAPI + produces declarations
    }

    internal async Task<IResult> HandleAsync(
        BindingResult<MyRequest> bindingResult,
        IBinacleService binacleService,
        ILogger<MyEndpoint> logger,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = Diagnostics.ActivitySource.StartActivity("My Operation: v4");

        return await bindingResult.ValidateAsync(async request =>
        {
            // call binacleService, return Results.Ok(...)
        });
    }
}
```

Tags group endpoints in OpenAPI. Current tags: `Fit`, `Pack`, `Presets`.

## Request Validation Flow

Always take `BindingResult<TRequest>` in the handler — never bind the request body directly.
It handles JSON binding and FluentValidation:

| Failure | HTTP response |
|---|---|
| Malformed JSON | `400 Bad Request` |
| Null body | `400 Bad Request` |
| Validation failure | `422 UnprocessableEntity` |

The handler only runs if binding and validation both pass.

## Rate Limiting

`.RateLimited()` marks an endpoint as user compute. It names no policy — it drops a marker in the endpoint's
metadata, and the ServiceModule turns that into `ApiUsage` when it is loaded. With the module off nothing reads
the marker and no limiter exists. `.RequireCors(CorsPolicy.CoreApi)` is likewise a no-op when the module is not
loaded.

## Contracts Location

- `api/src/Binacle.Net/v4/Contracts/` — request/response types, validators, OpenAPI examples
- `api/src/Binacle.Net/v3/Contracts/` — same structure for v3

See `$api/v4/contracts` for the full contract shape and `$api/v4/add-endpoint` for a step-by-step guide.
