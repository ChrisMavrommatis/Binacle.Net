using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.v2.Responses;
using Binacle.Net.v2.Responses.Examples;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v2;

internal class ApiV2EndpointGroup : IEndpointGroup
{
	public RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapGroup($"/api/{ApiV2Document.DocumentName}")
			.WithGroupName(ApiV2Document.DocumentName)
			.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json")
			.ResponseExample<ServerErrorResponseExample>(StatusCodes.Status500InternalServerError, "application/json")
			.ResponseDescription(
				StatusCodes.Status500InternalServerError,
				ResponseDescription.For500InternalServerError
			);
	}
}
