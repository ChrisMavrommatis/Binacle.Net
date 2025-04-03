using Binacle.Net.Api.Kernel.Endpoints;
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

internal class ChangePassword : IGroupedEndpoint<UsersGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPatch("/{email}", HandleAsync)
			.WithSummary("Change a user's password")
			.WithDescription("Use this endpoint if you are the admin to change a user's password")
			.Accepts<ChangeApiUserPasswordRequest>("application/json")
			.Produces(StatusCodes.Status204NoContent)
			.WithResponseDescription(StatusCodes.Status204NoContent, ResponseDescription.ForChangePassword204NoContent)
			.Produces(StatusCodes.Status404NotFound)
			.WithResponseDescription(StatusCodes.Status404NotFound, ResponseDescription.ForChangePassword404NotFound)
			.Produces(StatusCodes.Status409Conflict)
			.WithResponseDescription(StatusCodes.Status409Conflict, ResponseDescription.ForChangePassword409Conflict);
	}

	// [SwaggerRequestExample(typeof(v0.Requests.ChangeApiUserPasswordRequest), typeof(v0.Requests.Examples.ChangeApiUserPasswordRequestExample))]
	// [SwaggerResponseExample(typeof(v0.Responses.ErrorResponse), typeof(v0.Responses.Examples.ChangeApiUserPasswordErrorResponseExample), StatusCodes.Status400BadRequest)]
	internal async Task<IResult> HandleAsync(
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

		var result = await userManagerService.ChangePasswordAsync(new ChangeUserPasswordRequest(request.Email, request.Body!.Password), cancellationToken);

		return result.Unwrap(
			ok => Results.NoContent(),
			notFound => Results.NotFound(),
			conflict => Results.Conflict(),
			error => Results.BadRequest(ErrorResponse.Create(error.Message))
		);
	}
}
