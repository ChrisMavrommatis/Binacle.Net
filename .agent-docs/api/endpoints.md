---
description: Endpoint pattern, registration, and request validation flow
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
IEndpointGroup           — sets the route prefix and shared metadata
IGroupedEndpoint<TGroup> — one endpoint inside a group
IEndpoint                — one endpoint, not in a group
```

## Request Validation Flow

Requests go through `BindingResult<TRequest>`, which runs FluentValidation. The handler always starts with:

```csharp
return await bindingResult.ValidateAsync(async request => {
    // request is the validated, typed model
});
```

Validation failures return `422 UnprocessableEntity`.

## Contracts Location

- `src/Binacle.Net/v3/Contracts/` — request/response types, validators, examples
- `src/Binacle.Net/v4/Contracts/` — same structure for v4
