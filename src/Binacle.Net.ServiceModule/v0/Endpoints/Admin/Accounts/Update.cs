using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Users.Entities;
using Binacle.Net.ServiceModule.Domain.Users.Models;
using Binacle.Net.ServiceModule.Services;
using Binacle.Net.ServiceModule.Constants;
using Binacle.Net.ServiceModule.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Requests;
using Binacle.Net.ServiceModule.v0.Requests.Examples;
using Binacle.Net.ServiceModule.v0.Resources;
using Binacle.Net.ServiceModule.v0.Responses;
using Binacle.Net.ServiceModule.v0.Responses.Examples;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;
using YetAnotherMediator;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Accounts;

internal class Update : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPut("/account/{id}", HandleAsync)
			.WithSummary("Update an account")
			.WithDescription("Admins can use this endpoint to update the account.")
			.Accepts<UpdateAccountRequest>("application/json")
			.RequestExamples<UpdateAccountRequestExamples>("application/json")
			.Produces(StatusCodes.Status204NoContent)
			.WithResponseDescription(StatusCodes.Status204NoContent, UpdateAccountResponseDescription.For204NoContent)
			.ResponseExamples<UpdateAccountErrorResponseExamples>(
				StatusCodes.Status400BadRequest,
				"application/json"
			)
			.Produces(StatusCodes.Status404NotFound)
			.WithResponseDescription(StatusCodes.Status404NotFound, AccountResponseDescription.For404NotFound);
	}

	internal async Task<IResult> HandleAsync(
		string id, // TODO validate it
		ValidatedBindingResult<UpdateAccountRequest> request,
		IMediator mediator,
		CancellationToken cancellationToken = default
	)
	{
		if (request.Value is null)
		{
			return Results.BadRequest(
				ErrorResponse.MalformedRequest()
			);
		}

		if (!request.ValidationResult?.IsValid ?? false)
		{
			return Results.BadRequest(
				ErrorResponse.ValidationError(
					request.ValidationResult!.Errors.Select(x => x.ErrorMessage).ToArray()
				)
			);
		}

		var command = new UpdateAccountCommand()
		{

		};
		
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

		var result = await userManagerService.UpdateAsync(
			new UpdateUserRequest(request.Email, userGroup, isActive),
			cancellationToken
		);

		return result.Unwrap(
			ok => Results.NoContent(),
			notFound => Results.NotFound(),
			error => Results.BadRequest(ErrorResponse.Create(error.Message))
		);
	}
}
