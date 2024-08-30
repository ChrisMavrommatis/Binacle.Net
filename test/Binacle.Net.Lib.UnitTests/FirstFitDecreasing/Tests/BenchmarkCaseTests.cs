using Binacle.Net.Lib.Abstractions.Strategies;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;
using Xunit;

namespace Binacle.Net.Lib.UnitTests.FirstFitDecreasing.Tests;

[Trait("Benchmark Case Tests", "Ensures algorithms behave as expected for the benchmarks")]
public class BenchmarkCaseTests : IClassFixture<FirstFitDecreasingFixture>
{
	private FirstFitDecreasingFixture Fixture { get; }
	public BenchmarkCaseTests(FirstFitDecreasingFixture fixture)
	{
		Fixture = fixture;
	}

	[Theory]
	[ClassData(typeof(BenchmarkScalingTestsDataProvider))]
	public void Scaling_V1_5x5x5(BenchmarkScalingScenario scenario)
		=> RunTest(
			new Binacle.Net.Lib.Strategies.FirstFitDecreasing_v1(),
			scenario,
			BenchmarkScalingTestsDataProvider.GetDimensions(),
			BenchmarkScalingTestsDataProvider.BinCollectionName
			);

	[Theory]
	[ClassData(typeof(BenchmarkScalingTestsDataProvider))]
	public void Scaling_V2_5x5x5(BenchmarkScalingScenario scenario)
		=> RunTest(
			new Binacle.Net.Lib.Strategies.FirstFitDecreasing_v2(),
			scenario,
			BenchmarkScalingTestsDataProvider.GetDimensions(),
			BenchmarkScalingTestsDataProvider.BinCollectionName
			);
	[Theory]
	[ClassData(typeof(BenchmarkScalingTestsDataProvider))]
	public void Scaling_V3_5x5x5(BenchmarkScalingScenario scenario)
	{
		var dimensions = BenchmarkScalingTestsDataProvider.GetDimensions();
		var binCollection = this.Fixture.Bins[BenchmarkScalingTestsDataProvider.BinCollectionName];

		var items = new List<TestItem>()
		{
			new TestItem("5x5x5", dimensions, scenario.NoOfItems)
		};
		var expectedBin = scenario.ExpectedSize != "None" ? binCollection.FirstOrDefault(x => x.ID == scenario.ExpectedSize) : binCollection.Last();
		var algorithm = new Binacle.Net.Lib.Algorithms.FirstFitDecreasing_v3<TestBin, TestItem>(expectedBin, items);
		var result = algorithm.Execute();
		if (scenario.ExpectedSize != "None")
		{
			Xunit.Assert.Equal(BinPackingResultStatus.FullyPacked, result.Status);
		}
		else
		{
			BinPackingResultStatus[] expectedResults = [BinPackingResultStatus.PartiallyPacked, BinPackingResultStatus.NotPacked];

			Xunit.Assert.True(expectedResults.Contains(result.Status));
		}
	}


	private void RunTest<TStrategy>(
		TStrategy strategy,
		BenchmarkScalingScenario scenario,
		Dimensions dimensions,
		string binCollectionName
		)
		where TStrategy : class, IBinFittingStrategy
	{
		var binCollection = this.Fixture.Bins[binCollectionName];

		var items = Enumerable.Range(1, scenario.NoOfItems).Select(x => new TestItem(x.ToString(), dimensions)).ToList();

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

