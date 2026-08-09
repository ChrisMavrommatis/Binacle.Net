using Binacle.Net.DiagnosticsModule.Configuration.Models;
using FluentValidation;

namespace Binacle.Net.DiagnosticsModule.Configuration.Validators;

internal class PackingLogsConfigurationOptionsValidator : AbstractValidator<PackingLogsConfigurationOptions>
{
	public PackingLogsConfigurationOptionsValidator()
	{
		When(x => x.Enabled, () =>
		{
			// Cascade(Stop) is load-bearing, not tidying. Without it every rule in a chain runs even after one
			// fails, so a null reported three times (NotNull and NotEmpty share a message) and the predicate ran
			// on the null and crashed the start it exists to explain. Stopping at the first failure gives one
			// message per setting and lets each predicate assume a value.
			RuleFor(x => x.Path)
				.Cascade(CascadeMode.Stop)
				.NotEmpty();
			RuleFor(x => x.FileName)
				.Cascade(CascadeMode.Stop)
				.NotEmpty()
				.Must(fileName => fileName!.Contains("{0}"))
				.WithMessage("'{PropertyName}' must contain the date placeholder, for example 'packing-logs-{0}.log'.");
			RuleFor(x => x.DateFormat)
				.Cascade(CascadeMode.Stop)
				.NotEmpty()
				.Must(x => BeValidDateFormat(x!))
				.WithMessage("'{PropertyName}' is not a usable date format, for example 'yyyy-MM-dd'.");
			// Null is valid (no retention); any set value must be a positive number of days.
			RuleFor(x => x.RetentionDays)
				.Must(days => days is null or > 0)
				.WithMessage("'{PropertyName}' must be greater than 0 when set. Leave it unset to keep every file.");
		});
	}

	private static bool BeValidDateFormat(string dateFormat)
	{
		try
		{
			// Try formatting a sample date using the provided format
			var testDate = DateTime.UtcNow.ToString(dateFormat);
			return !string.IsNullOrEmpty(testDate);
		}
		catch (FormatException)
		{
			return false; // If it throws, it's an invalid format
		}
	}
}
