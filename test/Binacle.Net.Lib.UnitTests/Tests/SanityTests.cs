using Xunit;

namespace Binacle.Net.Lib.UnitTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class SanityTests : IClassFixture<SanityFixture>
{
	private SanityFixture Fixture { get; }

	public SanityTests(SanityFixture fixture)
	{
		Fixture = fixture;
	}

	[Fact]
	public void Tests_Work()
	{
		Assert.True(true);
	}

	[Fact]
	public void Bin_Collections_Configured()
	{
		Assert.NotNull(Fixture);
		Assert.NotNull(Fixture.BinCollectionsTestDataProvider.Collections);

		foreach (var (key, binCollection) in Fixture.BinCollectionsTestDataProvider.Collections)
		{
			Assert.NotEmpty(binCollection);
		}
	}

	[Fact]
	public void Scenarios_Configured_Correctly()
	{
		Assert.NotNull(Fixture);

		Assert_Scenarios_Are_ConfiguredCorrectly(Fixture.BinCollectionsTestDataProvider, Fixture.ScenarioCollectionsTestDataProvider);
	}

	private static void Assert_Scenarios_Are_ConfiguredCorrectly(
		TestsKernel.Providers.BinCollectionsTestDataProvider binCollectionsTestDataProvider,
		TestsKernel.Providers.ScenarioCollectionsTestDataProvider scenarioCollectionsTestDataProvider)
	{
		var scenarioCollections = scenarioCollectionsTestDataProvider.Collections;
		var bins = binCollectionsTestDataProvider.Collections;

		Assert.NotNull(scenarioCollections);
		Assert.True(scenarioCollections.Count > 0);

		Assert.NotNull(bins);
		Assert.True(bins.Count > 0);

		foreach (var (scenarioCollectionName, scenarios) in scenarioCollections)
		{
			foreach (var scenario in scenarios)
			{
				var bin = scenario.GetTestBin(binCollectionsTestDataProvider);
				Assert.NotNull(bin);

			}
		}
	}
}
