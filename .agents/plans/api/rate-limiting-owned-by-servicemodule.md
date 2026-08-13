# Move rate limiting out of the core endpoints and into the ServiceModule

**Status:** Investigated and proven feasible on 2026-08-13. Nothing built. The mechanism below was verified in a
throwaway ASP.NET Core 10 project, not in this repo.

## The problem

`.RequireRateLimiting("ApiUsage")` is written into 18 core endpoint files - every `fit` and `pack` route in
`api/src/Binacle.Net/v3/Endpoints/` and `api/src/Binacle.Net/v4/Endpoints/`. The limiter those endpoints name is
registered by `AddServiceModule` (`api/src/Binacle.Net.ServiceModule/ModuleDefinition.cs`), so the core API
names a policy that only the module supplies, and the call is a no-op whenever the module is off.

That inversion is the whole cost. Rate limiting is the module's job, but the core carries the knowledge of it,
which shows up in three places:

- **The core knows the module's policy name.** `"ApiUsage"` is a ServiceModule string sitting in v3/v4 code.
- **The OpenAPI transformer needs two guards to stay honest.**
  `api/src/Binacle.Net.Kernel/OpenApi/Transformers/RateLimiterResponseOperationTransformer.cs` documents `429`
  only when the `"RateLimiter"` feature is on **and** the operation carries `[EnableRateLimiting]`. The second
  guard alone is not enough precisely because the metadata is present with the module off. That guard was
  deleted once, on the reasoning that the metadata was sufficient, and the published v3.0.0 beta specs shipped
  a `429` no module-off build can emit.
- **Every new fit/pack endpoint has to remember an unrelated module's line.** The add-endpoint guide has to
  explain a no-op.

## The mechanism, verified

A module can attach rate limiting to endpoints defined in another assembly, off a policy-neutral marker the
endpoint author places. Three parts:

**1. Kernel owns a marker with no policy name in it.**

```csharp
public sealed record RateLimitedMetadata;

public static TBuilder RateLimited<TBuilder>(this TBuilder builder)
    where TBuilder : IEndpointConventionBuilder
{
    builder.WithMetadata(new RateLimitedMetadata());
    return builder;
}
```

Core endpoints call `.RateLimited()` where they call `.RequireRateLimiting("ApiUsage")` today. They declare
*that they are user compute*, not *which policy applies* - the module can't know which endpoints deserve
limiting, so something in the core has to say, but it does not have to name the policy. One marker is enough
while there is one core tier; give it an argument when a second appears.

**2. The ServiceModule translates the marker into the policy.**

```csharp
((IEndpointConventionBuilder)group).Finally(endpointBuilder =>
{
    if (endpointBuilder.Metadata.OfType<RateLimitedMetadata>().Any())
    {
        endpointBuilder.Metadata.Add(new EnableRateLimitingAttribute("ApiUsage"));
    }
});
```

**`Finally`, not `Add` - this is the trap, and it fails silently.** Group conventions registered with `Add` run
*before* each endpoint's own conventions, so the marker is not in `endpointBuilder.Metadata` yet and the check
finds nothing. No error, no warning, just no rate limiting. `Finally` runs after every convention. This was the
first thing the proof of concept got wrong.

**3. It reaches the OpenAPI document.** Metadata added in a `Finally` convention does land in
`ActionDescriptor.EndpointMetadata`, which is what the transformer reads. Confirmed by resolving
`IApiDescriptionGroupCollectionProvider` after `StartAsync()`: the marked endpoint reported policy `ApiUsage`,
the unmarked one reported none. Worth re-confirming inside this repo early, because the whole payoff below
rests on it.

## The payoff

**The two guards collapse into one, structurally.** If only the ServiceModule can ever add
`EnableRateLimitingAttribute`, then the presence of that metadata *means* the module is on, and the feature-flag
check in the transformer becomes redundant rather than load-bearing. The `429` cannot get into a module-off
document even if a later session deletes a guard, because there is no guard left to delete. That is worth more
than the tidiness: the guard has already been removed once by someone who reasoned about it and got it wrong.

Delete the feature check as the last step, not the first, and only after part 3 is confirmed in this repo.

## Wiring problem to solve first

`Program.cs` calls `app.UseServiceModule()` **before**
`app.RegisterEndpointsFromAssemblyContaining<IApiMarker>()`, so the module runs before the core endpoints and
their groups exist. The module cannot reach out and decorate them; the convention has to be waiting when the
core registers.

The seam is `RegisterEndpointsFromAssemblyContaining` in
`api/src/Binacle.Net.Kernel/Endpoints/ExtensionMethods/EndpointsWebApplicationExtensions.cs`. It already
resolves types out of a scoped provider, so it can also resolve registered conventions and apply each to every
group it builds in `RegisterEndpointsWithGroups`. `AddServiceModule` registers its convention at build time,
which is well before any of this runs.

Shape to settle when building it - do not treat this as decided:

- A Kernel interface (`IEndpointGroupConvention` or similar) resolved as `IEnumerable<>`, versus the
  `IOptionalDependency<T>` pattern already used for `IAuthenticationSchemeProvider` in the OpenAPI JWT wiring.
- Whether conventions apply to groups only, or to ungrouped `IEndpoint` registrations too. Only grouped
  endpoints need it today.

## Scope

- Kernel: the marker, the extension, the convention seam in the endpoint registrar.
- ServiceModule: register the convention in `AddServiceModule`.
- v3/v4: swap 18 `.RequireRateLimiting("ApiUsage")` calls for `.RateLimited()`.
- Kernel OpenAPI: delete the feature-flag guard, last.

**v3 is in scope and this does not break the freeze.** The 12 v3 and v4 endpoint files change, but the swap is
behaviour-preserving by construction - the same `EnableRateLimitingAttribute` ends up on the same endpoints, and
a caller cannot tell the difference. Prove it rather than assume it: compare the generated v3 document before
and after with the module off (both should have no `429`), and confirm a module-on instance still returns `429`.

`.RequireRateLimiting("AuthToken")` on `api/src/Binacle.Net.ServiceModule/v0/Endpoints/Auth/Token.cs` stays as
it is. That endpoint belongs to the module, so naming the module's own policy is correct there.

## Test gap this walks into

There is no test anywhere that asserts a `429` ever happens - the integration-test plan says so, and it is still
true. This refactor changes *how* the attribute arrives on the endpoint, with no test that would notice if it
stopped arriving. Either land the rate-limiting integration test first, or add a narrow assertion alongside this
work: with the module on, a marked endpoint carries `EnableRateLimitingAttribute` and an unmarked one does not.
Doing it with neither is how this ships broken and quiet.

## Timing

Not a v3.0.0 item - the guard is back in the working tree and the beta problem is fixed without this. It is the
durable fix for the same bug. It also overlaps the ServiceModule simplification idea, which would move this code
anyway, so check that direction has been decided before starting.
