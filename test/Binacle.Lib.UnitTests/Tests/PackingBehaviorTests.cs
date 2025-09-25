using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Packing.Models;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.UnitTests;

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

		var parameters = new PackingParameters 
		{ 
			OptInToEarlyFails = false, 
			NeverReportUnpackedItems = false, 
			ReportPackedItemsOnlyWhenFullyPacked = false 
		};

		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.PackingAlgorithmsUnderTest)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			result.Status.ShouldBe(PackingResultStatus.FullyPacked);

			// Packed Items
			this.AssertItemsAreCorrect(expectedPackedItem, result.PackedItems, coordinatesShouldExist: true);

			// Unpacked Items
			result.UnpackedItems.ShouldBeNullOrEmpty();
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

		var parameters = new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		};

		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.PackingAlgorithmsUnderTest)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			result.Status.ShouldBe(PackingResultStatus.PartiallyPacked);

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

		var parameters = new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = true
		};

		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.PackingAlgorithmsUnderTest)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			result.Status.ShouldBe(PackingResultStatus.PartiallyPacked);

			// Packed Items
			result.PackedItems.ShouldBeNullOrEmpty();

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

		var parameters = new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = true,
			ReportPackedItemsOnlyWhenFullyPacked = false
		};

		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.PackingAlgorithmsUnderTest)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			result.Status.ShouldBe(PackingResultStatus.PartiallyPacked);

			// Packed Items
			this.AssertItemsAreCorrect(expectedPackedItem, result.PackedItems, coordinatesShouldExist: true);

			// Unpacked Items
			result.UnpackedItems.ShouldBeNullOrEmpty();
		}
	}

	[Fact(DisplayName = "On Partial Pack And No Early Fails, With Report Packed Items Only When Fully Packed And Never Report Unpacked Items, Operation Does Not Report Either")]
	public void OnPartialPackAndNoEarlyFails_WithReportPackedItemsOnlyWhenFullyPackedAndNeverReportUnpackedItems_OperationDoesNotReportEither()
	{
		var expectedPackedItem = new TestItem("10x10x5", new Dimensions(10, 10, 5), 1);
		var expectedUnpackedItem = new TestItem("6x6x6", new Dimensions(6, 6, 6), 1);
		var testItems = new List<TestItem>() { expectedPackedItem, expectedUnpackedItem };

		var parameters = new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = true,
			ReportPackedItemsOnlyWhenFullyPacked = true
		};

		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.PackingAlgorithmsUnderTest)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			result.Status.ShouldBe(PackingResultStatus.PartiallyPacked);

			// Packed Items
			result.PackedItems.ShouldBeNullOrEmpty();

			// Unpacked Items
			result.UnpackedItems.ShouldBeNullOrEmpty();
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

		var parameters = new PackingParameters
		{
			OptInToEarlyFails = true,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		};

		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.PackingAlgorithmsUnderTest)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			result.Status.ShouldBe(PackingResultStatus.EarlyFail_ContainerDimensionExceeded);

			// Packed Items
			result.PackedItems.ShouldBeNullOrEmpty();

			// Unpacked Items
			this.AssertItemsAreCorrect(longItem, result.UnpackedItems, coordinatesShouldExist: false);
		}
	}

	[Fact(DisplayName = "With Items Total Volume Greater Than Bin Volume And Early Fails, Operation Fails Early, And Reports Container Volume Exceeded")]
	public void WithItemsTotalVolumeGreaterThanBinVolumeAndEarlyFails_OperationFailsEarly_AndReportsContainerVolumeExceeded()
	{
		var longItem = new TestItem("5x5x5", new Dimensions(5, 5, 5), 9);
		var testItems = new List<TestItem>() { longItem };

		var parameters = new PackingParameters
		{
			OptInToEarlyFails = true,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		};

		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.PackingAlgorithmsUnderTest)
		{
			var algorithmInstance = algorithmFactory(this.TestBin, testItems);
			var result = algorithmInstance.Execute(parameters);

			// Result
			result.Status.ShouldBe(PackingResultStatus.EarlyFail_ContainerVolumeExceeded);

			// Packed Items
			result.PackedItems.ShouldBeNullOrEmpty();

			// Unpacked Items
			this.AssertItemsAreCorrect(longItem, result.UnpackedItems, coordinatesShouldExist: false);
		}
	}

	#endregion
	
	private void AssertItemsAreCorrect(TestItem expectedItem, List<ResultItem>? items, bool coordinatesShouldExist)
	{
		items.ShouldNotBeEmpty();
		items.ShouldHaveCount(expectedItem.Quantity);
		foreach (var item in items)
		{
			item.ID.ShouldBe(expectedItem.ID);
			if (coordinatesShouldExist)
			{
				item.Coordinates.ShouldNotBeNull();
			}
			else
			{
				item.Coordinates.ShouldBeNull();
			}
		}
	}
}
