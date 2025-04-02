using Binacle.Net.Api.Kernel.Endpoints;

namespace Binacle.Net.Api.v2;

internal class ApiV2EndpointGroup : IEndpointGroup
{
	public RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapGroup($"/api/{ApiV2Document.DocumentName}")
			.WithGroupName(ApiV2Document.DocumentName);
	}
}
