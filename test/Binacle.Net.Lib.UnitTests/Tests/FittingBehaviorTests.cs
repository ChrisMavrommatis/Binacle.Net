using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.TestsKernel.Models;
using Xunit;

namespace Binacle.Net.Lib.UnitTests;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class PackingBehaviorTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }
	public TestBin TestBin { get; }
	public Func<TestBin, List<TestItem>, IPackingAlgorithm>[] TestedAlgorithms { get; }

	public PackingBehaviorTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
		this.TestBin = new TestBin("10x10x10", new Dimensions(10, 10, 10));
		this.TestedAlgorithms = [AlgorithmFactories.Packing_FFD_v1];

	}

	public void sda()
	{
		var expectedFittedItem = new TestItem("5x5x5", new Dimensions(5, 5, 5), 4);
		var testItems = new List<TestItem>() { expectedFittedItem };

		var parameters = new Packing.Models.PackingParameters { OptInToEarlyFails = false, NeverReportUnpackedItems = false, ReportPackedItemsOnlyWhenFullyPacked = false };
		foreach (var algorithmFactory in this.TestedAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			
		}
	}
}


[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class FittingBehaviorTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }
	public TestBin TestBin { get; }
	public Func<TestBin, IEnumerable<TestItem>, IFittingAlgorithm>[] TestedAlgorithms { get; }

	public FittingBehaviorTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
		this.TestBin = new TestBin("10x10x10", new Dimensions(10, 10, 10));
		this.TestedAlgorithms = [AlgorithmFactories.Fitting_FFD_v1, AlgorithmFactories.Fitting_FFD_v2];

	}

	[Fact(DisplayName = "On Success, With Report Fitted Items, Result Reports Fitted Items")]
	public void OnSuccess_WithReportFittedItems_ResultReportsFittedItems()
	{
		var expectedFittedItem = new TestItem("5x5x5", new Dimensions(5, 5, 5), 4);
		var testItems = new List<TestItem>() { expectedFittedItem };

		var parameters = new Fitting.Models.FittingParameters { ReportFittedItems = true, ReportUnfittedItems = false };
		foreach (var algorithmFactory in this.TestedAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			Assert.Equal(Fitting.Models.FittingResultStatus.Success, result.Status);
			Assert.NotNull(result.FittedItems);
			Assert.NotEmpty(result.FittedItems);
			Assert.Equal(expectedFittedItem.Quantity, result.FittedItems!.Count);
			foreach(var fittedItem in result.FittedItems)
			{
				Assert.Equal(expectedFittedItem.ID, fittedItem.ID);
				Assert.Equal(expectedFittedItem.Length, fittedItem.Length);
				Assert.Equal(expectedFittedItem.Width, fittedItem.Width);
				Assert.Equal(expectedFittedItem.Height, fittedItem.Height);
			}
		}
	}

	[Fact(DisplayName = "On Success, Without Report Fitted Items, Result Does Not Report Fitted Items")]
	public void OnSuccess_WithoutReportFittedItems_ResultDoesNotReportFittedItems()
	{
		var expectedFittedItem = new TestItem("5x5x5", new Dimensions(5, 5, 5), 4);
		var testItems = new List<TestItem>() { expectedFittedItem };

		var parameters = new Fitting.Models.FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false };
		foreach (var algorithmFactory in this.TestedAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			Assert.Equal(Fitting.Models.FittingResultStatus.Success, result.Status);
			// null or empty
			Assert.True(result.FittedItems is null || result.FittedItems.Count == 0);
		}
	}

	[Fact(DisplayName = "On Fail, With Report Fitted Items, Result Reports Fitted Items")]
	public void OnFail_WithReportFittedItems_ResultReportsFittedItems()
	{
		var expectedFittedItem = new TestItem("10x10x5", new Dimensions(10, 10, 5), 1);
		var expectedUnfittedItem = new TestItem("6x6x6", new Dimensions(6, 6, 6), 1);
		var testItems = new List<TestItem>() { expectedFittedItem, expectedUnfittedItem };

		var parameters = new Fitting.Models.FittingParameters { ReportFittedItems = true, ReportUnfittedItems = false };
		foreach (var algorithmFactory in this.TestedAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			Assert.Equal(Fitting.Models.FittingResultStatus.Fail, result.Status);
			Assert.NotNull(result.FittedItems);
			Assert.NotEmpty(result.FittedItems);
			Assert.Equal(expectedFittedItem.Quantity, result.FittedItems!.Count);
			foreach (var fittedItem in result.FittedItems)
			{
				Assert.Equal(expectedFittedItem.ID, fittedItem.ID);
				Assert.Equal(expectedFittedItem.Length, fittedItem.Length);
				Assert.Equal(expectedFittedItem.Width, fittedItem.Width);
				Assert.Equal(expectedFittedItem.Height, fittedItem.Height);
			}
		}
	}

	[Fact(DisplayName = "On Fail, With Report Unfitted Items, Result Reports Unfitted Items")]
	public void OnFail_WithReportUnfittedItems_ResultReportsUnfittedItems()
	{
		var expectedFittedItem = new TestItem("10x10x5", new Dimensions(10, 10, 5), 1);
		var expectedUnfittedItem = new TestItem("6x6x6", new Dimensions(6, 6, 6), 1);
		var testItems = new List<TestItem>() { expectedFittedItem, expectedUnfittedItem };

		var parameters = new Fitting.Models.FittingParameters { ReportFittedItems = false, ReportUnfittedItems = true };
		foreach (var algorithmFactory in this.TestedAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			Assert.Equal(Fitting.Models.FittingResultStatus.Fail, result.Status);
			Assert.NotNull(result.UnfittedItems);
			Assert.NotEmpty(result.UnfittedItems);
			Assert.Equal(expectedUnfittedItem.Quantity, result.UnfittedItems!.Count);
			foreach (var unfittedItem in result.UnfittedItems)
			{
				Assert.Equal(unfittedItem.ID, unfittedItem.ID);
				Assert.Equal(unfittedItem.Length, unfittedItem.Length);
				Assert.Equal(unfittedItem.Width, unfittedItem.Width);
				Assert.Equal(unfittedItem.Height, unfittedItem.Height);
			}
		}
	}

	[Fact(DisplayName = "On Fail, With Report Both Fitted And Unfitted Items, Result Reports Both")]
	public void OnFail_WithReportBothFittedAndUnfittedItems_ResultReportsBoth()
	{
		var expectedFittedItem = new TestItem("10x10x5", new Dimensions(10, 10, 5), 1);
		var expectedUnfittedItem = new TestItem("6x6x6", new Dimensions(6, 6, 6), 1);
		var testItems = new List<TestItem>() { expectedFittedItem, expectedUnfittedItem };

		var parameters = new Fitting.Models.FittingParameters { ReportFittedItems = true, ReportUnfittedItems = true };
		foreach (var algorithmFactory in this.TestedAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			Assert.Equal(Fitting.Models.FittingResultStatus.Fail, result.Status);
			Assert.NotNull(result.FittedItems);
			Assert.NotEmpty(result.FittedItems);
			Assert.NotNull(result.UnfittedItems);
			Assert.NotEmpty(result.UnfittedItems);
			Assert.Equal(expectedFittedItem.Quantity, result.FittedItems!.Count);
			Assert.Equal(expectedUnfittedItem.Quantity, result.UnfittedItems!.Count);

			foreach (var fittedItem in result.FittedItems)
			{
				Assert.Equal(expectedFittedItem.ID, fittedItem.ID);
				Assert.Equal(expectedFittedItem.Length, fittedItem.Length);
				Assert.Equal(expectedFittedItem.Width, fittedItem.Width);
				Assert.Equal(expectedFittedItem.Height, fittedItem.Height);
			}
			foreach (var unfittedItem in result.UnfittedItems)
			{
				Assert.Equal(expectedUnfittedItem.ID, unfittedItem.ID);
				Assert.Equal(expectedUnfittedItem.Length, unfittedItem.Length);
				Assert.Equal(expectedUnfittedItem.Width, unfittedItem.Width);
				Assert.Equal(expectedUnfittedItem.Height, unfittedItem.Height);
			}
		}
	}


}
