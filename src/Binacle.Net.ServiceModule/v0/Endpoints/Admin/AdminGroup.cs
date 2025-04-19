﻿using Binacle.Net.Kernel.Endpoints;
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
			.WithGroupName("Admin");
		// .AllEndpointsProduce<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
		// .AllEndpointsHaveResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
		// .AllEndpointsProduce(StatusCodes.Status401Unauthorized)
		// .AllEndpointsHaveResponseDescription(StatusCodes.Status401Unauthorized, ResponseDescription.For401Unauthorized)
		// .AllEndpointsProduce(StatusCodes.Status403Forbidden)
		// .AllEndpointsHaveResponseDescription(StatusCodes.Status403Forbidden, ResponseDescription.For403Forbidden);
	}
}

