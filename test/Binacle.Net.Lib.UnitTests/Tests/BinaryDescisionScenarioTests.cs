using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;
using Binacle.Net.TestsKernel.Models;
using Xunit;

namespace Binacle.Net.Lib.UnitTests;

[Trait("Scenario Tests", "Actual calculation for the algorithms.")]
public class BinaryDecisionScenarioTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }
	public BinaryDecisionScenarioTests(CommonTestingFixture fixture)
	{
		Fixture = fixture;
	}

	#region Fitting

	// Fitting FFD V1
	[Theory]
	[ClassData(typeof(BaselineScenarioTestDataProvider))]
	public void Fitting_FFD_Baseline_v1(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v1, scenario);

	[Theory]
	[ClassData(typeof(SimpleScenarioTestDataProvider))]
	public void Fitting_FFD_Simple_v1(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v1, scenario);

	[Theory]
	[ClassData(typeof(ComplexScenarioTestDataProvider))]
	public void Fitting_FFD_Complex_v1(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v1, scenario);


	// Fitting FFD V2
	[Theory]
	[ClassData(typeof(BaselineScenarioTestDataProvider))]
	public void Fitting_FFD_Baseline_v2(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v2, scenario);

	[Theory]
	[ClassData(typeof(SimpleScenarioTestDataProvider))]
	public void Fitting_FFD_Simple_v2(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v2, scenario);

	[Theory]
	[ClassData(typeof(ComplexScenarioTestDataProvider))]
	public void Fitting_FFD_Complex_v2(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v2, scenario);


	// Fitting FFD V3
	[Theory]
	[ClassData(typeof(BaselineScenarioTestDataProvider))]
	public void Fitting_FFD_Baseline_v3(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v3, scenario);

	[Theory]
	[ClassData(typeof(SimpleScenarioTestDataProvider))]
	public void Fitting_FFD_Simple_v3(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v3, scenario);

	[Theory]
	[ClassData(typeof(ComplexScenarioTestDataProvider))]
	public void Fitting_FFD_Complex_v3(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v3, scenario);


	private void RunFittingScenarioTest<TAlgorithm>(
		Func<TestBin, List<TestItem>, TAlgorithm> algorithmFactory,
		Scenario scenario
	)
		where TAlgorithm : class, IFittingAlgorithm
	{
		var bin = scenario.GetTestBin(this.Fixture.BinTestDataProvider);

		var algorithmInstance = algorithmFactory(bin, scenario.Items);

		var result = algorithmInstance.Execute(new Fitting.Models.FittingParameters
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
	// Packing FFD V1
	[Theory]
	[ClassData(typeof(BaselineScenarioTestDataProvider))]
	public void Packing_FFD_Baseline_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_FFD_v1, scenario);

	[Theory]
	[ClassData(typeof(SimpleScenarioTestDataProvider))]
	public void Packing_FFD_Simple_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_FFD_v1, scenario);

	[Theory]
	[ClassData(typeof(ComplexScenarioTestDataProvider))]
	public void Packing_FFD_Complex_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_FFD_v1, scenario);


	// Packing FFD V2
	[Theory]
	[ClassData(typeof(BaselineScenarioTestDataProvider))]
	public void Packing_FFD_Baseline_v2(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_FFD_v2, scenario);

	[Theory]
	[ClassData(typeof(SimpleScenarioTestDataProvider))]
	public void Packing_FFD_Simple_v2(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_FFD_v2, scenario);

	[Theory]
	[ClassData(typeof(ComplexScenarioTestDataProvider))]
	public void Packing_FFD_Complex_v2(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_FFD_v2, scenario);


	// Packing WFD V1
	[Theory]
	[ClassData(typeof(BaselineScenarioTestDataProvider))]
	public void Packing_WFD_Baseline_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_WFD_v1, scenario);

	[Theory]
	[ClassData(typeof(SimpleScenarioTestDataProvider))]
	public void Packing_WFD_Simple_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_WFD_v1, scenario);

	[Theory]
	[ClassData(typeof(ComplexScenarioTestDataProvider))]
	public void Packing_WFD_Complex_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_WFD_v1, scenario);

	// Packing BFD V1
	[Theory]
	[ClassData(typeof(BaselineScenarioTestDataProvider))]
	public void Packing_BFD_Baseline_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_BFD_v1, scenario);

	[Theory]
	[ClassData(typeof(SimpleScenarioTestDataProvider))]
	public void Packing_BFD_Simple_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_BFD_v1, scenario);

	[Theory]
	[ClassData(typeof(ComplexScenarioTestDataProvider))]
	public void Packing_BFD_Complex_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_BFD_v1, scenario);



	private void RunPackingScenarioTest<TAlgorithm>(
			Func<TestBin, List<TestItem>, TAlgorithm> algorithmFactory,
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
