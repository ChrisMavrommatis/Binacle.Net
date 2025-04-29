using System.Text.Json;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.ServiceModule.v0.Endpoints;

internal class AccountBindingResult<T>
{
	private readonly IServiceProvider serviceProvider;
	private readonly T? request;
	private readonly Exception? exception;
	private readonly CancellationToken cancelationToken;

	private AccountBindingResult(
		IServiceProvider serviceProvider,
		T? request,
		Exception? exception,
		CancellationToken cancelationToken = default
	)
	{
		this.serviceProvider = serviceProvider;
		this.request = request;
		this.exception = exception;
		this.cancelationToken = cancelationToken;
	}

	public async Task<IResult> ValidateAsync(
		string accountId, 
		Func<T, Account, Task<IResult>> handleRequest
		)
	{
		if (!Guid.TryParse(accountId, out var parsedAccountId))
		{
			var errors = new Dictionary<string, string[]>
			{
				{"id", ["The provided value is not a valid Guid"]}
			};
			return Results.ValidationProblem(errors);
		}
		
		if (this.exception is not null)
		{
			var problemDetails = GetProblemDetails(this.exception);
			return Results.Problem(problemDetails);
		}

		if (this.request is null)
		{
			var problemDetails = new ProblemDetails
			{
				Status = StatusCodes.Status400BadRequest,
				Title = "Bad Request",
				Detail = "The server could not read the request",
			};

			return Results.Problem(problemDetails);
		}

		var validator = this.serviceProvider.GetRequiredService<IValidator<T>>();
		var validationResult = await validator.ValidateAsync(this.request!, this.cancelationToken);

		if (!validationResult.IsValid)
		{
			return Results.ValidationProblem(
				validationResult!.GetValidationSummary()
			);
		}

		var accountRepository = this.serviceProvider.GetRequiredService<IAccountRepository>();
		var accountResult = await accountRepository.GetByIdAsync(parsedAccountId);

		var result = accountResult.Match(
			account => handleRequest(this.request!, account!),
			notFound => Task.FromResult<IResult>(Results.NotFound())
		);
		return await result;
	}


	private static ProblemDetails GetProblemDetails(Exception ex)
	{
		if (ex is JsonException jsonEx)
		{
			return new ProblemDetails
			{
				Status = StatusCodes.Status422UnprocessableEntity,
				Title = "Malformed Json",
				Detail = jsonEx.Message,
			};
		}

		// Generic fallback error
		return new ProblemDetails
		{
			Status = StatusCodes.Status500InternalServerError,
			Title = "Internal Server Error",
			Detail = "An unexpected error occurred while processing the request.",
		};
	}

	public static async ValueTask<AccountBindingResult<T>> BindAsync(HttpContext httpContext)
	{
		try
		{
			var request = await httpContext.Request.ReadFromJsonAsync<T>();
			return new(httpContext.RequestServices, request, null, httpContext.RequestAborted);
		}
		catch (Exception ex)
		{
			return new(httpContext.RequestServices, default, ex, httpContext.RequestAborted);
		}
	}
}


