using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.UnitTests.FirstFitDecreasing;

public sealed class SanityFixture : IDisposable
{
	private readonly BinCollectionsTestDataProvider binCollectionsTestDataProvider;
	private readonly ScenarioCollectionsTestDataProvider scenarioCollectionsTestDataProvider;
	private readonly CompactScenarioCollectionsTestDataProvider compactScenarioCollectionsTestDataProvider;

	public Dictionary<string, List<TestBin>> Bins => this.binCollectionsTestDataProvider.Collections;
	public Dictionary<string, List<Scenario>> CompactScenarios => this.compactScenarioCollectionsTestDataProvider.Collections;
	public Dictionary<string, List<Scenario>> NormalScenarios => this.scenarioCollectionsTestDataProvider.Collections;

	public SanityFixture()
	{
		this.binCollectionsTestDataProvider = new BinCollectionsTestDataProvider(Data.Constants.SolutionRootBasePath);
		this.scenarioCollectionsTestDataProvider = new ScenarioCollectionsTestDataProvider(Data.Constants.SolutionRootBasePath);
		this.compactScenarioCollectionsTestDataProvider = new CompactScenarioCollectionsTestDataProvider(Data.Constants.SolutionRootBasePath);
	}

	public void Dispose()
	{

	}
}
