using Microsoft.AspNetCore.Builder;

namespace Binacle.Net.Kernel.Endpoints;

// A module's hook into endpoints defined by another assembly. The registrar runs every registered convention
// inside one Finally, after each endpoint's own conventions - a convention that reads metadata is looking at a
// list that is still being filled until then, and finds nothing without a word of warning.
public interface IEndpointConvention
{
	void Apply(EndpointBuilder endpointBuilder);
}

// Says the endpoint is user compute, not which policy limits it. Only a module knows that, and only if one is
// registered at all.
public sealed record RateLimitedMetadata;
