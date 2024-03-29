using Binacle.Net.Api.ServiceModule.ApiModels.Requests;
using Binacle.Net.Api.ServiceModule.ApiModels.Responses;
using Binacle.Net.Api.ServiceModule.Data.Entities;
using Binacle.Net.Api.ServiceModule.Models;
using Binacle.Net.Api.ServiceModule.Services;
using ChrisMavrommatis.Api.MinimalEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Annotations;

namespace Binacle.Net.Api.ServiceModule.EndpointDefinitions;

internal class UsersEndpointsDefinition : IEndpointDefinition
{
	public void DefineEndpoints(WebApplication app)
	{
		var group = app.MapGroup("/users")
			.WithTags("Users (Admin only)")
			.RequireAuthorization(builder =>
			{
				builder.RequireAuthenticatedUser();
				builder.RequireClaim(JwtApplicationClaimNames.Groups, UserGroups.Admins);
			});

		if (!app.Environment.IsDevelopment())
		{
			group = group.ExcludeFromDescription();
		}

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
	[SwaggerResponse(StatusCodes.Status400BadRequest, "When the request is invalid", typeof(DescriptiveErrorResponse), "application/json")]
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
			return Results.BadRequest(DescriptiveErrorResponse.Create("Validation Error", validationResult.Errors.Select(x => x.ErrorMessage).ToArray()));
		}

		var result = await userManagerService.CreateAsync(new UserActionRequest(request.Email, request.Password), cancellationToken);
		if (!result.Success)
		{
			var responseResult = result.ResultType switch
			{
				UserActionResultType.Conflict => Results.Conflict(),
				_ => Results.BadRequest(DescriptiveErrorResponse.Create(result.Message))
			};

			return responseResult;
		}

		return Results.Created();
	}

	[SwaggerResponse(StatusCodes.Status204NoContent, "When the user was deleted")]
	[SwaggerResponse(StatusCodes.Status400BadRequest, "When the request is invalid", typeof(DescriptiveErrorResponse), "application/json")]
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
			return Results.BadRequest(DescriptiveErrorResponse.Create("Validation Error", validationResult.Errors.Select(x => x.ErrorMessage).ToArray()));
		}

		var result = await userManagerService.DeleteAsync(new UserActionRequest(request.Email, string.Empty), cancellationToken);

		if (!result.Success)
		{
			var responseResult = result.ResultType switch
			{
				UserActionResultType.NotFound => Results.NotFound(),
				_ => Results.BadRequest(DescriptiveErrorResponse.Create(result.Message))
			};

			return responseResult;
		}

		return Results.NoContent();

	}

	[SwaggerResponse(StatusCodes.Status204NoContent, "When the password was changed")]
	[SwaggerResponse(StatusCodes.Status400BadRequest, "When the request is invalid", typeof(DescriptiveErrorResponse), "application/json")]
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
			return Results.BadRequest(DescriptiveErrorResponse.Create("Validation Error", validationResult.Errors.Select(x => x.ErrorMessage).ToArray()));
		}

		var result = await userManagerService.ChangePasswordAsync(new UserActionRequest(request.Email, request.Body.Password), cancellationToken);
		if (!result.Success)
		{
			var responseResult = result.ResultType switch
			{
				UserActionResultType.Conflict => Results.Conflict(),
				UserActionResultType.NotFound => Results.NotFound(),
				_ => Results.BadRequest(DescriptiveErrorResponse.Create(result.Message))
			};

			return responseResult;
		}

		return Results.NoContent();
	}
}
