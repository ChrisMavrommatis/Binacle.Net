using Binacle.Net.Api.ServiceModule.Domain.Users.Models;
using Binacle.Net.Api.ServiceModule.Extensions;
using Binacle.Net.Api.ServiceModule.Services;
using ChrisMavrommatis.MinimalEndpointDefinitions;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Binacle.Net.Api.ServiceModule.v0.Endpoints.Users;

internal class ChangePassword : IEndpointDefinition<UsersGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPatch("/{email}", HandleAsync)
			.WithSummary("Change a user's password")
			.WithDescription("Use this endpoint if you are the admin to change a user's password")
			.Accepts<v0.Requests.ChangeApiUserPasswordRequest>("application/json")
			.Produces(StatusCodes.Status204NoContent)
			.Produces<v0.Responses.ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.Produces(StatusCodes.Status401Unauthorized)
			.Produces(StatusCodes.Status403Forbidden)
			.Produces(StatusCodes.Status404NotFound)
			.Produces(StatusCodes.Status409Conflict)
			.WithOpenApi(operation =>
			{
				operation.SetResponseDescription(StatusCodes.Status204NoContent, @"**No Content**
				<br />
				<p>
					When the password was changed.
				</p>");

				operation.SetResponseDescription(StatusCodes.Status400BadRequest, @"**Bad Request**
				<br />
				<p>
					When the request is invalid.
				</p>");

				operation.SetResponseDescription(StatusCodes.Status401Unauthorized, @" **Unauthorized**
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

				operation.SetResponseDescription(StatusCodes.Status409Conflict, @"**Conflict**
				<br />
				<p>
					When the password is the same as the old.
				</p>");

				return operation;
			});
	}

	[SwaggerRequestExample(typeof(v0.Requests.ChangeApiUserPasswordRequest), typeof(v0.Requests.Examples.ChangeApiUserPasswordRequestExample))]
	[SwaggerResponseExample(typeof(v0.Responses.ErrorResponse), typeof(v0.Responses.Examples.ChangeApiUserPasswordErrorResponseExample), StatusCodes.Status400BadRequest)]
	internal async Task<IResult> HandleAsync(
			IUserManagerService userManagerService,
			[AsParameters] v0.Requests.ChangeApiUserPasswordRequestWithBody request,
			IValidator<v0.Requests.ChangeApiUserPasswordRequestWithBody> validator,
			CancellationToken cancellationToken = default
		)
	{
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.BadRequest(v0.Responses.ErrorResponse.Create("Validation Error", validationResult.Errors.Select(x => x.ErrorMessage).ToArray()));
		}

		var result = await userManagerService.ChangePasswordAsync(new ChangeUserPasswordRequest(request.Email, request.Body.Password), cancellationToken);

		return result.Unwrap(
			ok => Results.NoContent(),
			notFound => Results.NotFound(),
			conflict => Results.Conflict(),
			error => Results.BadRequest(v0.Responses.ErrorResponse.Create(error.Message))
		);
	}
}
