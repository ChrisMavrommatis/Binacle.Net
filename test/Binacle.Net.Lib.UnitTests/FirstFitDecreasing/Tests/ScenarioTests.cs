using Binacle.Net.Lib.Abstractions.Strategies;
using Binacle.Net.Lib.UnitTests.Data.Models;
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
	[ClassData(typeof(Data.Providers.BaselineTestDataProvider))]
	public void Baseline_v1(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Strategies.FirstFitDecreasing_v1(), scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.SimpleTestDataProvider))]
	public void Simple_v1(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Strategies.FirstFitDecreasing_v1(), scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.ComplexTestDataProvider))]
	public void Complex_v1(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Strategies.FirstFitDecreasing_v1(), scenario);


	// V2
	[Theory]
	[ClassData(typeof(Data.Providers.BaselineTestDataProvider))]
	public void Baseline_v2(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Strategies.FirstFitDecreasing_v2(), scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.SimpleTestDataProvider))]
	public void Simple_v2(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Strategies.FirstFitDecreasing_v2(), scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.ComplexTestDataProvider))]
	public void Complex_v2(Scenario scenario)
		=> this.RunScenarioTest(new Binacle.Net.Lib.Strategies.FirstFitDecreasing_v2(), scenario);

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

