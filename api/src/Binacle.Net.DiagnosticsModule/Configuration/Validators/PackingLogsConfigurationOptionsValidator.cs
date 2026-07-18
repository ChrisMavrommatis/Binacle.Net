using Binacle.Net.DiagnosticsModule.Configuration.Models;
using FluentValidation;

namespace Binacle.Net.DiagnosticsModule.Configuration.Validators;

internal class PackingLogsConfigurationOptionsValidator : AbstractValidator<PackingLogsConfigurationOptions>
{
	public PackingLogsConfigurationOptionsValidator()
	{
		When(x => x.Enabled, () =>
		{
			RuleFor(x => x.Path).NotNull().NotEmpty();
			RuleFor(x => x.FileName)
				.NotNull()
				.NotEmpty()
				.Must(x => x!.Contains("{0}"));
			RuleFor(x => x.DateFormat)
				.NotNull()
				.NotEmpty()
				.Must(x => BeValidDateFormat(x!));
			// Null is valid (no retention); any set value must be a positive number of days.
			RuleFor(x => x.RetentionDays)
				.Must(days => days is null or > 0)
				.WithMessage("RetentionDays must be greater than 0 when set.");
		});
	}

	private bool BeValidDateFormat(string dateFormat)
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
