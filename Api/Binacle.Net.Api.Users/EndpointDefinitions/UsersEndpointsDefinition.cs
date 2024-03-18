﻿using Binacle.Net.Api.Users.Data.Schemas;
using Binacle.Net.Api.Users.Data.Services;
using Binacle.Net.Api.Users.Models;
using Binacle.Net.Api.Users.Requests;
using Binacle.Net.Api.Users.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Binacle.Net.Api.Users.EndpointDefinitions;

internal class UsersEndpointsDefinition : IEndpointDefinition
{
	public void DefineEndpoints(WebApplication app)
	{
		var group = app.MapGroup("/users")
			.WithTags("Users")
			.RequireAuthorization(builder =>
			{
				builder.RequireAuthenticatedUser();
				builder.RequireClaim(JwtApplicationClaimNames.Groups, UserGroups.Admins);
			}).ExcludeFromDescription();


		group.MapPost("/", Create)
			.WithSummary("Create a user (Admin)")
			.WithDescription("Use this endpoint if you are the  admin to create users")
			.Accepts<TokenRequest>("application/json")
			.WithOpenApi(); 
	}


	[SwaggerResponse(StatusCodes.Status201Created, "When you have successfully created a user")]
	[SwaggerResponse(StatusCodes.Status400BadRequest, "When the request is invalid", typeof(AuthErrorResponse), "application/json")]
	[SwaggerResponse(StatusCodes.Status401Unauthorized, "When provided user token is invalid")]
	[SwaggerResponse(StatusCodes.Status403Forbidden, "When provided user token does not have permission")]
	[SwaggerResponse(StatusCodes.Status409Conflict, "When a user with the same email exists", typeof(AuthErrorResponse), "application/json")]
	internal async Task<IResult> Create(
			IAuthService authService,
			IValidator<CreateApiUserRequest> validator,
			[FromBody] CreateApiUserRequest request,
			CancellationToken cancellationToken = default)
	{
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.BadRequest(AuthErrorResponse.Create("Validation Error", validationResult.Errors.Select(x => x.ErrorMessage).ToArray()));
		}

		var createUserResult = await authService.CreateAsync(new CreateUserRequest(request.Email, request.Password), cancellationToken);
		if (!createUserResult.Success)
		{
			var result = createUserResult.Reason switch
			{
				CreateUserFailedResultReason.InvalidCredentials => Results.BadRequest(AuthErrorResponse.Create("Invalid credentials")),
				CreateUserFailedResultReason.AlreadyExists => Results.Conflict(AuthErrorResponse.Create("User already exists")),
				_ => Results.BadRequest(AuthErrorResponse.Create("Something went wrong, check your request"))
			};
			return result;

		}
		return Results.Created();
	}
}
