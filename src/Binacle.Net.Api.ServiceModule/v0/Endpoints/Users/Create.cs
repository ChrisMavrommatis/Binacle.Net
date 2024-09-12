using Binacle.Net.Api.ServiceModule.Domain.Users.Models;
using Binacle.Net.Api.ServiceModule.Extensions;
using Binacle.Net.Api.ServiceModule.Services;
using ChrisMavrommatis.MinimalEndpointDefinitions;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Binacle.Net.Api.ServiceModule.v0.Endpoints.Users;

internal class Create : IEndpointDefinition<UsersGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("/", HandleAsync)
			.WithSummary("Create a user")
			.WithDescription("Use this endpoint if you are the  admin to create users")
			.Accepts<v0.Requests.CreateApiUserRequest>("application/json")
			.Produces(StatusCodes.Status201Created)
			.Produces<v0.Responses.ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.Produces(StatusCodes.Status401Unauthorized)
			.Produces(StatusCodes.Status403Forbidden)
			.Produces(StatusCodes.Status409Conflict)
			.WithOpenApi(operation =>
			{
				operation.SetResponseDescription(StatusCodes.Status201Created, @"**Created**
				<br />
				<p>
					When you have successfully created a user.
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

				operation.SetResponseDescription(StatusCodes.Status409Conflict, @"**Conflict**
				<br />
				<p>
					When the user already exists.
				</p>");

				return operation;
			});
	}

	[SwaggerRequestExample(typeof(v0.Requests.CreateApiUserRequest), typeof(v0.Requests.Examples.CreateApiUserRequestExample))]
	[SwaggerResponseExample(typeof(v0.Responses.ErrorResponse), typeof(v0.Responses.Examples.CreateApiUserErrorResponseExample), StatusCodes.Status400BadRequest)]
	internal async Task<IResult> HandleAsync(
			IUserManagerService userManagerService,
			IValidator<v0.Requests.CreateApiUserRequest> validator,
			[FromBody] v0.Requests.CreateApiUserRequest request,
			CancellationToken cancellationToken = default)
	{
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.BadRequest(v0.Responses.ErrorResponse.Create("Validation Error", validationResult.Errors.Select(x => x.ErrorMessage).ToArray()));
		}

		var result = await userManagerService.CreateAsync(new CreateUserRequest(request.Email, request.Password), cancellationToken);

		return result.Unwrap(
			user => Results.Created(),
			conflict => Results.Conflict(),
			error => Results.BadRequest(v0.Responses.ErrorResponse.Create(error.Message))
			);
	}
}
