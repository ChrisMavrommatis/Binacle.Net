using Binacle.Net.Configuration;

namespace Binacle.Net.UnitTests;

// Presets.json is not optional and every preset endpoint reads from it, so a bad entry here is a broken route.
// The messages have to name the preset and the bin, because the property path only carries indexes.
[Trait("Behavioral Tests", "Ensures bin presets are validated as expected")]
public class BinPresetOptionsValidatorTests
{
	private readonly BinPresetOptionsOptionsValidator validator = new();

	private static BinOption Bin(string id, int length = 10, int width = 10, int height = 10)
		=> new() { ID = id, Length = length, Width = width, Height = height };

	private static BinPresetOptions PresetWith(string name, params BinOption[] bins)
		=> new() { Presets = new Dictionary<string, BinPresetOption> { [name] = new() { Bins = [.. bins] } } };

	[Fact]
	public void A_Preset_With_Usable_Bins_Is_Valid()
	{
		var smallBin = Bin("small");
		var largeBin = Bin("large");
		var preset = PresetWith("demo", smallBin, largeBin);
		this.validator.Validate(preset).IsValid.ShouldBeTrue();
	}

	[Fact]
	public void No_Presets_At_All_Is_Valid()
	{
		this.validator.Validate(new BinPresetOptions()).IsValid.ShouldBeTrue();
	}

	[Fact]
	public void An_Empty_Preset_Fails_And_The_Message_Names_It()
	{
		var preset = PresetWith("demo");
		var result = this.validator.Validate(preset);

		result.IsValid.ShouldBeFalse();
		result.Errors.ShouldContain(error => error.ErrorMessage.Contains("'demo'"));
	}

	// Two bins sharing an ID makes one unreachable: {preset}/{bin} resolves to the first match and the second is
	// dead config that looks fine.
	[Fact]
	public void Duplicate_Bin_Ids_Fail_And_The_Message_Names_The_Preset_And_The_Id()
	{
		var sameBin1 = Bin("same");
		var sameBin2 = Bin("same", 20, 20, 20);
		var preset = PresetWith("demo", sameBin1, sameBin2);
		var result = this.validator.Validate(preset);

		result.IsValid.ShouldBeFalse();
		var message = result.Errors.Single().ErrorMessage;
		message.ShouldContain("'demo'");
		message.ShouldContain("'same'");
	}

	[Fact]
	public void A_Bin_Without_An_Id_Fails_Validation()
	{
		var bin = Bin("");
		var preset = PresetWith("demo", bin);
		this.validator.Validate(preset).IsValid.ShouldBeFalse();
	}

	[Theory]
	[InlineData(0, 10, 10)]
	[InlineData(10, 0, 10)]
	[InlineData(10, 10, 0)]
	[InlineData(-1, 10, 10)]
	public void A_Bin_Dimension_Of_Zero_Or_Less_Fails_Validation(int length, int width, int height)
	{
		var bin = Bin("small", length, width, height);
		var preset = PresetWith("demo", bin);
		var result = this.validator.Validate(preset);

		result.IsValid.ShouldBeFalse();
	}

	[Fact]
	public void A_Bad_Dimension_Message_Names_The_Bin_And_The_Value()
	{
		var bin = Bin("small", length: 0);
		var preset = PresetWith("demo", bin);
		var result = this.validator.Validate(preset);

		var message = result.Errors.Single().ErrorMessage;
		message.ShouldContain("'small'");
		message.ShouldContain("Length");
		message.ShouldContain("0");
	}
}
