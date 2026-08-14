using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.Configuration.Validators;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

// PackingLogs configuration was flattened in v3.0.0, so an existing deployment is the likeliest source of a
// config that no longer fits. The validator has to name the problem rather than fall over: PackingLogs.json is
// not optional and is read on every start.
[Trait("Behavioral Tests", "Ensures packing logs configuration is validated as expected")]
public class PackingLogsConfigurationOptionsValidatorTests
{
	private readonly PackingLogsConfigurationOptionsValidator validator = new();

	private static PackingLogsConfigurationOptions OptionsWith(
		string? path = "logs",
		string? fileName = "pack-{0}.log",
		string? dateFormat = "yyyy-MM-dd",
		int? retentionDays = null
	) => new()
	{
		Enabled = true,
		Path = path,
		FileName = fileName,
		DateFormat = dateFormat,
		RetentionDays = retentionDays
	};

	[Fact]
	public void A_complete_enabled_configuration_is_valid()
	{
		this.validator.Validate(OptionsWith()).IsValid.ShouldBeTrue();
	}

	// The When(Enabled) gate: nothing inside it applies while the feature is off, so an empty section is fine.
	[Fact]
	public void A_disabled_configuration_needs_nothing_else()
	{
		var options = new PackingLogsConfigurationOptions { Enabled = false };

		this.validator.Validate(options).IsValid.ShouldBeTrue();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public void A_missing_path_fails_validation(string? path)
	{
		this.validator.Validate(OptionsWith(path: path)).IsValid.ShouldBeFalse();
	}

	// FileName carries the date placeholder, so a name without {0} writes every day to one file. A missing
	// FileName once threw out of the validator instead of reporting itself.
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("pack.log")]
	public void A_file_name_without_the_date_placeholder_fails_validation(string? fileName)
	{
		this.validator.Validate(OptionsWith(fileName: fileName)).IsValid.ShouldBeFalse();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public void A_missing_date_format_fails_validation(string? dateFormat)
	{
		this.validator.Validate(OptionsWith(dateFormat: dateFormat)).IsValid.ShouldBeFalse();
	}

	[Theory]
	[InlineData("yyyy-MM-dd")]
	[InlineData("yyyyMMdd")]
	public void A_usable_date_format_is_accepted(string dateFormat)
	{
		this.validator.Validate(OptionsWith(dateFormat: dateFormat)).IsValid.ShouldBeTrue();
	}

	[Theory]
	[InlineData(null)] // no retention — keep every file
	[InlineData(1)]
	[InlineData(7)]
	public void A_null_or_positive_retention_is_accepted(int? retentionDays)
	{
		this.validator.Validate(OptionsWith(retentionDays: retentionDays)).IsValid.ShouldBeTrue();
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public void A_non_positive_retention_fails_validation(int retentionDays)
	{
		this.validator.Validate(OptionsWith(retentionDays: retentionDays)).IsValid.ShouldBeFalse();
	}

	// The setting likeliest to be wrong after the v3.0.0 flattening, so it gets one line showing a working value
	// rather than three lines of framework defaults.
	[Fact]
	public void A_missing_file_name_is_reported_once_with_an_example()
	{
		var result = this.validator.Validate(OptionsWith(fileName: "pack.log"));

		result.Errors.Count.ShouldBe(1);
		result.Errors.Single().ErrorMessage.ShouldContain("{0}");
	}

}
