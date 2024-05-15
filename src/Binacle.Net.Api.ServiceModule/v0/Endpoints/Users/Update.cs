using Binacle.Net.Api.ServiceModule.Domain.Users.Entities;
using Binacle.Net.Api.ServiceModule.Domain.Users.Models;
using Binacle.Net.Api.ServiceModule.Services;
using Binacle.Net.Api.ServiceModule.v0.Requests;
using Binacle.Net.Api.ServiceModule.v0.Responses;
using ChrisMavrommatis.MinimalEndpointDefinitions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace Binacle.Net.Api.ServiceModule.v0.Endpoints.Users;

internal class Update : IEndpointDefinition<UsersGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPut("/{email}", HandleAsync)
			.WithSummary("Update a user")
			.WithDescription("Use this endpoint if you are the admin to update the user but not change the password.")
			.Accepts<UpdateApiUserRequestWithBody>("application/json")
			.WithOpenApi();
	}

	[SwaggerResponse(StatusCodes.Status204NoContent, "When the user was updated")]
	[SwaggerResponse(StatusCodes.Status400BadRequest, "When the request is invalid", typeof(ErrorResponse), "application/json")]
	[SwaggerResponse(StatusCodes.Status401Unauthorized, "When provided user token is invalid")]
	[SwaggerResponse(StatusCodes.Status403Forbidden, "When provided user token does not have permission")]
	[SwaggerResponse(StatusCodes.Status404NotFound, "When the user does not exist")]
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

		var userGroup = request.Body.Type switch
		{
			Models.UserType.Admin => UserGroups.Admins,
			Models.UserType.User => UserGroups.Users,
			_ => null
		};

		var isActive = request.Body.Status switch
		{
			Models.UserStatus.Active => (bool?)true,
			Models.UserStatus.Inactive => (bool?)false,
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
