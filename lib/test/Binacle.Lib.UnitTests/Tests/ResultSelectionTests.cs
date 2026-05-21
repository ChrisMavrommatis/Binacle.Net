using Binacle.Lib.ResultSelection;
using Binacle.TestsKernel.ResultSelection.Providers;

namespace Binacle.Lib.UnitTests;

[Trait("Scenario Tests", "Actual calculation for the algorithms.")]
public class ResultSelectionTests : IClassFixture<ResultSelectionTestingFixture>
{
	private readonly ResultSelectionTestingFixture fixture;

	public ResultSelectionTests(ResultSelectionTestingFixture fixture)
	{
		this.fixture = fixture;
	}

	[Theory]
	[MemberData(nameof(BestAlgorithmScenarioProvider.ScenarioNames), MemberType = typeof(BestAlgorithmScenarioProvider))]
    public void BestAlgorithm_v1(string scenarioName)
       => this.fixture.RunTest(scenarioName, new BestAlgorithm_v1(), x=> x.AlgorithmInfo.GetAlgorithmIdentifierName());
    
	[Theory]
	[MemberData(nameof(BestAlgorithmScenarioProvider.ScenarioNames), MemberType = typeof(BestAlgorithmScenarioProvider))]
    public void BestAlgorithm_v2(string scenarioName)
       => this.fixture.RunTest(scenarioName, new BestAlgorithm_v2(), x=> x.AlgorithmInfo.GetAlgorithmIdentifierName());
    
	[Theory]
	[MemberData(nameof(BestBinScenarioProvider.ScenarioNames), MemberType = typeof(BestBinScenarioProvider))]
	public void BestBin_v1(string scenarioName)
		=> this.fixture.RunTest(scenarioName, new BestBin_v1(), x=> x.Bin.ID);
    
	[Theory]
	[MemberData(nameof(BestBinScenarioProvider.ScenarioNames), MemberType = typeof(BestBinScenarioProvider))]
	public void BestBin_v2(string scenarioName)
		=> this.fixture.RunTest(scenarioName, new BestBin_v2(), x=> x.Bin.ID);
	
	[Theory]
	[MemberData(nameof(SmallestBinScenarioProvider.ScenarioNames), MemberType = typeof(SmallestBinScenarioProvider))]
	public void SmallestBin_v1(string scenarioName)
		=> this.fixture.RunTest(scenarioName, new SmallestBin_v1(), x=> x.Bin.ID);
    
	[Theory]
	[MemberData(nameof(SmallestBinScenarioProvider.ScenarioNames), MemberType = typeof(SmallestBinScenarioProvider))]
	public void SmallestBin_v2(string scenarioName)
		=> this.fixture.RunTest(scenarioName, new SmallestBin_v2(), x=> x.Bin.ID);
}
