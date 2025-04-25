using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin;

internal class AdminGroup : IEndpointGroup
{
	public RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapGroup($"/api/admin")
			.WithTags("Admin")
			.RequireAuthorization("Admin")
			.WithGroupName(ServiceModuleApiDocument.DocumentName)
			.AllEndpointsProduce<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.AllEndpointsHaveResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.AllEndpointsProduce(StatusCodes.Status401Unauthorized)
			.AllEndpointsHaveResponseDescription(
				StatusCodes.Status401Unauthorized,
				ResponseDescription.For401Unauthorized
			)
			.AllEndpointsProduce(StatusCodes.Status403Forbidden)
			.AllEndpointsHaveResponseDescription(StatusCodes.Status403Forbidden, ResponseDescription.For403Forbidden)
			.AllEndpointsProduce<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json")
			.AllEndpointsHaveResponseDescription(
				StatusCodes.Status500InternalServerError,
				ResponseDescription.For500InternalServerError
			)
			.AllEndpointsHaveResponseExample<InternalServerErrorErrorResponseExample>(
				StatusCodes.Status500InternalServerError, 
				"application/json"
			);
	}
}

internal static class RequestValidationExtensions
{
	public static async Task<IResult> WithValidatedRequest<TRequest>(
		this ValidatedBindingResult<TRequest> request,
		Func<TRequest, Task<IResult>> handleRequest
	)
	{
		try
		{
			if (request.Value is null)
			{
				return Results.BadRequest(
					ErrorResponse.MalformedRequest
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

			return await handleRequest(request.Value);
		}
		catch (Exception ex)
		{
			return Results.InternalServerError(
				ErrorResponse.ServerError(ex)
			);
		}
	}

	public static async Task<IResult> WithTryCatch(
		Func<Task<IResult>> handleRequest
	)
	{
		try
		{
			return await handleRequest();
		}
		catch (Exception ex)
		{
			return Results.InternalServerError(
				ErrorResponse.ServerError(ex)
			);
		}
	}
}
