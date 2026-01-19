using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.UnitTests.Data.Providers.Benchmarks;
using Binacle.Net.TestsKernel.Models;

#pragma warning disable xUnit1007 

namespace Binacle.Lib.UnitTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class MultipleBinsBenchmarkTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }

	public MultipleBinsBenchmarkTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
	}

	[Theory]
	[ClassData(typeof(MultipleBinsBenchmarksesProvider))]
	public void Fitting(string algorithm, Scenario scenario)
		=> this.RunScenarioTest(algorithm, scenario, AlgorithmOperation.Fitting);

	[Theory]
	[ClassData(typeof(MultipleBinsBenchmarksesProvider))]
	public void Packing(string algorithm, Scenario scenario)
		=> this.RunScenarioTest(algorithm, scenario, AlgorithmOperation.Packing);

	private void RunScenarioTest(
		string algorithmKey,
		Scenario scenario,
		AlgorithmOperation operation
	)
	{
		var algorithmFactory = this.Fixture.AlgorithmsUnderTest[algorithmKey];
		var bin = scenario.GetTestBin(this.Fixture.BinDataProvider);

		var algorithmInstance = algorithmFactory(bin, scenario.Items);

		var result = algorithmInstance.Execute(new OperationParameters
		{
			Operation = operation
		});

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();

		if (scenarioResult.Fits)
		{
			result.Status.ShouldBe(OperationResultStatus.FullyPacked);
		}
		else
		{
			result.Status.ShouldNotBe(OperationResultStatus.FullyPacked);
		}
	}

}
