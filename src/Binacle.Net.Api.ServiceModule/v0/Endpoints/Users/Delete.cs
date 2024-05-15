using Binacle.Net.Api.ServiceModule.Domain.Users.Models;
using Binacle.Net.Api.ServiceModule.Services;
using Binacle.Net.Api.ServiceModule.v0.Requests;
using Binacle.Net.Api.ServiceModule.v0.Responses;
using Binacle.Net.Api.ServiceModule.v0.Responses.Examples;
using ChrisMavrommatis.MinimalEndpointDefinitions;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;


namespace Binacle.Net.Api.ServiceModule.v0.Endpoints.Users;

internal class Delete : IEndpointDefinition<UsersGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapDelete("/{email}", HandleAsync)
			.WithSummary("Delete a user")
			.WithDescription("Use this endpoint if you are the admin to delete a user")
			.Produces(StatusCodes.Status204NoContent)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.Produces(StatusCodes.Status401Unauthorized)
			.Produces(StatusCodes.Status403Forbidden)
			.Produces(StatusCodes.Status404NotFound)
			.WithOpenApi(operation =>
			{
				operation.Responses["204"].Description = @"**No Content**
				<br />
				<p>
					When the user was deleted.
				</p>";

				operation.Responses["400"].Description = @"**Bad Request**
				<br />
				<p>
					When the request is invalid.
				</p>";

				operation.Responses["401"].Description = @"**Unauthorized**
				<br />
				<p>
					When provided user token is invalid.
				</p>";

				operation.Responses["403"].Description = @"**Forbidden**
				<br />
				<p>
					When provided user token does not have permission.
				</p>";

				operation.Responses["404"].Description = @"**Not Found**
				<br />
				<p>
					When the user does not exist.
				</p>";

				return operation;
			});
	}

	[SwaggerResponseExample(typeof(ErrorResponse), typeof(DeleteApiUserErrorResponseExample), StatusCodes.Status400BadRequest)]
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
