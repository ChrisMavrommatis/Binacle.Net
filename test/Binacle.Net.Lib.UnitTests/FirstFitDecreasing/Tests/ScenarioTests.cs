using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Packing.Models;
using Binacle.Net.TestsKernel.Models;
using Xunit;

namespace Binacle.Net.Lib.UnitTests.FirstFitDecreasing.Tests;

[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class ScenarioTests : IClassFixture<FirstFitDecreasingFixture>
{
	private FirstFitDecreasingFixture Fixture { get; }
	public ScenarioTests(FirstFitDecreasingFixture fixture)
	{
		Fixture = fixture;
	}

	// V1
	[Theory]
	[ClassData(typeof(Data.Providers.BaselineScenarioTestDataProvider))]
	public void Baseline_v1(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Fitting.Algorithms.FirstFitDecreasing_v1(), scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.SimpleScenarioTestDataProvider))]
	public void Simple_v1(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Fitting.Algorithms.FirstFitDecreasing_v1(), scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.ComplexScenarioTestDataProvider))]
	public void Complex_v1(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Fitting.Algorithms.FirstFitDecreasing_v1(), scenario);


	// V2
	[Theory]
	[ClassData(typeof(Data.Providers.BaselineScenarioTestDataProvider))]
	public void Baseline_v2(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Fitting.Algorithms.FirstFitDecreasing_v2(), scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.SimpleScenarioTestDataProvider))]
	public void Simple_v2(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Fitting.Algorithms.FirstFitDecreasing_v2(), scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.ComplexScenarioTestDataProvider))]
	public void Complex_v2(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Fitting.Algorithms.FirstFitDecreasing_v2(), scenario);

	// V3
	[Theory]
	[ClassData(typeof(Data.Providers.BaselineScenarioTestDataProvider))]
	public void Baseline_v3(Scenario scenario)
	{
		var binCollection = this.Fixture.Bins[scenario.BinCollection];

		var expectedBin = scenario.ExpectedSize != "None" ? binCollection.FirstOrDefault(x => x.ID == scenario.ExpectedSize) : binCollection.Last();

		var algorithm = new Binacle.Net.Lib.Packing.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(expectedBin!, scenario.Items);
		this.RunAlgorithmScenario(algorithm, scenario);
	}

	[Theory]
	[ClassData(typeof(Data.Providers.SimpleScenarioTestDataProvider))]
	public void Simple_v3(Scenario scenario)
	{
		var binCollection = this.Fixture.Bins[scenario.BinCollection];

		var expectedBin = scenario.ExpectedSize != "None" ? binCollection.FirstOrDefault(x => x.ID == scenario.ExpectedSize) : binCollection.Last();

		var algorithm = new Binacle.Net.Lib.Packing.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(expectedBin!, scenario.Items);
		this.RunAlgorithmScenario(algorithm, scenario);
	}

	[Theory]
	[ClassData(typeof(Data.Providers.ComplexScenarioTestDataProvider))]
	public void Complex_v3(Scenario scenario)
	{
		var binCollection = this.Fixture.Bins[scenario.BinCollection];

		var expectedBin = scenario.ExpectedSize != "None" ? binCollection.FirstOrDefault(x => x.ID == scenario.ExpectedSize) : binCollection.Last();

		var algorithm = new Binacle.Net.Lib.Packing.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(expectedBin!, scenario.Items);
		this.RunAlgorithmScenario(algorithm, scenario);
	}

	private void RunAlgorithmScenario<TAlgorithm>(TAlgorithm algorithm, Scenario scenario)
		where TAlgorithm : class, IPackingAlgorithm
	{
		var result = algorithm.Execute();
		if(scenario.ExpectedSize != "None")
		{
			Xunit.Assert.Equal(PackingResultStatus.FullyPacked, result.Status);
		}
		else
		{
			PackingResultStatus[] expectedResults = [PackingResultStatus.PartiallyPacked, PackingResultStatus.NotPacked];
			
			Xunit.Assert.True(expectedResults.Contains(result.Status));
		}
	}

	private void RunScenarioTest<TAlgorithm>(TAlgorithm strategy, Scenario scenario)
		where TAlgorithm : class, IFittingAlgorithm
	{
		var binCollection = this.Fixture.Bins[scenario.BinCollection];

		// flatten scenario.Items based on quantity
		var items = scenario.Items.SelectMany(x => Enumerable.Repeat(x, x.Quantity));

		var operation = strategy
		 .WithBins(binCollection)
		 .AndItems(items)
		 .Build();

		var result = operation.Execute();

		if (scenario.ExpectedSize != "None")
		{
			Xunit.Assert.Equal(scenario.ExpectedSize, result.FoundBin.ID);
		}
		else
		{
			Xunit.Assert.Null(result.FoundBin);
		}
	}
}

