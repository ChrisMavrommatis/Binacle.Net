using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Users.Models;
using Binacle.Net.ServiceModule.Services;
using Binacle.Net.ServiceModule.Constants;
using Binacle.Net.ServiceModule.v0.Requests;
using Binacle.Net.ServiceModule.v0.Requests.Examples;
using Binacle.Net.ServiceModule.v0.Responses;
using Binacle.Net.ServiceModule.v0.Responses.Examples;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Users;

internal class Create : IGroupedEndpoint<UsersGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("/", HandleAsync)
			.WithSummary("Create a user")
			.WithDescription("Use this endpoint if you are the  admin to create users")
			.Accepts<CreateApiUserRequest>("application/json")
			.RequestExample<CreateApiUserRequestExample>("application/json")
			.Produces(StatusCodes.Status201Created)
			.WithResponseDescription(StatusCodes.Status201Created, ResponseDescription.ForCreate200OK)
			.ResponseExamples<CreateApiUserErrorResponseExample>(
				StatusCodes.Status400BadRequest,
				"application/json"
			)
			.Produces(StatusCodes.Status409Conflict)
			.WithResponseDescription(StatusCodes.Status409Conflict, ResponseDescription.ForCreate409Conflict);
	}

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
