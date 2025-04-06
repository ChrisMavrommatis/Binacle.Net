using Binacle.Lib.Abstractions.Fitting;
using Binacle.Lib.Fitting.Models;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.UnitTests;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class FittingBehaviorTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }
	public TestBin TestBin { get; }
	public FittingBehaviorTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
		this.TestBin = new TestBin("10x10x10", new Dimensions(10, 10, 10));
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

		var parameters = new FittingParameters { ReportFittedItems = true, ReportUnfittedItems = false };
		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.FittingAlgorithmsUnderTest)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);
			
			// Result
			result.Status.ShouldBe(FittingResultStatus.Success);
			result.Reason.ShouldBeNull();

			// Fitted Items
			this.AssertItemsAreCorrect(expectedFittedItem, result.FittedItems);

			// Unfitted Items
			result.UnfittedItems.ShouldBeNullOrEmpty();
		}
	}

	[Fact(DisplayName = "On Success, Without Report Fitted Items, Result Does Not Report Fitted Items")]
	public void OnSuccess_WithoutReportFittedItems_ResultDoesNotReportFittedItems()
	{
		var expectedFittedItem = new TestItem("5x5x5", new Dimensions(5, 5, 5), 4);
		var testItems = new List<TestItem>() { expectedFittedItem };

		var parameters = new FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false };
		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.FittingAlgorithmsUnderTest)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			result.Status.ShouldBe(FittingResultStatus.Success);
			result.Reason.ShouldBeNull();
			
			// Fitted Items
			result.FittedItems.ShouldBeNullOrEmpty();

			// Unfitted Items
			result.UnfittedItems.ShouldBeNullOrEmpty();
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

		var parameters = new FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false };
		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.FittingAlgorithmsUnderTest)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);
			
			// Result
			result.Status.ShouldBe(FittingResultStatus.Fail);
			result.Reason.ShouldNotBeNull();
			result.Reason.ShouldBe(FittingFailedResultReason.DidNotFit);

			// Fitted Items
			result.FittedItems.ShouldBeNullOrEmpty();

			// Unfitted Items
			result.UnfittedItems.ShouldBeNullOrEmpty();
		}
	}

	[Fact(DisplayName = "On Fail, With Report Fitted Items, Result Reports Fitted Items")]
	public void OnFail_WithReportFittedItems_ResultReportsFittedItems()
	{
		var expectedFittedItem = new TestItem("10x10x5", new Dimensions(10, 10, 5), 1);
		var expectedUnfittedItem = new TestItem("6x6x6", new Dimensions(6, 6, 6), 1);
		var testItems = new List<TestItem>() { expectedFittedItem, expectedUnfittedItem };

		var parameters = new FittingParameters { ReportFittedItems = true, ReportUnfittedItems = false };
		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.FittingAlgorithmsUnderTest)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			result.Status.ShouldBe(FittingResultStatus.Fail);
			result.Reason.ShouldNotBeNull();
			result.Reason.ShouldBe(FittingFailedResultReason.DidNotFit);

			// Fitted Items
			this.AssertItemsAreCorrect(expectedFittedItem, result.FittedItems);

			// Unfitted Items
			result.UnfittedItems.ShouldBeNullOrEmpty();
		}
	}

	[Fact(DisplayName = "On Fail, With Report Unfitted Items, Result Reports Unfitted Items")]
	public void OnFail_WithReportUnfittedItems_ResultReportsUnfittedItems()
	{
		var expectedFittedItem = new TestItem("10x10x5", new Dimensions(10, 10, 5), 1);
		var expectedUnfittedItem = new TestItem("6x6x6", new Dimensions(6, 6, 6), 1);
		var testItems = new List<TestItem>() { expectedFittedItem, expectedUnfittedItem };

		var parameters = new FittingParameters { ReportFittedItems = false, ReportUnfittedItems = true };
		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.FittingAlgorithmsUnderTest)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			result.Status.ShouldBe(FittingResultStatus.Fail);
			result.Reason.ShouldNotBeNull();
			result.Reason.ShouldBe(FittingFailedResultReason.DidNotFit);

			// Fitted Items
			result.FittedItems.ShouldBeNullOrEmpty();

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

		var parameters = new FittingParameters { ReportFittedItems = true, ReportUnfittedItems = true };
		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.FittingAlgorithmsUnderTest)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			result.Status.ShouldBe(FittingResultStatus.Fail);
			result.Reason.ShouldNotBeNull();
			result.Reason.ShouldBe(FittingFailedResultReason.DidNotFit);

			// Fitted Items
			this.AssertItemsAreCorrect(expectedFittedItem, result.FittedItems);

			// Unfitted Items
			this.AssertItemsAreCorrect(expectedUnfittedItem, result.UnfittedItems);
		}
	}
	#endregion

	private void AssertItemsAreCorrect(TestItem expectedItem, List<ResultItem>? items)
	{
		items.ShouldNotBeEmpty();
		items.ShouldHaveCount(expectedItem.Quantity);
		foreach (var item in items)
		{
			item.ID.ShouldBe(expectedItem.ID);
		}
	}

	#region Early Fails
	[Fact(DisplayName = "With Long Item, Operation Fails, And Reports Item Dimension Exceeded")]
	public void WithLongItem_OperationFails_AndReportsItemDimensionExceeded()
	{
		var longItem = new TestItem("1x1x15", new Dimensions(1, 1, 15), 1);
		var testItems = new List<TestItem>() { longItem };

		var parameters = new FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false };
		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.FittingAlgorithmsUnderTest)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			result.Status.ShouldBe(FittingResultStatus.Fail);
			result.Reason.ShouldNotBeNull();
			result.Reason.ShouldBe(FittingFailedResultReason.ItemDimensionExceeded);
		}
	}

	[Fact(DisplayName = "With Items Total Volume Greater Than Bin Volume, Operation Fails, And Reports Total Volume Exceeded")]
	public void WithItemsTotalVolumeGreaterThanBinVolume_OperationFails_AndReportsTotalVolumeExceeded()
	{
		var longItem = new TestItem("5x5x5", new Dimensions(5, 5, 5), 9);
		var testItems = new List<TestItem>() { longItem };

		var parameters = new FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false };
		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.FittingAlgorithmsUnderTest)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			result.Status.ShouldBe(FittingResultStatus.Fail);
			result.Reason.ShouldNotBeNull();
			result.Reason.ShouldBe(FittingFailedResultReason.TotalVolumeExceeded);
		}
	}
	#endregion
}
