
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
		true.ShouldBe(true);
	}

	[Fact]
	public void Bin_Collections_Configured()
	{
		Fixture.ShouldNotBeNull();
		Fixture.BinCollectionsTestDataProvider.Collections.ShouldNotBeEmpty();

		foreach (var (key, binCollection) in Fixture.BinCollectionsTestDataProvider.Collections)
		{
			binCollection.ShouldNotBeEmpty();
		}
	}

	[Fact]
	public void Scenarios_Configured_Correctly()
	{
		Fixture.ShouldNotBeNull();

		Assert_Scenarios_Are_ConfiguredCorrectly(
			Fixture.BinCollectionsTestDataProvider, 
			Fixture.ScenarioCollectionsTestDataProvider
		);
	}

	private static void Assert_Scenarios_Are_ConfiguredCorrectly(
		TestsKernel.Providers.BinCollectionsTestDataProvider binCollectionsTestDataProvider,
		TestsKernel.Providers.ScenarioCollectionsTestDataProvider scenarioCollectionsTestDataProvider)
	{
		var scenarioCollections = scenarioCollectionsTestDataProvider.Collections;
		var bins = binCollectionsTestDataProvider.Collections;


		scenarioCollections.ShouldNotBeEmpty();
		bins.ShouldNotBeEmpty();

		foreach (var (scenarioCollectionName, scenarios) in scenarioCollections)
		{
			foreach (var scenario in scenarios)
			{
				var bin = scenario.GetTestBin(binCollectionsTestDataProvider);
				bin.ShouldNotBeNull();
			}
		}
	}
}
