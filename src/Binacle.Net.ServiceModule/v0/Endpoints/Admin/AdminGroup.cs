using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin;

internal class AdminGroup : IEndpointGroup
{
	public RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapGroup($"/api/admin")
			.WithTags("Admin")
			.RequireAuthorization("Admin")
			.WithGroupName(ServiceModuleApiDocument.DocumentName)
			.AllEndpointsProduce<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.AllEndpointsHaveResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.AllEndpointsProduce(StatusCodes.Status401Unauthorized)
			.AllEndpointsHaveResponseDescription(StatusCodes.Status401Unauthorized,
				ResponseDescription.For401Unauthorized)
			.AllEndpointsProduce(StatusCodes.Status403Forbidden)
			.AllEndpointsHaveResponseDescription(StatusCodes.Status403Forbidden, ResponseDescription.For403Forbidden);
	}
}
