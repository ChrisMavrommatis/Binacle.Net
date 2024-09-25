using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Fitting;
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

	// Fitting V1

	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.BaselineScenarioTestDataProvider))]
	public void Fitting_Baseline_v1(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v1, scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.SimpleScenarioTestDataProvider))]
	public void Fitting_Simple_v1(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v1, scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.ComplexScenarioTestDataProvider))]
	public void Fitting_Complex_v1(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v1, scenario);


	// Fitting V2


	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.BaselineScenarioTestDataProvider))]
	public void Fitting_Baseline_v2(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v2, scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.SimpleScenarioTestDataProvider))]
	public void Fitting_Simple_v2(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v2, scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.ComplexScenarioTestDataProvider))]
	public void Fitting_Complex_v2(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v2, scenario);


	// Fitting V3

	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.BaselineScenarioTestDataProvider))]
	public void Fitting_Baseline_v3(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v3, scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.SimpleScenarioTestDataProvider))]
	public void Fitting_Simple_v3(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v3, scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.ComplexScenarioTestDataProvider))]
	public void Fitting_Complex_v3(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v3, scenario);

	// Fitting V4

	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.BaselineScenarioTestDataProvider))]
	public void Fitting_Baseline_v4(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v4, scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.SimpleScenarioTestDataProvider))]
	public void Fitting_Simple_v4(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v4, scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.ComplexScenarioTestDataProvider))]
	public void Fitting_Complex_v4(Scenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v4, scenario);

	private void RunFittingScenarioTest<TAlgorithm>(
		Func<TestBin, IEnumerable<TestItem>, TAlgorithm> algorithmFactory,
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

		if (scenario.Fits)
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
	// Packing V1

	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.BaselineScenarioTestDataProvider))]
	public void Packing_Baseline_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_FFD_v1, scenario);


	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.SimpleScenarioTestDataProvider))]
	public void Packing_Simple_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_FFD_v1, scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.ComplexScenarioTestDataProvider))]
	public void Packing_Complex_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_FFD_v1, scenario);


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

		if (scenario.Fits)
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
