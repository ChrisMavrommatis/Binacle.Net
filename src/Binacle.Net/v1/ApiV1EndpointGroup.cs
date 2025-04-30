using Binacle.Net.Constants;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.v1.Responses;
using Binacle.Net.v1.Responses.Examples;
using OpenApiExamples;

namespace Binacle.Net.v1;

internal class ApiV1EndpointGroup : IEndpointGroup
{
	public RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapGroup($"/api/{ApiV1Document.DocumentName}")
			.WithGroupName(ApiV1Document.DocumentName)
			.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json")
			.ResponseExample<ServerErrorResponseExample>(StatusCodes.Status500InternalServerError, "application/json")
			.ResponseDescription(
				StatusCodes.Status500InternalServerError,
				ResponseDescription.For500InternalServerError
			);
	}
}
