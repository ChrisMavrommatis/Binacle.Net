using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.TestsKernel.Models;
using Xunit;

namespace Binacle.Net.Lib.UnitTests;

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

	#region Parameters / Successfull Result
	// report | unfitted Items
	//    0   |    always 0
	//    1   |    always 0

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
			
			// Result
			Assert.Equal(Fitting.Models.FittingResultStatus.Success, result.Status);
			Assert.Null(result.Reason);

			// Fitted Items
			this.AssertItemsAreCorrect(expectedFittedItem, result.FittedItems);

			// Unfitted Items
			this.AssertItemsDontExist(result.UnfittedItems);
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

			// Result
			Assert.Equal(Fitting.Models.FittingResultStatus.Success, result.Status);
			Assert.Null(result.Reason);

			// Fitted Items
			this.AssertItemsDontExist(result.FittedItems);
			
			// Unfitted Items
			this.AssertItemsDontExist(result.UnfittedItems);
		}
	}

	#endregion

	#region Parameters / Failed Result
	// Report Fitted | Report Unfitted
	//       0		 |        0
	//       0		 |        1
	//       1		 |        0
	//       1		 |        1

	[Fact(DisplayName = "On Fail, Without Report Fitted And Unfitted Items, Result Does Not Report Either")]
	public void OnFail_WithoutReportFittedAndUnfittedItems_ResultDoesNotReportEither()
	{
		var expectedFittedItem = new TestItem("10x10x5", new Dimensions(10, 10, 5), 1);
		var expectedUnfittedItem = new TestItem("6x6x6", new Dimensions(6, 6, 6), 1);
		var testItems = new List<TestItem>() { expectedFittedItem, expectedUnfittedItem };

		var parameters = new Fitting.Models.FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false };
		foreach (var algorithmFactory in this.TestedAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);
			
			// Result
			Assert.Equal(Fitting.Models.FittingResultStatus.Fail, result.Status);
			Assert.NotNull(result.Reason);
			Assert.Equal(Fitting.Models.FittingFailedResultReason.DidNotFit, result.Reason);

			// Fitted Items
			this.AssertItemsDontExist(result.FittedItems);

			// Unfitted Items
			this.AssertItemsDontExist(result.UnfittedItems);
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

			// Result
			Assert.Equal(Fitting.Models.FittingResultStatus.Fail, result.Status);
			Assert.NotNull(result.Reason);
			Assert.Equal(Fitting.Models.FittingFailedResultReason.DidNotFit, result.Reason);

			// Fitted Items
			this.AssertItemsAreCorrect(expectedFittedItem, result.FittedItems);

			// Unfitted Items
			this.AssertItemsDontExist(result.UnfittedItems);
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

			// Result
			Assert.Equal(Fitting.Models.FittingResultStatus.Fail, result.Status);
			Assert.NotNull(result.Reason);
			Assert.Equal(Fitting.Models.FittingFailedResultReason.DidNotFit, result.Reason);

			// Fitted Items
			this.AssertItemsDontExist(result.FittedItems);

			// Unfitted Items
			this.AssertItemsAreCorrect(expectedUnfittedItem, result.UnfittedItems);
		}
	}

	[Fact(DisplayName = "On Fail, With Report Fitted And Unfitted Items, Result Reports Both")]
	public void OnFail_WithReportFittedAndUnfittedItems_ResultReportsBoth()
	{
		var expectedFittedItem = new TestItem("10x10x5", new Dimensions(10, 10, 5), 1);
		var expectedUnfittedItem = new TestItem("6x6x6", new Dimensions(6, 6, 6), 1);
		var testItems = new List<TestItem>() { expectedFittedItem, expectedUnfittedItem };

		var parameters = new Fitting.Models.FittingParameters { ReportFittedItems = true, ReportUnfittedItems = true };
		foreach (var algorithmFactory in this.TestedAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			Assert.Equal(Fitting.Models.FittingResultStatus.Fail, result.Status);
			Assert.NotNull(result.Reason);
			Assert.Equal(Fitting.Models.FittingFailedResultReason.DidNotFit, result.Reason);

			// Fitted Items
			this.AssertItemsAreCorrect(expectedFittedItem, result.FittedItems);

			// Unfitted Items
			this.AssertItemsAreCorrect(expectedUnfittedItem, result.UnfittedItems);
		}
	}
	#endregion

	private void AssertItemsAreCorrect(TestItem expectedItem, List<Fitting.Models.ResultItem>? items)
	{
		Assert.NotNull(items);
		Assert.NotEmpty(items);
		Assert.Equal(expectedItem.Quantity, items!.Count);
		foreach (var item in items)
		{
			Assert.Equal(expectedItem.ID, item.ID);
		}
	}

	private void AssertItemsDontExist(List<Fitting.Models.ResultItem>? items)
	{
		Assert.True(items is null || items.Count == 0);
	}

	#region Early Fails
	[Fact(DisplayName = "With Long Item, Operation Fails, And Reports Item Dimension Exceeded")]
	public void WithLongItem_OperationFails_AndReportsItemDimensionExceeded()
	{
		var longItem = new TestItem("1x1x15", new Dimensions(1, 1, 15), 1);
		var testItems = new List<TestItem>() { longItem };

		var parameters = new Fitting.Models.FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false };
		foreach (var algorithmFactory in this.TestedAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			Assert.Equal(Fitting.Models.FittingResultStatus.Fail, result.Status);
			Assert.NotNull(result.Reason);
			Assert.Equal(Fitting.Models.FittingFailedResultReason.ItemDimensionExceeded, result.Reason);
		}
	}

	[Fact(DisplayName = "With Items Total Volume Greater Than Bin Volume, Operation Fails, And Reports Total Volume Exceeded")]
	public void WithItemsTotalVolumeGreaterThanBinVolume_OperationFails_AndReportsTotalVolumeExceeded()
	{
		var longItem = new TestItem("5x5x5", new Dimensions(5, 5, 5), 9);
		var testItems = new List<TestItem>() { longItem };

		var parameters = new Fitting.Models.FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false };
		foreach (var algorithmFactory in this.TestedAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			Assert.Equal(Fitting.Models.FittingResultStatus.Fail, result.Status);
			Assert.NotNull(result.Reason);
			Assert.Equal(Fitting.Models.FittingFailedResultReason.TotalVolumeExceeded, result.Reason);
		}
	}
	#endregion
}
