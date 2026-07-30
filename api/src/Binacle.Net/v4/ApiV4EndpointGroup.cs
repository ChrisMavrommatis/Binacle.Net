using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.v4.Contracts;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v4;

internal class ApiV4EndpointGroup : IEndpointGroup
{
	public RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapGroup($"/api/{ApiV4Document.DocumentName}")
			.WithGroupName(ApiV4Document.DocumentName)

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
