﻿using Binacle.Net.Api.ServiceModule.Domain.Users.Entities;
using Binacle.Net.Api.ServiceModule.Domain.Users.Models;
using Binacle.Net.Api.ServiceModule.Extensions;
using Binacle.Net.Api.ServiceModule.Services;
using Binacle.Net.Api.ServiceModule.v0.Requests;
using Binacle.Net.Api.ServiceModule.v0.Requests.Examples;
using Binacle.Net.Api.ServiceModule.v0.Responses;
using Binacle.Net.Api.ServiceModule.v0.Responses.Examples;
using ChrisMavrommatis.MinimalEndpointDefinitions;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Binacle.Net.Api.ServiceModule.v0.Endpoints.Users;

internal class Update : IEndpointDefinition<UsersGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPut("/{email}", HandleAsync)
			.WithSummary("Update a user")
			.WithDescription("Use this endpoint if you are the admin to update the user but not change the password.")
			.Accepts<UpdateApiUserRequestWithBody>("application/json")
			.Produces(StatusCodes.Status204NoContent)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.Produces(StatusCodes.Status401Unauthorized)
			.Produces(StatusCodes.Status403Forbidden)
			.Produces(StatusCodes.Status404NotFound)
			.WithOpenApi(operation =>
			{
				operation.SetResponseDescription(StatusCodes.Status204NoContent, @"**No Content**
				<br />
				<p>
					When the user was updated.
				</p>");

				operation.SetResponseDescription(StatusCodes.Status400BadRequest, @"**Bad Request**
				<br />
				<p>
					When the request is invalid.
				</p>");

				operation.SetResponseDescription(StatusCodes.Status401Unauthorized, @"**Unauthorized**
				<br />
				<p>
					When provided user token is invalid.
				</p>");

				operation.SetResponseDescription(StatusCodes.Status403Forbidden, @"**Forbidden**
				<br />
				<p>
					When provided user token does not have permission.
				</p>");

				operation.SetResponseDescription(StatusCodes.Status404NotFound, @"**Not Found**
				<br />
				<p>
					When the user does not exist.
				</p>");

				return operation;
			});
	}

	[SwaggerRequestExample(typeof(UpdateApiUserRequest), typeof(UpdateApiUserRequestExample))]
	[SwaggerResponseExample(typeof(ErrorResponse), typeof(UpdateApiUserErrorResponseExample), StatusCodes.Status400BadRequest)]
	internal async Task<IResult> HandleAsync(
			IUserManagerService userManagerService,
			[AsParameters] UpdateApiUserRequestWithBody request,
			IValidator<UpdateApiUserRequestWithBody> validator,
			CancellationToken cancellationToken = default
		)
	{
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.BadRequest(ErrorResponse.Create("Validation Error", validationResult.Errors.Select(x => x.ErrorMessage).ToArray()));
		}

		var userGroup = request.Body.Type switch
		{
			Models.UserType.Admin => UserGroups.Admins,
			Models.UserType.User => UserGroups.Users,
			_ => null
		};

		var isActive = request.Body.Status switch
		{
			Models.UserStatus.Active => (bool?)true,
			Models.UserStatus.Inactive => (bool?)false,
			_ => null
		};

		var result = await userManagerService.UpdateAsync(new UpdateUserRequest(request.Email, userGroup, isActive), cancellationToken);

		return result.Unwrap(
			ok => Results.NoContent(),
			notFound => Results.NotFound(),
			error => Results.BadRequest(ErrorResponse.Create(error.Message))
		);
	}
}
