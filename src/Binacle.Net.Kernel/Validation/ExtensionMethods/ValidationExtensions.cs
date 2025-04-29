using FluentValidation.Results;

namespace Binacle.Net;

public static class ValidationExtensions
{
	public static Dictionary<string, string[]> GetValidationSummary(
		this ValidationResult validationResult
	)
	{
		return validationResult.Errors
			.GroupBy(x => x.PropertyName)
			.ToDictionary(
				group => group.Key,
				group => group.Select(x => x.ErrorMessage).ToArray()
			);
	}
}
