using Binacle.Net.TestsKernel.Models;
using Xunit;

namespace Binacle.Net.Lib.UnitTests.FirstFitDecreasing.Tests;

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
		Dictionary<string, List<Scenario>> scenarioCollections
		)
	{

		Xunit.Assert.NotNull(scenarioCollections);
		Xunit.Assert.True(scenarioCollections.Count > 0);

		foreach (var (scenarioCollectionName, scenarios) in scenarioCollections)
		{
			foreach(var scenario in scenarios)
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
}
