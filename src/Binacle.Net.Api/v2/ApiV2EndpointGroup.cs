using Binacle.Net.Api.Constants;
using Binacle.Net.Api.Kernel.Endpoints;
using Binacle.Net.Api.v2.Responses;

namespace Binacle.Net.Api.v2;

internal class ApiV2EndpointGroup : IEndpointGroup
{
	public RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapGroup($"/api/{ApiV2Document.DocumentName}")
			.WithGroupName(ApiV2Document.DocumentName)
			.AllEndpointsProduce<ErrorResponse>(StatusCodes.Status500InternalServerError)
			.AllEndpointsHaveResponseDescription(
				StatusCodes.Status500InternalServerError,
				ResponseDescription.For500InternalServerError
			);
		
	}
}
