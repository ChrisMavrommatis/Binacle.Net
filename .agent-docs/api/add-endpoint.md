---
description: Step-by-step guide for adding a new v4 endpoint
---

# How to Add an Endpoint

No manual registration needed — the app finds endpoints automatically.

New endpoints go in **v4 only**. v3 is stable and must not be modified.

## Steps

### 1. Create contract types (if new)

Add request/response types under `src/Binacle.Net/v4/Contracts/`.

Request types should use the relevant `IWith*` interfaces (`IWithBin`, `IWithBins`, `IWithItems`, `IWithOperationParameters`).
Add a FluentValidation validator in the same file.
Add OpenAPI examples next to the contract types.

### 2. Create the endpoint class

Create a file under `src/Binacle.Net/v4/Endpoints/<Tag>/`.

```csharp
internal class MyEndpoint : IGroupedEndpoint<ApiV4EndpointGroup>
{
    public void DefineEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("my-route", HandleAsync)
            .WithTags("MyTag")
            .WithSummary("...")
            .Accepts<MyRequest>("application/json")
            .Produces<MyResponse>(StatusCodes.Status200OK, "application/json")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .RequireRateLimiting("ApiUsage")
            .RequireCors(CorsPolicy.CoreApi);
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

### 3. Done

The endpoint is auto-registered. No changes to `Program.cs` or any registration file needed.

## Choosing the Service Method

| Scenario | Service method |
|---|---|
| Single bin, specific algorithm | `SingleBinAsync(algorithm, bin, items, parameters)` |
| Single bin, auto-select best | `SingleBinAsync(bin, items, parameters)` |
| Smallest fitting bin, specific algorithm | `SmallestBinAsync(algorithm, bins, items, parameters)` |
| Smallest fitting bin, auto-select best | `SmallestBinAsync(bins, items, parameters)` |
