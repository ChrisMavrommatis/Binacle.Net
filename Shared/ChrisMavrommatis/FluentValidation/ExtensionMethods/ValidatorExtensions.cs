using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FluentValidation;

public static class ValidatorExtensions
{
	public static async Task<ValidationResult> ValidateAndAddToModelStateAsync<TModel>(
		this IValidator<TModel> validator,
		TModel model,
		ModelStateDictionary modelState,
		CancellationToken cancellationToken = default(CancellationToken))
	{
		var validationResult = await validator.ValidateAsync(model, cancellationToken);
		if (!validationResult.IsValid)
		{
			foreach (var error in validationResult.Errors)
			{
				modelState.AddModelError(error.PropertyName, error.ErrorMessage);
			}
		}
		return validationResult;
	}
}
