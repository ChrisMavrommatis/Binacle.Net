using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.Kernel.Endpoints;

public class BindingResult<T>
{
	private readonly IServiceProvider serviceProvider;
	private readonly T? request;
	private readonly Exception? exception;
	private readonly CancellationToken cancelationToken;

	private BindingResult(
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

	public async Task<IResult> ValidateAsync(Func<T, Task<IResult>> handleValidRequest)
	{
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
				Title = "Malformed Request",
				Detail = "The server could not process the request because it is malformed or contains invalid data. Please verify the request format and try again.",
			};

			return Results.Problem(problemDetails);
		}

		var validator = this.serviceProvider.GetRequiredService<IValidator<T>>();
		var validationResult = await validator.ValidateAsync(this.request!, this.cancelationToken);

		if (!validationResult.IsValid)
		{
			return Results.ValidationProblem(
				validationResult!.GetValidationSummary(),
				statusCode: StatusCodes.Status422UnprocessableEntity
			);
		}

		return await handleValidRequest(this.request!);
	}


	private static ProblemDetails GetProblemDetails(Exception ex)
	{
		if (ex is JsonException jsonEx)
		{
			return new ProblemDetails
			{
				Status = StatusCodes.Status400BadRequest,
				Title = "Invalid JSON Format",
				Detail = jsonEx.Message,
			};
		}

		// Generic fallback error
		var problemDetails = new ProblemDetails
		{
			Status = StatusCodes.Status500InternalServerError,
			Title = "Unexpected Server Error",
			Detail = "An unexpected error occurred while processing your request. Please try again later or contact support.",
		};
		
		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			problemDetails.Extensions.TryAdd("exception", ex.GetType().Name);
			problemDetails.Extensions.TryAdd("message", ex.Message);
			problemDetails.Extensions.TryAdd("stackTrace", ex.StackTrace);
		}

		return problemDetails;
	}

	public static async ValueTask<BindingResult<T>> BindAsync(HttpContext httpContext)
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
