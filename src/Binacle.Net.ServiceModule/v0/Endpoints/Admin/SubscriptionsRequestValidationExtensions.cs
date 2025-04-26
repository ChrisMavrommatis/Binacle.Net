using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using FluxResults.Unions;
using Microsoft.AspNetCore.Http;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin;

internal static class SubscriptionsRequestValidationExtensions
{
	public static async Task<IResult> WithAccountValidatedRequest<TRequest>(
		this ValidatedBindingResult<TRequest> request,
		IAccountRepository accountRepository,
		string id,
		Func<TRequest, Account, Task<IResult>> handleRequest
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

			if (!Guid.TryParse(id, out var accountId))
			{
				return Results.BadRequest(
					ErrorResponse.IdToGuidParameterError
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

			var accountResult = await accountRepository.GetByIdAsync(accountId);
			if (!accountResult.TryGetValue<Account>(out var account) && account is null)
			{
				return Results.NotFound();
			}

			return await handleRequest(request.Value, account!);
		}
		catch (Exception ex)
		{
			return Results.InternalServerError(
				ErrorResponse.ServerError(ex)
			);
		}
	}
	
	public static async Task<IResult> WithAccountValidatedTryCatch(
		IAccountRepository accountRepository,
		string id,
		Func<Account, Task<IResult>> handleRequest
	)
	{
		try
		{
			if (!Guid.TryParse(id, out var accountId))
			{
				return Results.BadRequest(
					ErrorResponse.IdToGuidParameterError
				);
			}
			
			var accountResult = await accountRepository.GetByIdAsync(accountId);
			if (!accountResult.TryGetValue<Account>(out var account) && account is null)
			{
				return Results.NotFound();
			}
			
			return await handleRequest(account!);
		}
		catch (Exception ex)
		{
			return Results.InternalServerError(
				ErrorResponse.ServerError(ex)
			);
		}
	}
}
