using Binacle.Net.Api.ServiceModule.Domain.Users.Models;
using Binacle.Net.Api.ServiceModule.Services;
using Binacle.Net.Api.ServiceModule.v0.Requests;
using Binacle.Net.Api.ServiceModule.v0.Responses;
using ChrisMavrommatis.MinimalEndpointDefinitions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace Binacle.Net.Api.ServiceModule.v0.Endpoints.Users;

internal class Create : IEndpointDefinition<UsersGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("/", HandleAsync)
			.WithSummary("Create a user")
			.WithDescription("Use this endpoint if you are the  admin to create users")
			.Accepts<CreateApiUserRequest>("application/json")
			.WithOpenApi();
	}

	[SwaggerResponse(StatusCodes.Status201Created, "When you have successfully created a user")]
	[SwaggerResponse(StatusCodes.Status400BadRequest, "When the request is invalid", typeof(ErrorResponse), "application/json")]
	[SwaggerResponse(StatusCodes.Status401Unauthorized, "When provided user token is invalid")]
	[SwaggerResponse(StatusCodes.Status403Forbidden, "When provided user token does not have permission")]
	[SwaggerResponse(StatusCodes.Status409Conflict, "When a user with the same email exists")]
	internal async Task<IResult> HandleAsync(
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
}
