﻿using Binacle.Net.Api.Kernel.Endpoints;
using Binacle.Net.Api.ServiceModule.Constants;
using Binacle.Net.Api.ServiceModule.Domain.Users.Models;
using Binacle.Net.Api.ServiceModule.Services;
using Binacle.Net.Api.ServiceModule.v0.Requests;
using Binacle.Net.Api.ServiceModule.v0.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Binacle.Net.Api.ServiceModule.v0.Endpoints.Users;

internal class Delete : IGroupedEndpoint<UsersGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapDelete("/{email}", HandleAsync)
			.WithSummary("Delete a user")
			.WithDescription("Use this endpoint if you are the admin to delete a user")
			.Produces(StatusCodes.Status204NoContent)
			.WithResponseDescription(StatusCodes.Status204NoContent, ResponseDescription.ForDelete204NoContent)
			.Produces(StatusCodes.Status404NotFound)
			.WithResponseDescription(StatusCodes.Status404NotFound, ResponseDescription.ForDelete404NotFound);
	}

	// [SwaggerResponseExample(typeof(v0.Responses.ErrorResponse), typeof(v0.Responses.Examples.DeleteApiUserErrorResponseExample), StatusCodes.Status400BadRequest)]
	internal async Task<IResult> HandleAsync(
			IUserManagerService userManagerService,
			[AsParameters] DeleteApiUserRequest request,
			IValidator<DeleteApiUserRequest> validator,
			CancellationToken cancellationToken = default
		)
	{
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.BadRequest(ErrorResponse.Create("Validation Error", validationResult.Errors.Select(x => x.ErrorMessage).ToArray()));
		}

		var result = await userManagerService.DeleteAsync(new DeleteUserRequest(request.Email), cancellationToken);

		return result.Unwrap(
			ok => Results.NoContent(),
			notFound => Results.NotFound(),
			error => Results.BadRequest(ErrorResponse.Create(error.Message))
		);
	}
}
