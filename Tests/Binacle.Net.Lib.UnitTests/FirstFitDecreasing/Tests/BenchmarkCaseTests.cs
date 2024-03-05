using Binacle.Net.Lib.Abstractions.Strategies;
using Binacle.Net.Lib.Tests.Data.Providers;
using Binacle.Net.Lib.Tests.Models;
using Xunit;

namespace Binacle.Net.Lib.UnitTests.FirstFitDecreasing.Tests;

public class BenchmarkCaseTests : IClassFixture<FirstFitDecreasingFixture>
{
	private FirstFitDecreasingFixture Fixture { get; }
	public BenchmarkCaseTests(FirstFitDecreasingFixture fixture)
	{
		Fixture = fixture;
	}

	[Theory]
	[ClassData(typeof(BenchmarkScalingTestsDataProvider))]
	public void Scaling_V1_5x5x5(int noOfItems, string expectedSize)
		=> RunTest(
			new Binacle.Net.Lib.Strategies.FirstFitDecreasing_v1(),
			noOfItems,
			BenchmarkScalingTestsDataProvider.GetDimensions(),
			expectedSize,
			BenchmarkScalingTestsDataProvider.BinCollectionName
			);

	[Theory]
	[ClassData(typeof(BenchmarkScalingTestsDataProvider))]
	public void Scaling_V2_5x5x5(int noOfItems, string expectedSize)
		=> RunTest(
			new Binacle.Net.Lib.Strategies.FirstFitDecreasing_v2(),
			noOfItems,
			BenchmarkScalingTestsDataProvider.GetDimensions(),
			expectedSize,
			BenchmarkScalingTestsDataProvider.BinCollectionName
			);

	private void RunTest<TStrategy>(
		TStrategy strategy,
		int noOfItems,
		Dimensions<int> dimensions,
		string expectedSize,
		string binCollectionName
		)
		where TStrategy : class, IBinFittingStrategy
	{
		var binCollection = this.Fixture.Bins[binCollectionName];

		var items = Enumerable.Range(1, noOfItems).Select(x => new TestItem(x.ToString(), dimensions)).ToList();

		var operation = strategy
		 .WithBins(binCollection)
		 .AndItems(items)
		 .Build();

		var result = operation.Execute();

		if (expectedSize != "None")
		{
			Xunit.Assert.Equal(expectedSize, result.FoundBin.ID);
		}
		else
		{
			Xunit.Assert.Null(result.FoundBin);
		}
	}
}

