using Binacle.Net.Api.Kernel.Endpoints;
using Binacle.Net.Api.ServiceModule.Constants;
using Binacle.Net.Api.ServiceModule.Domain.Users.Entities;
using Binacle.Net.Api.ServiceModule.Domain.Users.Models;
using Binacle.Net.Api.ServiceModule.Models;
using Binacle.Net.Api.ServiceModule.Services;
using Binacle.Net.Api.ServiceModule.v0.Requests;
using Binacle.Net.Api.ServiceModule.v0.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Binacle.Net.Api.ServiceModule.v0.Endpoints.Users;

internal class Update : IGroupedEndpoint<UsersGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPut("/{email}", HandleAsync)
			.WithSummary("Update a user")
			.WithDescription("Use this endpoint if you are the admin to update the user but not change the password.")
			.Accepts<UpdateApiUserRequestWithBody>("application/json")
			.Produces(StatusCodes.Status204NoContent)
			.WithResponseDescription(StatusCodes.Status204NoContent, ResponseDescription.ForUpdate204NoContent)
			.Produces(StatusCodes.Status404NotFound)
			.WithResponseDescription(StatusCodes.Status404NotFound, ResponseDescription.ForUpdate404NotFound);
	}

	// [SwaggerRequestExample(typeof(v0.Requests.UpdateApiUserRequest), typeof(v0.Requests.Examples.UpdateApiUserRequestExample))]
	// [SwaggerResponseExample(typeof(v0.Responses.ErrorResponse), typeof(v0.Responses.Examples.UpdateApiUserErrorResponseExample), StatusCodes.Status400BadRequest)]
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

		var userGroup = request.Body!.Type switch
		{
			UserType.Admin => UserGroups.Admins,
			UserType.User => UserGroups.Users,
			_ => null
		};

		var isActive = request.Body.Status switch
		{
			UserStatus.Active => (bool?)true,
			UserStatus.Inactive => (bool?)false,
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
