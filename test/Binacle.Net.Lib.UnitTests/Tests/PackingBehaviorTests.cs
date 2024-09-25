using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.TestsKernel.Models;
using Xunit;

namespace Binacle.Net.Lib.UnitTests;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class PackingBehaviorTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }
	public TestBin TestBin { get; }
	
	public PackingBehaviorTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
		this.TestBin = new TestBin("10x10x10", new Dimensions(10, 10, 10));
	}

	// Fully Packed | OptInToEarlyFails |  NeverReportUnpackedItems | ReportPackedItemsOnlyWhenFullyPacked
	// 1            |		X		    |			x			    |				x
									    						    
	// 0	        |		0		    |			0			    |				0
	// 0	        |		0		    |			0			    |				1
	// 0	        |		0		    |			1			    |				0
	// 0	        |		0		    |			1			    |				1
									    						    
	// 0	        |		1		    |			0			    |				0
	// 0	        |		1		    |			0			    |				1
	// 0	        |		1		    |			1			    |				0
	// 0	        |		1		    |			1			    |				1

	#region All Items Will Be Packed
	// Fully Packed | OptInToEarlyFails |  NeverReportUnpackedItems | ReportPackedItemsOnlyWhenFullyPacked
	// 1            |		X		    |			x			    |				x

	[Fact(DisplayName = "On Fully Packed, Operation Reports Packed Items")]
	public void OnFullyPacked_OperationReportsPackedItems()
	{
		var expectedPackedItem = new TestItem("5x5x5", new Dimensions(5, 5, 5), 4);
		var testItems = new List<TestItem>() { expectedPackedItem };

		var parameters = new Packing.Models.PackingParameters 
		{ 
			OptInToEarlyFails = false, 
			NeverReportUnpackedItems = false, 
			ReportPackedItemsOnlyWhenFullyPacked = false 
		};

		foreach (var algorithmFactory in this.Fixture.TestedPackingAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			Assert.Equal(Packing.Models.PackingResultStatus.FullyPacked, result.Status);

			// Packed Items
			this.AssertItemsAreCorrect(expectedPackedItem, result.PackedItems, coordinatesShouldExist: true);

			// Unpacked Items
			this.AssertItemsDontExist(result.UnpackedItems);
		}
	}
	#endregion

	#region Not All Items Will Be Packed / No Early Fails
	// Fully Packed | OptInToEarlyFails |  NeverReportUnpackedItems | ReportPackedItemsOnlyWhenFullyPacked
	// 0	        |		0		    |			0			    |				0
	// 0	        |		0		    |			0			    |				1
	// 0	        |		0		    |			1			    |				0
	// 0	        |		0		    |			1			    |				1
	[Fact(DisplayName = "On Partial Pack And No Early Fails, Without Report Packed Items Only When Fully Packed, Operation Reports Packed Items")]
	public void OnPartialPackAndNoEarlyFails_WithoutReportPackedItemsOnlyWhenFullyPacked_OperationReportsPackedItems()
	{
		var expectedPackedItem = new TestItem("10x10x5", new Dimensions(10, 10, 5), 1);
		var expectedUnpackedItem = new TestItem("6x6x6", new Dimensions(6, 6, 6), 1);
		var testItems = new List<TestItem>() { expectedPackedItem, expectedUnpackedItem };

		var parameters = new Packing.Models.PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		};

		foreach (var algorithmFactory in this.Fixture.TestedPackingAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			Assert.Equal(Packing.Models.PackingResultStatus.PartiallyPacked, result.Status);

			// Packed Items
			this.AssertItemsAreCorrect(expectedPackedItem, result.PackedItems, coordinatesShouldExist: true);

			// Unpacked Items
			this.AssertItemsAreCorrect(expectedUnpackedItem, result.UnpackedItems, coordinatesShouldExist: false);
		}
	}

	[Fact(DisplayName = "On Partial Pack And No Early Fails, With Report Packed Items Only When Fully Packed, Operation Does Not Report Packed Items")]
	public void OnPartialPackAndNoEarlyFails_WithReportPackedItemsOnlyWhenFullyPacked_OperationDoesNotReportPackedItems()
	{
		var expectedPackedItem = new TestItem("10x10x5", new Dimensions(10, 10, 5), 1);
		var expectedUnpackedItem = new TestItem("6x6x6", new Dimensions(6, 6, 6), 1);
		var testItems = new List<TestItem>() { expectedPackedItem, expectedUnpackedItem };

		var parameters = new Packing.Models.PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = true
		};

		foreach (var algorithmFactory in this.Fixture.TestedPackingAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			Assert.Equal(Packing.Models.PackingResultStatus.PartiallyPacked, result.Status);

			// Packed Items
			this.AssertItemsDontExist(result.PackedItems);

			// Unpacked Items
			this.AssertItemsAreCorrect(expectedUnpackedItem, result.UnpackedItems, coordinatesShouldExist: false);
		}
	}

	[Fact(DisplayName = "On Partial Pack And No Early Fails, With Never Report Unpacked Items, Operation Does Not Report Unpacked Items")]
	public void OnPartialPackAndNoEarlyFails_WithNeverReportUnpackedItems_OperationDoesNotReportUnpackedItems()
	{
		var expectedPackedItem = new TestItem("10x10x5", new Dimensions(10, 10, 5), 1);
		var expectedUnpackedItem = new TestItem("6x6x6", new Dimensions(6, 6, 6), 1);
		var testItems = new List<TestItem>() { expectedPackedItem, expectedUnpackedItem };

		var parameters = new Packing.Models.PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = true,
			ReportPackedItemsOnlyWhenFullyPacked = false
		};

		foreach (var algorithmFactory in this.Fixture.TestedPackingAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			Assert.Equal(Packing.Models.PackingResultStatus.PartiallyPacked, result.Status);

			// Packed Items
			this.AssertItemsAreCorrect(expectedPackedItem, result.PackedItems, coordinatesShouldExist: true);

			// Unpacked Items
			this.AssertItemsDontExist(result.UnpackedItems);
		}
	}

	[Fact(DisplayName = "On Partial Pack And No Early Fails, With Report Packed Items Only When Fully Packed And Never Report Unpacked Items, Operation Does Not Report Either")]
	public void OnPartialPackAndNoEarlyFails_WithReportPackedItemsOnlyWhenFullyPackedAndNeverReportUnpackedItems_OperationDoesNotReportEither()
	{
		var expectedPackedItem = new TestItem("10x10x5", new Dimensions(10, 10, 5), 1);
		var expectedUnpackedItem = new TestItem("6x6x6", new Dimensions(6, 6, 6), 1);
		var testItems = new List<TestItem>() { expectedPackedItem, expectedUnpackedItem };

		var parameters = new Packing.Models.PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = true,
			ReportPackedItemsOnlyWhenFullyPacked = true
		};

		foreach (var algorithmFactory in this.Fixture.TestedPackingAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			Assert.Equal(Packing.Models.PackingResultStatus.PartiallyPacked, result.Status);

			// Packed Items
			this.AssertItemsDontExist(result.PackedItems);

			// Unpacked Items
			this.AssertItemsDontExist(result.UnpackedItems);
		}
	}
	#endregion

	#region Not All Items Will Be Packed / Early Fails
	// Fully Packed | OptInToEarlyFails |  NeverReportUnpackedItems | ReportPackedItemsOnlyWhenFullyPacked
	// 0	        |		1		    |			0			    |				0
	// 0	        |		1		    |			0			    |				1
	// 0	        |		1		    |			1			    |				0
	// 0	        |		1		    |			1			    |				1
	[Fact(DisplayName = "With Long Item And Early Fails, Operation Fails Early, And Reports Container Dimension Exceeded")]
	public void WithLongItemAndEarlyFails_OperationFailsEarly_AndReportsContainerDimensionExceeded()
	{
		var longItem = new TestItem("1x1x15", new Dimensions(1, 1, 15), 1);
		var testItems = new List<TestItem>() { longItem };

		var parameters = new Packing.Models.PackingParameters
		{
			OptInToEarlyFails = true,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		};

		foreach (var algorithmFactory in this.Fixture.TestedPackingAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			Assert.Equal(Packing.Models.PackingResultStatus.EarlyFail_ContainerDimensionExceeded, result.Status);

			// Packed Items
			this.AssertItemsDontExist(result.PackedItems);

			// Unpacked Items
			this.AssertItemsAreCorrect(longItem, result.UnpackedItems, coordinatesShouldExist: false);
		}
	}

	[Fact(DisplayName = "With Items Total Volume Greater Than Bin Volume And Early Fails, Operation Fails Early, And Reports Container Volume Exceeded")]
	public void WithItemsTotalVolumeGreaterThanBinVolumeAndEarlyFails_OperationFailsEarly_AndReportsContainerVolumeExceeded()
	{
		var longItem = new TestItem("5x5x5", new Dimensions(5, 5, 5), 9);
		var testItems = new List<TestItem>() { longItem };

		var parameters = new Packing.Models.PackingParameters
		{
			OptInToEarlyFails = true,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		};

		foreach (var algorithmFactory in this.Fixture.TestedPackingAlgorithms)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			Assert.Equal(Packing.Models.PackingResultStatus.EarlyFail_ContainerVolumeExceeded, result.Status);

			// Packed Items
			this.AssertItemsDontExist(result.PackedItems);

			// Unpacked Items
			this.AssertItemsAreCorrect(longItem, result.UnpackedItems, coordinatesShouldExist: false);
		}
	}

	#endregion
	private void AssertItemsAreCorrect(TestItem expectedItem, List<Packing.Models.ResultItem>? items, bool coordinatesShouldExist)
	{
		Assert.NotNull(items);
		Assert.NotEmpty(items);
		Assert.Equal(expectedItem.Quantity, items!.Count);
		foreach (var item in items)
		{
			Assert.Equal(expectedItem.ID, item.ID);
			if (coordinatesShouldExist)
			{
				Assert.NotNull(item.Coordinates);
			}
			else
			{
				Assert.Null(item.Coordinates);
			}
		}
	}

	private void AssertItemsDontExist(List<Packing.Models.ResultItem>? items)
	{
		Assert.True(items is null || items.Count == 0);
	}

}
