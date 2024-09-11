//using Binacle.Net.Lib.Abstractions.Fitting;
//using Binacle.Net.Lib.Packing.Models;
//using Binacle.Net.TestsKernel.Models;
//using Binacle.Net.TestsKernel.Providers;
//using Xunit;

//namespace Binacle.Net.Lib.UnitTests;

//[Trait("Benchmark Case Tests", "Ensures algorithms behave as expected for the benchmarks")]
//public class BenchmarkCaseTests : IClassFixture<CommonTestingFixture>
//{
//	private CommonTestingFixture Fixture { get; }
//	public BenchmarkCaseTests(CommonTestingFixture fixture)
//	{
//		Fixture = fixture;
//	}

//	[Theory]
//	[ClassData(typeof(BenchmarkScalingTestsDataProvider))]
//	public void Scaling_V1_5x5x5(BenchmarkScalingScenario scenario)
//		=> RunTest(
//			new Fitting.Algorithms.FirstFitDecreasing_v1(),
//			scenario,
//			BenchmarkScalingTestsDataProvider.GetDimensions(),
//			BenchmarkScalingTestsDataProvider.BinCollectionName
//			);

//	[Theory]
//	[ClassData(typeof(BenchmarkScalingTestsDataProvider))]
//	public void Scaling_V2_5x5x5(BenchmarkScalingScenario scenario)
//		=> RunTest(
//			new Fitting.Algorithms.FirstFitDecreasing_v2(),
//			scenario,
//			BenchmarkScalingTestsDataProvider.GetDimensions(),
//			BenchmarkScalingTestsDataProvider.BinCollectionName
//			);

//	[Theory]
//	[ClassData(typeof(BenchmarkScalingTestsDataProvider))]
//	public void Scaling_V3_5x5x5(BenchmarkScalingScenario scenario)
//	{
//		var dimensions = BenchmarkScalingTestsDataProvider.GetDimensions();
//		var binCollection = Fixture.Bins[BenchmarkScalingTestsDataProvider.BinCollectionName];

//		var items = new List<TestItem>()
//		{
//			new TestItem("5x5x5", dimensions, scenario.NoOfItems)
//		};
//		var expectedBin = scenario.ExpectedSize != "None" ? binCollection.FirstOrDefault(x => x.ID == scenario.ExpectedSize) : binCollection.Last();
//		var algorithmInstance = new Packing.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(expectedBin, items);
//		var result = algorithmInstance.Execute();
//		if (scenario.ExpectedSize != "None")
//		{
//			Assert.Equal(PackingResultStatus.FullyPacked, result.Status);
//		}
//		else
//		{
//			PackingResultStatus[] expectedResults = [PackingResultStatus.PartiallyPacked, PackingResultStatus.NotPacked];

//			Assert.True(expectedResults.Contains(result.Status));
//		}
//	}


//	private void RunTest<TAlgorithm>(
//		TAlgorithm algorithmInstance,
//		BenchmarkScalingScenario scenario,
//		Dimensions dimensions,
//		string binCollectionName
//		)
//		where TAlgorithm : class, IFittingAlgorithm
//	{
//		var binCollection = Fixture.Bins[binCollectionName];

//		var items = Enumerable.Range(1, scenario.NoOfItems).Select(x => new TestItem(x.ToString(), dimensions)).ToList();

//		var operation = algorithmInstance
//		 .WithBins(binCollection)
//		 .AndItems(items)
//		 .Build();

//		var result = operation.Execute();

//		if (scenario.ExpectedSize != "None")
//		{
//			Assert.Equal(scenario.ExpectedSize, result.FoundBin.ID);
//		}
//		else
//		{
//			Assert.Null(result.FoundBin);
//		}
//	}
//}

