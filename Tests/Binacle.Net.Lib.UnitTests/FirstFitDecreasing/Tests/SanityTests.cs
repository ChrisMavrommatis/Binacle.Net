using Binacle.Net.Lib.Tests.Models;
using Binacle.Net.Lib.UnitTests.Data.Models;
using Xunit;

namespace Binacle.Net.Lib.UnitTests.FirstFitDecreasing;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class SanityTests : IClassFixture<SanityFixture>
{
	private SanityFixture Fixture { get; }

	public SanityTests(SanityFixture fixture)
	{
		this.Fixture = fixture;
	}

	[Fact]
	public void Tests_Work()
	{
		Xunit.Assert.True(true);
	}

	[Fact]
	public void Bin_Collections_Configured()
	{
		Xunit.Assert.NotNull(this.Fixture);
		Xunit.Assert.NotNull(this.Fixture.Bins);

		foreach (var binCollection in this.Fixture.Bins.Values)
		{
			Xunit.Assert.NotEmpty(binCollection);
		}
	}

	[Fact]
	public void Scenarios_Configured_Correctly()
	{
		Xunit.Assert.NotNull(this.Fixture);

		Assert_Scenarios_Are_ConfiguredCorrectly(this.Fixture.Bins, this.Fixture.NormalScenarios);
		Assert_Scenarios_Are_ConfiguredCorrectly(this.Fixture.Bins, this.Fixture.CompactScenarios);
	}

	private static void Assert_Scenarios_Are_ConfiguredCorrectly(
		Dictionary<string, List<TestBin>> bins,
		Dictionary<string, Scenario> scenarios
		)
	{

		Xunit.Assert.NotNull(scenarios);
		Xunit.Assert.True(scenarios.Count > 0);

		foreach (var scenario in scenarios.Values)
		{
			var scenarioExpectedSize = scenario.ExpectedSize;
			bins.TryGetValue(scenario.BinCollection, out var binCollection);
			Xunit.Assert.NotNull(binCollection);
			if (scenarioExpectedSize != "None")
			{
				var expectedSize = binCollection.FirstOrDefault(x => x.ID == scenarioExpectedSize);
				Xunit.Assert.NotNull(expectedSize);
			}
		}
	}


}
