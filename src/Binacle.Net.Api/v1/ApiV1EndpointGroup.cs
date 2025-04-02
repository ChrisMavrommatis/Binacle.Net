using Binacle.Net.Api.Kernel.Endpoints;

namespace Binacle.Net.Api.v1;

internal class ApiV1EndpointGroup : IEndpointGroup
{
	public RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapGroup($"/api/{ApiV1Document.DocumentName}")
			.WithGroupName(ApiV1Document.DocumentName);
	}
}
