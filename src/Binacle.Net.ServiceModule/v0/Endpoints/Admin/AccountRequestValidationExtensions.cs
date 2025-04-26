using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Microsoft.AspNetCore.Http;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin;

internal static class AccountRequestValidationExtensions
{
	public static async Task<IResult> WithValidatedRequest<TRequest>(
		this ValidatedBindingResult<TRequest> request,
		Func<TRequest, Task<IResult>> handleRequest)
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
