using Microsoft.AspNetCore.Builder;

namespace Binacle.Net.Api.Users.EndpointDefinitions;

internal interface IEndpointDefinition
{
	void DefineEndpoints(WebApplication app);
}
