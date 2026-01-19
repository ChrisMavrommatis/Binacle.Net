using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.UnitTests.Data.Providers.Benchmarks;
using Binacle.Net.TestsKernel.Models;

#pragma warning disable xUnit1007


namespace Binacle.Lib.UnitTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class CubeScalingBenchmarksCaseTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }

	public CubeScalingBenchmarksCaseTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
	}

	[Theory]
	[ClassData(typeof(CubeScalingBenchmarksProvider))]
	public void Fitting_Algorithms(string algorithm, CubeScalingBenchmarkScenario scenario)
		=> this.RunScenarioTest(algorithm, scenario, AlgorithmOperation.Fitting);

	[Theory]
	[ClassData(typeof(CubeScalingBenchmarksProvider))]
	public void Packing(string algorithm, CubeScalingBenchmarkScenario scenario)
		=> this.RunScenarioTest(algorithm, scenario, AlgorithmOperation.Packing);

	private void RunScenarioTest(
		string algorithmKey,
		CubeScalingBenchmarkScenario scenario,
		AlgorithmOperation operation
	)
	{
		var algorithmFactory = this.Fixture.AlgorithmsUnderTest[algorithmKey];
		var bin = scenario.GetTestBin(this.Fixture.BinDataProvider);
		foreach (var noOfItems in scenario.GetNoOfItems())
		{
			var items = scenario.GetTestItems(noOfItems);
			var algorithmInstance = algorithmFactory(bin, items);

			var result = algorithmInstance.Execute(new OperationParameters()
			{
				Operation = operation
			});

			if (scenario.MaxInRange < noOfItems) // doesn't fit
			{
				result.Status.ShouldNotBe(OperationResultStatus.FullyPacked);
			}
			else
			{
				result.Status.ShouldBe(OperationResultStatus.FullyPacked);
			}
		}
	}
}
