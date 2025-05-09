using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.v3.Contracts;
using OpenApiExamples;

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
			)
			.ResponseExample<Status500ResponseExample>(
				StatusCodes.Status500InternalServerError,
				"application/problem+json"
			);
	}
}
