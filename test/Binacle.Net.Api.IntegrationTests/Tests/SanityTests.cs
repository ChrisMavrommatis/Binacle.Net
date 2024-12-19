using Binacle.Net.Api.Configuration.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

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
		Assert.True(true);
	}

	[Fact]
	public void Presets_Are_Configured_Correctly()
	{
		var presetOptions = sut.Services.GetRequiredService<IOptions<BinPresetOptions>>();
		presetOptions.Value.Presets.Should().NotBeNullOrEmpty();
		presetOptions.Value.Presets.Should().HaveCount(2);
		presetOptions.Value.Presets.Should().ContainKey("rectangular-cuboids");
		presetOptions.Value.Presets.Should().ContainKey("special");
	}
}
