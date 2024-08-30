using Binacle.Net.Lib.Abstractions.Strategies;
using Binacle.Net.Lib.Algorithms;
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
		=> this.RunScenarioTest(new Binacle.Net.Lib.Strategies.FirstFitDecreasing_v1(), scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.SimpleScenarioTestDataProvider))]
	public void Simple_v1(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Strategies.FirstFitDecreasing_v1(), scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.ComplexScenarioTestDataProvider))]
	public void Complex_v1(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Strategies.FirstFitDecreasing_v1(), scenario);


	// V2
	[Theory]
	[ClassData(typeof(Data.Providers.BaselineScenarioTestDataProvider))]
	public void Baseline_v2(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Strategies.FirstFitDecreasing_v2(), scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.SimpleScenarioTestDataProvider))]
	public void Simple_v2(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Strategies.FirstFitDecreasing_v2(), scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.ComplexScenarioTestDataProvider))]
	public void Complex_v2(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Strategies.FirstFitDecreasing_v2(), scenario);

	// V3
	[Theory]
	[ClassData(typeof(Data.Providers.BaselineScenarioTestDataProvider))]
	public void Baseline_v3(Scenario scenario)
	{
		var binCollection = this.Fixture.Bins[scenario.BinCollection];

		var expectedBin = scenario.ExpectedSize != "None" ? binCollection.FirstOrDefault(x => x.ID == scenario.ExpectedSize) : binCollection.Last();

		var algorithm = new Binacle.Net.Lib.Algorithms.FirstFitDecreasing_v3<TestBin, TestItem>(expectedBin!, scenario.Items);
		this.RunAlgorithmScenario(algorithm, scenario);
	}

	[Theory]
	[ClassData(typeof(Data.Providers.SimpleScenarioTestDataProvider))]
	public void Simple_v3(Scenario scenario)
	{
		var binCollection = this.Fixture.Bins[scenario.BinCollection];

		var expectedBin = scenario.ExpectedSize != "None" ? binCollection.FirstOrDefault(x => x.ID == scenario.ExpectedSize) : binCollection.Last();

		var algorithm = new Binacle.Net.Lib.Algorithms.FirstFitDecreasing_v3<TestBin, TestItem>(expectedBin!, scenario.Items);
		this.RunAlgorithmScenario(algorithm, scenario);
	}

	[Theory]
	[ClassData(typeof(Data.Providers.ComplexScenarioTestDataProvider))]
	public void Complex_v3(Scenario scenario)
	{
		var binCollection = this.Fixture.Bins[scenario.BinCollection];

		var expectedBin = scenario.ExpectedSize != "None" ? binCollection.FirstOrDefault(x => x.ID == scenario.ExpectedSize) : binCollection.Last();

		var algorithm = new Binacle.Net.Lib.Algorithms.FirstFitDecreasing_v3<TestBin, TestItem>(expectedBin!, scenario.Items);
		this.RunAlgorithmScenario(algorithm, scenario);
	}

	private void RunAlgorithmScenario(FirstFitDecreasing_v3<TestBin, TestItem> algorithm, Scenario scenario)
	{
		var result = algorithm.Execute();
		if(scenario.ExpectedSize != "None")
		{
			Xunit.Assert.Equal(BinPackingResultStatus.FullyPacked, result.Status);
		}
		else
		{
			BinPackingResultStatus[] expectedResults = [BinPackingResultStatus.PartiallyPacked, BinPackingResultStatus.NotPacked];
			
			Xunit.Assert.True(expectedResults.Contains(result.Status));
		}
	}

	private void RunScenarioTest<TStrategy>(TStrategy strategy, Scenario scenario)
		where TStrategy : class, IBinFittingStrategy
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

