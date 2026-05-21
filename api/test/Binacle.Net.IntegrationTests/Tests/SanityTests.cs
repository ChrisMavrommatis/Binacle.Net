using Binacle.Net.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.IntegrationTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class SanityTests
{
	private readonly BinacleApi sut;

	public SanityTests(BinacleApi sut)
	{
		this.sut = sut;
	}

	[Fact]
	public void Tests_Work()
	{
		true.ShouldBe(true);
	}

	[Fact]
	public void Presets_Are_Configured_Correctly()
	{
		var presetOptions = sut.Services.GetRequiredService<IOptions<BinPresetOptions>>();
		presetOptions.Value.Presets.ShouldNotBeEmpty();
		presetOptions.Value.Presets.ShouldHaveCount(3);
		presetOptions.Value.Presets.ShouldContainKey(PresetKeys.CustomProblems);
		presetOptions.Value.Presets.ShouldContainKey(PresetKeys.BiscoffSuite);
		presetOptions.Value.Presets.ShouldContainKey(PresetKeys.SpecialSet);
	}
}
