using Binacle.Net.Api.Kernel.Endpoints;
using Binacle.Net.Api.ServiceModule.Constants;
using Binacle.Net.Api.ServiceModule.Domain.Users.Entities;
using Binacle.Net.Api.ServiceModule.Models;
using Binacle.Net.Api.ServiceModule.v0.Responses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Binacle.Net.Api.ServiceModule.v0.Endpoints;

internal class UsersGroup : IEndpointGroup
{
	public RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapGroup($"/api/{UsersApiDocument.DocumentName}")
			.WithTags("Users (Admin only)")
			.RequireAuthorization(builder =>
			{
				builder.RequireAuthenticatedUser();
				builder.RequireClaim(JwtApplicationClaimNames.Groups, UserGroups.Admins);
			})
			.WithGroupName(UsersApiDocument.DocumentName)
			.AllEndpointsProduce<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.AllEndpointsHaveResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.AllEndpointsProduce(StatusCodes.Status401Unauthorized)
			.AllEndpointsHaveResponseDescription(StatusCodes.Status401Unauthorized, ResponseDescription.For401Unauthorized)
			.AllEndpointsProduce(StatusCodes.Status403Forbidden)
			.AllEndpointsHaveResponseDescription(StatusCodes.Status403Forbidden, ResponseDescription.For403Forbidden);
	}
}
