using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin;

internal class AdminGroup : IEndpointGroup
{
	public RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapGroup($"/api/admin")
				.WithTags("Admin")
				.RequireAuthorization("Admin")
				.WithGroupName(ServiceModuleApiDocument.DocumentName)
				
				.Produces(StatusCodes.Status401Unauthorized)
				.ResponseDescription(
					StatusCodes.Status401Unauthorized,
					"The provided Bearer token is invalid."
				)
				
				.Produces(StatusCodes.Status403Forbidden)
				.ResponseDescription(
					StatusCodes.Status403Forbidden,
					"The provided Bearer token does not have permission."
				)
				
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
