using Binacle.Net.Kernel.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;

namespace Binacle.Net.ServiceModule.Services;

// The policy name lives here and nowhere else. An endpoint says it is user compute with .RateLimited(); which
// limiter that means is this module's answer, and with the module off nobody answers.
internal sealed class RateLimitedEndpointConvention : IEndpointConvention
{
	public void Apply(EndpointBuilder endpointBuilder)
	{
		if (endpointBuilder.Metadata.OfType<RateLimitedMetadata>().Any())
		{
			endpointBuilder.Metadata.Add(new EnableRateLimitingAttribute("ApiUsage"));
		}
	}
}
