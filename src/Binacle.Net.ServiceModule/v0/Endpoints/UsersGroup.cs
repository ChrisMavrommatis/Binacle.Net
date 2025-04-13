using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Users.Entities;
using Binacle.Net.ServiceModule.Constants;
using Binacle.Net.ServiceModule.Models;
using Binacle.Net.ServiceModule.v0.Responses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Binacle.Net.ServiceModule.v0.Endpoints;

internal class UsersGroup : IEndpointGroup
{
	public RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapGroup($"/api/{UsersApiDocument.DocumentName}")
			.WithTags("Users (Admin only)")
			.RequireAuthorization("Admin")
			.WithGroupName(UsersApiDocument.DocumentName)
			.AllEndpointsProduce<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.AllEndpointsHaveResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.AllEndpointsProduce(StatusCodes.Status401Unauthorized)
			.AllEndpointsHaveResponseDescription(StatusCodes.Status401Unauthorized, ResponseDescription.For401Unauthorized)
			.AllEndpointsProduce(StatusCodes.Status403Forbidden)
			.AllEndpointsHaveResponseDescription(StatusCodes.Status403Forbidden, ResponseDescription.For403Forbidden);
	}
}
