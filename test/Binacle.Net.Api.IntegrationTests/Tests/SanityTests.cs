using Binacle.Net.Api.Configuration.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Api.IntegrationTests;

[Collection(BinacleApiCollection.Name)]
[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class SanityTests
{
	private readonly BinacleApiFactory sut;

	public SanityTests(BinacleApiFactory sut)
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
		presetOptions.Value.Presets.ShouldHaveCount(2);
		presetOptions.Value.Presets.ShouldContainKey("rectangular-cuboids");
		presetOptions.Value.Presets.ShouldContainKey("special");
	}
}
