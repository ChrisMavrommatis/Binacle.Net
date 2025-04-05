using Binacle.Net.Api.Constants;
using Binacle.Net.Api.Kernel.Endpoints;
using Binacle.Net.Api.v3.Responses;
using Binacle.Net.Api.v3.Responses.Examples;
using OpenApiExamples;

namespace Binacle.Net.Api.v3;

internal class ApiV3EndpointGroup : IEndpointGroup
{
	public RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapGroup($"/api/{ApiV3Document.DocumentName}")
			.WithGroupName(ApiV3Document.DocumentName)
			.AllEndpointsProduce<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json")
			.AllEndpointsHaveResponseExample<ServerErrorResponseExample>(StatusCodes.Status500InternalServerError, "application/json")
			.AllEndpointsHaveResponseDescription(
				StatusCodes.Status500InternalServerError,
				ResponseDescription.For500InternalServerError
			);
	}
}
