using Binacle.Net.Kernel.Endpoints;

namespace Binacle.Net.v3;

internal class ApiV3EndpointGroup : IEndpointGroup
{
	public RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapGroup($"/api/{ApiV3Document.DocumentName}")
			.WithGroupName(ApiV3Document.DocumentName)

			.ProducesProblem(StatusCodes.Status500InternalServerError)
			.ResponseDescription(
				StatusCodes.Status500InternalServerError,
				ResponseDescription.For500InternalServerError
			);
		
		// TODO: Example
		// .ResponseExample<Status500ResponseExample>(
		// 	StatusCodes.Status500InternalServerError,
		// 	"application/problem+json"
		// )
		// .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json")
		// .ResponseExample<ServerErrorResponseExample>(StatusCodes.Status500InternalServerError, "application/json")
		// .ResponseDescription(
		// 	StatusCodes.Status500InternalServerError,
		// 	ResponseDescription.For500InternalServerError
		// );
	}
}
