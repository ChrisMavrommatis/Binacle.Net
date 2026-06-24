// using Binacle.ViPaq.UnitTests.Models;
// using Binacle.ViPaq.UnitTests.Providers;
// using Version = Binacle.ViPaq.Version;
//
// namespace Binacle.ViPaq.UnitTests;
//
// // Purpose: the 1 byte encoding header that Serialize writes is correct — the right storage width per section
// // and the right compression flag.
// [Trait("Result Tests", "Ensures results are as expected")]
// public class SerializationEncodingTests : IClassFixture<SerializationTestingFixture>
// {
// 	private readonly SerializationTestingFixture fixture;
//
// 	public SerializationEncodingTests(SerializationTestingFixture fixture)
// 	{
// 		this.fixture = fixture;
// 	}
//
// 	[Theory]
// 	[ClassData(typeof(DimensionBitSizeBoundaryProvider))]
// 	public void Serialize_Writes_Correct_BinDimensionsBitSize(int dimension, BitSize expected)
// 	{
// 		var bin = new Bin<int> { Length = dimension, Width = dimension, Height = dimension };
// 		var items = new List<Item<int>> { new() { Length = 1, Width = 1, Height = 1, X = 0, Y = 0, Z = 0 } };
//
// 		var data = this.fixture.SerializeInt32(bin, items);
//
// 		this.fixture.ReadHeader(data).BinDimensionsBitSize.ShouldBe(expected);
// 	}
//
// 	[Fact]
// 	public void Serialize_Marks_Version_CompressedGzip_When_Body_Is_Large()
// 	{
// 		var bin = this.fixture.CreateBin(1, 255);
// 		var items = this.fixture.CreateItems(1, 255, 1, 255, 60);
//
// 		var data = this.fixture.SerializeInt32(bin, items);
//
// 		this.fixture.ReadHeader(data).Version.ShouldBe(Version.CompressedGzip);
// 	}
//
// 	[Fact]
// 	public void Serialize_Marks_Version_Uncompressed_When_Body_Is_Small()
// 	{
// 		var bin = this.fixture.CreateBin(1, 255);
// 		var items = this.fixture.CreateItems(1, 255, 1, 255, 2);
//
// 		var data = this.fixture.SerializeInt32(bin, items);
//
// 		this.fixture.ReadHeader(data).Version.ShouldBe(Version.Uncompressed);
// 	}
// }
