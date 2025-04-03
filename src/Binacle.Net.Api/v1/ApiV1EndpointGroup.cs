using Binacle.Net.Api.Constants;
using Binacle.Net.Api.Kernel.Endpoints;
using Binacle.Net.Api.v1.Responses;

namespace Binacle.Net.Api.v1;

internal class ApiV1EndpointGroup : IEndpointGroup
{
	public RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapGroup($"/api/{ApiV1Document.DocumentName}")
			.WithGroupName(ApiV1Document.DocumentName)
			.AllEndpointsProduce<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json")
			.AllEndpointsHaveResponseDescription(
				StatusCodes.Status500InternalServerError,
				ResponseDescription.For500InternalServerError
			);
	}
}
