using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.Kernel.Endpoints;

public record struct ValidatedBindingResult<T>(
	T? Value,
	Exception? Exception,
	ValidationResult? ValidationResult
)
{
	public static async ValueTask<ValidatedBindingResult<T>> BindAsync(HttpContext httpContext)
	{
		try
		{
			var item = await httpContext.Request.ReadFromJsonAsync<T>();
			var validator = httpContext.RequestServices.GetService<IValidator<T>>();
			if (validator is null)
			{
				return new(item, null, null);
			}

			var validationResult = await validator.ValidateAsync(item!, httpContext.RequestAborted);
			return new(item, null, validationResult);
		}
		catch (Exception ex)
		{
			return new(default, ex, null);
		}
	}
}
