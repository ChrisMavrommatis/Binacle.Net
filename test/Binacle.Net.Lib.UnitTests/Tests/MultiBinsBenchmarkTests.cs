using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;
using Xunit;

namespace Binacle.Net.Lib.UnitTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class MultiBinsBenchmarkTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }

	public MultiBinsBenchmarkTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
	}

	#region Fitting

	[Theory]
	[ClassData(typeof(MultiBinsBenchmarkTestsDataProvider))]
	public void Fitting_FFD_v1(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v1, scenario);

	[Theory]
	[ClassData(typeof(MultiBinsBenchmarkTestsDataProvider))]
	public void Fitting_FFD_v2(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v2, scenario);

	[Theory]
	[ClassData(typeof(MultiBinsBenchmarkTestsDataProvider))]
	public void Fitting_FFD_v3(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v3, scenario);

	private void RunFittingScenarioTest<TAlgorithm>(
		AlgorithmFactory<TAlgorithm> algorithmFactory,
		Scenario scenario
	)
		where TAlgorithm : class, IFittingAlgorithm
	{
		var bin = scenario.GetTestBin(this.Fixture.BinTestDataProvider);

		var algorithmInstance = algorithmFactory(bin, scenario.Items);

		var result = algorithmInstance.Execute(new FittingParameters()
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();

		if (scenarioResult.Fits)
		{
			Xunit.Assert.Equal(Fitting.Models.FittingResultStatus.Success, result.Status);
		}
		else
		{
			Xunit.Assert.Equal(Fitting.Models.FittingResultStatus.Fail, result.Status);
		}
	}

	#endregion

	#region Packing

	[Theory]
	[ClassData(typeof(MultiBinsBenchmarkTestsDataProvider))]
	public void Packing_FFD_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_FFD_v1, scenario);

	[Theory]
	[ClassData(typeof(MultiBinsBenchmarkTestsDataProvider))]
	public void Packing_FFD_v2(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_FFD_v2, scenario);

	[Theory]
	[ClassData(typeof(MultiBinsBenchmarkTestsDataProvider))]
	public void Packing_WFD_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_WFD_v1, scenario);

	[Theory]
	[ClassData(typeof(MultiBinsBenchmarkTestsDataProvider))]
	public void Packing_BFD_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_BFD_v1, scenario);


	private void RunPackingScenarioTest<TAlgorithm>(
		AlgorithmFactory<TAlgorithm> algorithmFactory,
		Scenario scenario
	)
		where TAlgorithm : class, IPackingAlgorithm
	{
		var bin = scenario.GetTestBin(this.Fixture.BinTestDataProvider);

		var algorithmInstance = algorithmFactory(bin, scenario.Items);

		var result = algorithmInstance.Execute(new Packing.Models.PackingParameters
		{
			NeverReportUnpackedItems = true,
			ReportPackedItemsOnlyWhenFullyPacked = true,
			OptInToEarlyFails = true
		});

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();

		if (scenarioResult.Fits)
		{
			Xunit.Assert.Equal(Packing.Models.PackingResultStatus.FullyPacked, result.Status);
		}
		else
		{
			Xunit.Assert.NotEqual(Packing.Models.PackingResultStatus.FullyPacked, result.Status);
		}
	}

	#endregion
}
