using Binacle.Net.TestsKernel.Data.Providers;

namespace Binacle.Lib.UnitTests;

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
		true.ShouldBe(true);
	}

	[Fact]
	public void Bin_Collections_Configured()
	{
		Fixture.ShouldNotBeNull();
		Fixture.BinCollectionsDataProvider.Collections.ShouldNotBeEmpty();

		foreach (var (key, binCollection) in Fixture.BinCollectionsDataProvider.Collections)
		{
			binCollection.ShouldNotBeEmpty();
		}
	}

	[Fact]
	public void Scenarios_Configured_Correctly()
	{
		Fixture.ShouldNotBeNull();

		Assert_Scenarios_Are_ConfiguredCorrectly(
			Fixture.BinCollectionsDataProvider, 
			Fixture.ScenarioCollectionsDataProvider
		);
	}

	private static void Assert_Scenarios_Are_ConfiguredCorrectly(
		BinCollectionsDataProvider binCollectionsDataProvider,
		ScenarioCollectionsDataProvider scenarioCollectionsDataProvider)
	{
		var scenarioCollections = scenarioCollectionsDataProvider.Collections;
		var bins = binCollectionsDataProvider.Collections;


		scenarioCollections.ShouldNotBeEmpty();
		bins.ShouldNotBeEmpty();

		foreach (var (scenarioCollectionName, scenarios) in scenarioCollections)
		{
			foreach (var scenario in scenarios)
			{
				var bin = scenario.GetTestBin(binCollectionsDataProvider);
				bin.ShouldNotBeNull();
			}
		}
	}
}
