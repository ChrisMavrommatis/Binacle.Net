using Binacle.Net.Constants;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.v3.Responses;
using Binacle.Net.v3.Responses.Examples;
using OpenApiExamples;

namespace Binacle.Net.v3;

internal class ApiV3EndpointGroup : IEndpointGroup
{
	public RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapGroup($"/api/{ApiV3Document.DocumentName}")
			.WithGroupName(ApiV3Document.DocumentName)
			.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json")
			.ResponseExample<ServerErrorResponseExample>(StatusCodes.Status500InternalServerError, "application/json")
			.ResponseDescription(
				StatusCodes.Status500InternalServerError,
				ResponseDescription.For500InternalServerError
			);
	}
}
