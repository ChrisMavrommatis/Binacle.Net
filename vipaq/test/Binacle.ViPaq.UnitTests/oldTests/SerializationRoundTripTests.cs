// using Binacle.ViPaq.UnitTests.Models;
//
// namespace Binacle.ViPaq.UnitTests;
//
// // Purpose: a bin and its items come back unchanged through Serialize -> Deserialize, across every storage width.
// [Trait("Result Tests", "Ensures results are as expected")]
// public class SerializationRoundTripTests : IClassFixture<SerializationTestingFixture>
// {
// 	private readonly SerializationTestingFixture fixture;
//
// 	public SerializationRoundTripTests(SerializationTestingFixture fixture)
// 	{
// 		this.fixture = fixture;
// 	}
//
// 	#region Int32
//
// 	[Fact]
// 	public void Int32_RoundTrips_When_Values_Are_Eight_Bit()
// 	{
// 		var bin = this.fixture.CreateBin(1, 255);
// 		var items = this.fixture.CreateItems(1, 255, 0, 255, 2);
//
// 		this.fixture.RoundTripInt32(bin, items);
// 	}
//
// 	[Fact]
// 	public void Int32_RoundTrips_When_Values_Are_Sixteen_Bit()
// 	{
// 		var bin = this.fixture.CreateBin(256, 65535);
// 		var items = this.fixture.CreateItems(256, 65535, 256, 65535, 2);
//
// 		this.fixture.RoundTripInt32(bin, items);
// 	}
//
// 	// Above 65535 only round trips once Bug B (the overflowing comparison constant) is fixed.
// 	[Fact]
// 	public void Int32_RoundTrips_When_Values_Are_Thirty_Two_Bit()
// 	{
// 		var bin = this.fixture.CreateBin(65536, 1_000_000);
// 		var items = this.fixture.CreateItems(65536, 1_000_000, 65536, 1_000_000, 1);
//
// 		this.fixture.RoundTripInt32(bin, items);
// 	}
//
// 	// Eight bit dimensions mixed with thirty two bit coordinates: the second width also trips Bug B.
// 	[Fact]
// 	public void Int32_RoundTrips_When_Dimensions_Are_Small_And_Coordinates_Are_Large()
// 	{
// 		var bin = this.fixture.CreateBin(1, 255);
// 		var items = this.fixture.CreateItems(1, 255, 65536, 1_000_000, 1);
//
// 		this.fixture.RoundTripInt32(bin, items);
// 	}
//
// 	#endregion
//
// 	#region UInt16
//
// 	// Eight bit sections only round trip once Bug A (ReadAsByte for non-int T) is fixed.
// 	[Fact]
// 	public void UInt16_RoundTrips_When_Values_Are_Eight_Bit()
// 	{
// 		var bin = this.fixture.CreateBinU(1, 255);
// 		var items = this.fixture.CreateItemsU(1, 255, 0, 255, 2);
//
// 		this.fixture.RoundTripUInt16(bin, items);
// 	}
//
// 	[Fact]
// 	public void UInt16_RoundTrips_When_Bin_Is_Small_And_Items_Are_Large()
// 	{
// 		var bin = this.fixture.CreateBinU(1, 255);
// 		var items = this.fixture.CreateItemsU(256, 65535, 256, 65535, 1);
//
// 		this.fixture.RoundTripUInt16(bin, items);
// 	}
//
// 	#endregion
//
// 	#region UInt64
//
// 	[Fact]
// 	public void UInt64_RoundTrips_When_Values_Are_Sixty_Four_Bit()
// 	{
// 		var bin = this.fixture.CreateBinUL(5_000_000_000, 7_000_000_000);
// 		var items = this.fixture.CreateItemsUL(5_000_000_000, 7_000_000_000, 5_000_000_000, 7_000_000_000, 1);
//
// 		this.fixture.RoundTripUInt64(bin, items);
// 	}
//
// 	[Fact]
// 	public void UInt64_RoundTrips_When_Bin_Is_Small_And_Items_Are_Large()
// 	{
// 		var bin = this.fixture.CreateBinUL(1, 255);
// 		var items = this.fixture.CreateItemsUL(5_000_000_000, 7_000_000_000, 5_000_000_000, 7_000_000_000, 1);
//
// 		this.fixture.RoundTripUInt64(bin, items);
// 	}
//
// 	#endregion
//
// 	#region Edge cases
//
// 	[Fact]
// 	public void Int32_RoundTrips_When_Coordinates_Are_Zero()
// 	{
// 		var bin = this.fixture.CreateBin(1, 255);
// 		var items = this.fixture.CreateItems(1, 255, 0, 0, 1);
//
// 		var (_, resultItems) = this.fixture.RoundTripInt32(bin, items);
// 		resultItems[0].X.ShouldBe(0);
// 		resultItems[0].Y.ShouldBe(0);
// 		resultItems[0].Z.ShouldBe(0);
// 	}
//
// 	[Fact]
// 	public void Int32_RoundTrips_When_There_Are_No_Items()
// 	{
// 		var bin = this.fixture.CreateBin(1, 255);
//
// 		var (_, resultItems) = this.fixture.RoundTripInt32(bin, new List<Item<int>>());
// 		resultItems.Count.ShouldBe(0);
// 	}
//
// 	// A body over 255 bytes is gzipped; it must still round trip through the compressed path.
// 	[Fact]
// 	public void Int32_RoundTrips_When_Body_Is_Compressed()
// 	{
// 		var bin = this.fixture.CreateBin(1, 255);
// 		var items = this.fixture.CreateItems(1, 255, 1, 255, 60);
//
// 		this.fixture.RoundTripInt32(bin, items);
// 	}
//
// 	#endregion
// }
