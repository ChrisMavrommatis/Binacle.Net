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

internal class Delete : IEndpointDefinition<UsersGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapDelete("/{email}", HandleAsync)
			.WithSummary("Delete a user")
			.WithDescription("Use this endpoint if you are the admin to delete a user")
			.WithOpenApi();
	}

	[SwaggerResponse(StatusCodes.Status204NoContent, "When the user was deleted")]
	[SwaggerResponse(StatusCodes.Status400BadRequest, "When the request is invalid", typeof(ErrorResponse), "application/json")]
	[SwaggerResponse(StatusCodes.Status401Unauthorized, "When provided user token is invalid")]
	[SwaggerResponse(StatusCodes.Status403Forbidden, "When provided user token does not have permission")]
	[SwaggerResponse(StatusCodes.Status404NotFound, "When the user does not exist")]
	internal async Task<IResult> HandleAsync(
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
}
