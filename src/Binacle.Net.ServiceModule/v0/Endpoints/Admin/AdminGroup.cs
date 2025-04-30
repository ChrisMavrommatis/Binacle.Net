using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin;

internal class AdminGroup : IEndpointGroup
{
	public RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapGroup($"/api/admin")
				.WithTags("Admin")
				.RequireAuthorization("Admin")
				.WithGroupName(ServiceModuleApiDocument.DocumentName)
				
				.ProducesProblem(StatusCodes.Status400BadRequest)
				.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
				.ResponseExamples<Status400ResponseExamples>(
					StatusCodes.Status400BadRequest,
					"application/problem+json"
				)
				
				.Produces(StatusCodes.Status401Unauthorized)
				.ResponseDescription(
					StatusCodes.Status401Unauthorized,
					"When provided user token is invalid."
				)
				
				.Produces(StatusCodes.Status403Forbidden)
				.ResponseDescription(
					StatusCodes.Status403Forbidden,
					"When provided user token does not have permission."
				)
				
				.ProducesProblem(StatusCodes.Status500InternalServerError)
				.ResponseDescription(
					StatusCodes.Status500InternalServerError,
					ResponseDescription.For500InternalServerError
				)
				.ResponseExamples<Status500ResponseExamples>(
					StatusCodes.Status500InternalServerError,
					"application/problem+json"
				);
	}
}
