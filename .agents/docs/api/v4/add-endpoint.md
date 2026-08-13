---
id: api/v4/add-endpoint
description: Step-by-step guide for adding a new v4 endpoint
verified: 2026-08-13
check: Code template matches a real v4 endpoint file and compiles
also_update:
  - api/v4
  - api/v4/contracts
paths:
  - "api/src/Binacle.Net/v4/**"

---

# How to Add an Endpoint

No manual registration needed — the app finds endpoints automatically.

New endpoints go in **v4 only**. v3 is stable and must not be modified.

## Steps

### 1. Create contract types (if new)

Add request/response types under `api/src/Binacle.Net/v4/Contracts/`.

Request types should use the relevant `IWith*` interfaces (`IWithBin`, `IWithBins`, `IWithItems`, `IWithOperationParameters`).
See `$api/v4/contracts` for the full interface table and a concrete example request class.
Add a FluentValidation validator in the same file.
Add OpenAPI examples next to the contract types — see `$api/v4/contracts` for the pattern.
Example classes use `RequestExample<T>`, `ResponseExamples<T>` from `OpenApiExamples.ExtensionMethods`.

### 2. Create the endpoint class

Create a file under `api/src/Binacle.Net/v4/Endpoints/<Tag>/`.

```csharp
internal class MyEndpoint : IGroupedEndpoint<ApiV4EndpointGroup>
{
    public void DefineEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("my-route", HandleAsync)
            .WithTags("MyTag")
            .WithSummary("...")
            .WithDescription("...")

            .Accepts<MyRequest>("application/json")
            .RequestExample<MyRequestExample>("application/json")

            .Produces<MyResponse>(StatusCodes.Status200OK, "application/json")
            .ResponseDescription(StatusCodes.Status200OK, "...")
            .ResponseExamples<MyResponseExamples>(StatusCodes.Status200OK, "application/json")

            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
            .ResponseExamples<Status400ResponseExamples>(StatusCodes.Status400BadRequest, "application/problem+json")

            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ResponseDescription(StatusCodes.Status422UnprocessableEntity, ResponseDescription.For400BadRequest)
            .ResponseExamples<MyValidationProblemResponseExamples>(StatusCodes.Status422UnprocessableEntity, "application/problem+json")

            .RequireRateLimiting("ApiUsage")   // on user-request (fit/pack) endpoints — no-op if ServiceModule is off
            .RequireCors(CorsPolicy.CoreApi);   // include where CORS protection is needed — no-op if ServiceModule is off
            // do NOT add .ProducesProblem(500) — ApiV4EndpointGroup sets it for all endpoints (see openapi.md)
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
            // call binacleService here
            return Results.Ok(...);
        });
    }
}
```

> **Rate limiting:** add `.RequireRateLimiting("ApiUsage")` to endpoints that handle user compute requests
> (all `fit` and `pack` routes, including their preset variants). Read-only list endpoints do **not** get it —
> e.g. the live `GET /api/v4/presets` is not rate-limited. It's safe to include unconditionally where it does
> belong: it's a no-op when ServiceModule is off. The `429` follows the same rule — it appears in the OpenAPI
> document only when the endpoint has the metadata **and** the ServiceModule is on to supply the limiter, so a
> document generated without the module carries no `429` at all.

> **CORS:** add `.RequireCors(CorsPolicy.CoreApi)` where CORS protection is needed. Check existing endpoints in the
> same group for the expected pattern before deciding.

### 3. Create the response type (if new)

Subclass `BinResponseBase` and add a static `From()` factory:

```csharp
public class MyResponse : BinResponseBase
{
    public MyStatus Status { get; set; }

    internal static MyResponse From(OperationParameters parameters, OperationResult result)
    {
        var response = From<MyResponse>(parameters, result);
        response.Status = ...; // map from result
        return response;
    }
}
```

`BinResponseBase.From<T>()` populates the common fields (Bin, AlgorithmUsed, PackedItems, UnpackedItems,
volume percentages, ViPaqData). Your subclass only needs to set what's specific to the operation.

Response types live in `api/src/Binacle.Net/v4/Contracts/`. See `$api/v4/contracts` for existing types.

### 4. Done

The endpoint is auto-registered. No changes to `Program.cs` or any registration file needed.

## Preset Endpoints — 404 Case

If your endpoint takes `{preset}` or `{bin}` route params, you must handle the not-found case.
The preset or bin may not exist. Return `Results.NotFound()` and declare it in `DefineEndpoint`:

```csharp
.Produces(StatusCodes.Status404NotFound)
.ResponseDescription(StatusCodes.Status404NotFound, "Preset or bin not found.")
```

See `PresetBin.cs` in the existing v4 endpoints for a working example.

## Choosing the Service Method

See `$api/service` for the full method reference and call pattern.
Quick reference: single bin → `SingleBinAsync`, multiple bins → `MultipleBinsAsync`, smallest bin → `SmallestBinAsync`.
Each has an explicit-algorithm overload and an auto-select overload.

To understand how the service runs algorithms and picks results, see `$lib/processors`
and `$lib/result-selection`.
