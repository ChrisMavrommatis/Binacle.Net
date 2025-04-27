using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Services;

internal class InternalServerErrorExceptionHandler : IExceptionHandler
{
	private readonly IProblemDetailsService problemDetailsService;

	public InternalServerErrorExceptionHandler(
		IProblemDetailsService problemDetailsService
		)
	{
		this.problemDetailsService = problemDetailsService;
	}
	
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext, 
		Exception exception,
		CancellationToken cancellationToken
		)
	{
		
		var problemDetails = new ProblemDetails
		{
			Status = StatusCodes.Status500InternalServerError,
			Title = "Internal Server Error",
			Detail = "An unexpected error occurred.",
			Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
		};

		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			problemDetails.Extensions.TryAdd("exception", exception.GetType().Name);
			problemDetails.Extensions.TryAdd("message", exception.Message);
			problemDetails.Extensions.TryAdd("stackTrace", exception.StackTrace);
		}

		return await this.problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
		{
			ProblemDetails = problemDetails,
			HttpContext = httpContext,
			Exception = exception
		});
	}
}
