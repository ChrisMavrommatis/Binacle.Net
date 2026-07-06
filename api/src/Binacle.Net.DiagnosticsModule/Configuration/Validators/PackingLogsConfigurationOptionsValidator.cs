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
