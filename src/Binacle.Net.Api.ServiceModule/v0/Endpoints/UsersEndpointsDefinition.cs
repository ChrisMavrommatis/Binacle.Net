using Binacle.Net.Api.ServiceModule.Domain.Users.Entities;
using Binacle.Net.Api.ServiceModule.Domain.Users.Models;
using Binacle.Net.Api.ServiceModule.Models;
using Binacle.Net.Api.ServiceModule.Services;
using Binacle.Net.Api.ServiceModule.v0.Requests;
using Binacle.Net.Api.ServiceModule.v0.Responses;
using ChrisMavrommatis.MinimalEndpointDefinitions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Annotations;

namespace Binacle.Net.Api.ServiceModule.v0.Endpoints;

internal class UsersEndpointsDefinition : IEndpointDefinition
{
	public void DefineEndpoints(WebApplication app)
	{
		if (!app.Environment.IsDevelopment())
		{
			return;
		}

		var group = app.MapGroup(UsersApiDocument.ApiName)
			.WithTags("Users (Admin only)")
			.RequireAuthorization(builder =>
			{
				builder.RequireAuthenticatedUser();
				builder.RequireClaim(JwtApplicationClaimNames.Groups, UserGroups.Admins);
			}).WithGroupName(UsersApiDocument.ApiName);

		group.MapPost("/", Create)
			.WithSummary("Create a user")
			.WithDescription("Use this endpoint if you are the  admin to create users")
			.Accepts<CreateApiUserRequest>("application/json")
			.WithOpenApi();

		group.MapDelete("/{email}", Delete)
			.WithSummary("Delete a user")
			.WithDescription("Use this endpoint if you are the admin to delete a user")
			.WithOpenApi();

		group.MapPatch("/{email}", ChangePassword)
			.WithSummary("Change a user's password")
			.WithDescription("Use this endpoint if you are the admin to change a user's password")
			.Accepts<ChangeApiUserPasswordRequest>("application/json")
			.WithOpenApi();
	}

	[SwaggerResponse(StatusCodes.Status201Created, "When you have successfully created a user")]
	[SwaggerResponse(StatusCodes.Status400BadRequest, "When the request is invalid", typeof(ErrorResponse), "application/json")]
	[SwaggerResponse(StatusCodes.Status401Unauthorized, "When provided user token is invalid")]
	[SwaggerResponse(StatusCodes.Status403Forbidden, "When provided user token does not have permission")]
	[SwaggerResponse(StatusCodes.Status409Conflict, "When a user with the same email exists")]
	internal async Task<IResult> Create(
			IUserManagerService userManagerService,
			IValidator<CreateApiUserRequest> validator,
			[FromBody] CreateApiUserRequest request,
			CancellationToken cancellationToken = default)
	{
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.BadRequest(ErrorResponse.Create("Validation Error", validationResult.Errors.Select(x => x.ErrorMessage).ToArray()));
		}

		var result = await userManagerService.CreateAsync(new CreateUserRequest(request.Email, request.Password), cancellationToken);

		return result.Unwrap(
			user => Results.Created(),
			conflict => Results.Conflict(),
			error => Results.BadRequest(ErrorResponse.Create(error.Message))
			);
	}

	[SwaggerResponse(StatusCodes.Status204NoContent, "When the user was deleted")]
	[SwaggerResponse(StatusCodes.Status400BadRequest, "When the request is invalid", typeof(ErrorResponse), "application/json")]
	[SwaggerResponse(StatusCodes.Status401Unauthorized, "When provided user token is invalid")]
	[SwaggerResponse(StatusCodes.Status403Forbidden, "When provided user token does not have permission")]
	[SwaggerResponse(StatusCodes.Status404NotFound, "When the user does not exist")]
	internal async Task<IResult> Delete(
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

	[SwaggerResponse(StatusCodes.Status204NoContent, "When the password was changed")]
	[SwaggerResponse(StatusCodes.Status400BadRequest, "When the request is invalid", typeof(ErrorResponse), "application/json")]
	[SwaggerResponse(StatusCodes.Status401Unauthorized, "When provided user token is invalid")]
	[SwaggerResponse(StatusCodes.Status403Forbidden, "When provided user token does not have permission")]
	[SwaggerResponse(StatusCodes.Status404NotFound, "When the user does not exist")]
	[SwaggerResponse(StatusCodes.Status409Conflict, "When the password is the same as the old")]
	internal async Task<IResult> ChangePassword(
			IUserManagerService userManagerService,
			[AsParameters] ChangeApiUserPasswordRequestWithBody request,
			IValidator<ChangeApiUserPasswordRequestWithBody> validator,
			CancellationToken cancellationToken = default
		)
	{
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.BadRequest(ErrorResponse.Create("Validation Error", validationResult.Errors.Select(x => x.ErrorMessage).ToArray()));
		}

		var result = await userManagerService.ChangePasswordAsync(new ChangeUserPasswordRequest(request.Email, request.Body.Password), cancellationToken);

		return result.Unwrap(
			ok => Results.NoContent(),
			notFound => Results.NotFound(),
			conflict => Results.Conflict(),
			error => Results.BadRequest(ErrorResponse.Create(error.Message))
		);
	}
}
