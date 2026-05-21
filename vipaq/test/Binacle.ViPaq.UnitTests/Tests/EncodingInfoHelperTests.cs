using Binacle.ViPaq.UnitTests.Models;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

[Trait("Result Tests", "Ensures results are as expected")]
public class EncodingInfoHelperTests
{
	public EncodingInfoHelperTests()
	{
		Randomizer.Seed	= new Random(605080);
	}
	
	[Theory]
	[InlineData(byte.MinValue + 1, byte.MaxValue, BitSize.Eight)]
	[InlineData(byte.MaxValue + 1, ushort.MaxValue, BitSize.Sixteen)]
	[InlineData(ushort.MaxValue + 1, uint.MaxValue, BitSize.ThirtyTwo)]
	[InlineData((ulong)uint.MaxValue + 1, ulong.MaxValue, BitSize.SixtyFour)]
	public void CreateEncodingInfo_Creates_EncodingInfo_With_Correct_Bit_Sizes(
		ulong minValue, 
		ulong maxValue,
		BitSize expectedBitSize
	)
	{
		var binFaker = new Faker<Bin<ulong>>()
			.RuleFor(x => x.Length, x => x.Random.ULong(minValue, maxValue))
			.RuleFor(x => x.Width, x => x.Random.ULong(minValue, maxValue))
			.RuleFor(x => x.Height, x => x.Random.ULong(minValue, maxValue));
		
		var itemFaker = new Faker<Item<ulong>>()
			.RuleFor(x => x.Length, x => x.Random.ULong(minValue, maxValue))
			.RuleFor(x => x.Width, x => x.Random.ULong(minValue, maxValue))
			.RuleFor(x => x.Height, x => x.Random.ULong(minValue, maxValue))
			.RuleFor(x => x.X, x => x.Random.ULong(minValue, maxValue))
			.RuleFor(x => x.Y, x => x.Random.ULong(minValue, maxValue))
			.RuleFor(x => x.Z, x => x.Random.ULong(minValue, maxValue));

		var bin = binFaker.Generate(1).FirstOrDefault()!;
		var items = itemFaker.Generate(3).ToList();
		
		var encodingInfo = EncodingInfoHelper.CreateEncodingInfo<Bin<ulong>, Item<ulong>, ulong>(bin, items);
		
		encodingInfo.BinDimensionsBitSize.ShouldBe(expectedBitSize);
		encodingInfo.ItemDimensionsBitSize.ShouldBe(expectedBitSize);
		encodingInfo.ItemCoordinatesBitSize.ShouldBe(expectedBitSize);
	}
	
	[Theory]
	[InlineData(byte.MinValue + 1, byte.MaxValue, BitSize.Eight)]
	[InlineData(byte.MaxValue + 1, ushort.MaxValue, BitSize.Sixteen)]
	[InlineData(ushort.MaxValue + 1, uint.MaxValue, BitSize.ThirtyTwo)]
	[InlineData((ulong)uint.MaxValue + 1, ulong.MaxValue, BitSize.SixtyFour)]
	public void CreateEncodingInfo_Creates_EncodingInfo_With_Correct_BinDimensionsBitSize(
		ulong minValue, 
		ulong maxValue,
		BitSize expectedBitSize
	)
	{
		var binFaker = new Faker<Bin<ulong>>()
			.RuleFor(x => x.Length, x => x.Random.ULong(minValue, maxValue))
			.RuleFor(x => x.Width, x => x.Random.ULong(minValue, maxValue))
			.RuleFor(x => x.Height, x => x.Random.ULong(minValue, maxValue));
		
		var itemFaker = new Faker<Item<ulong>>()
			.RuleFor(x => x.Length, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.Width, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.Height, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.X, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.Y, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.Z, x => x.Random.ULong(0, byte.MaxValue));

		var bin = binFaker.Generate(1).FirstOrDefault()!;
		var items = itemFaker.Generate(3).ToList();
		
		var encodingInfo = EncodingInfoHelper.CreateEncodingInfo<Bin<ulong>, Item<ulong>, ulong>(bin, items);
		
		encodingInfo.BinDimensionsBitSize.ShouldBe(expectedBitSize);
		encodingInfo.ItemDimensionsBitSize.ShouldBe(BitSize.Eight);
		encodingInfo.ItemCoordinatesBitSize.ShouldBe(BitSize.Eight);
	}
	
	[Theory]
	[InlineData(byte.MinValue + 1, byte.MaxValue, BitSize.Eight)]
	[InlineData(byte.MaxValue + 1, ushort.MaxValue, BitSize.Sixteen)]
	[InlineData(ushort.MaxValue + 1, uint.MaxValue, BitSize.ThirtyTwo)]
	[InlineData((ulong)uint.MaxValue + 1, ulong.MaxValue, BitSize.SixtyFour)]
	public void CreateEncodingInfo_Creates_EncodingInfo_With_Correct_ItemDimensionsBitSize(
		ulong minValue, 
		ulong maxValue,
		BitSize expectedBitSize
	)
	{
		var binFaker = new Faker<Bin<ulong>>()
			.RuleFor(x => x.Length, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.Width, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.Height, x => x.Random.ULong(0, byte.MaxValue));
		
		var itemFaker = new Faker<Item<ulong>>()
			.RuleFor(x => x.Length, x => x.Random.ULong(minValue, maxValue))
			.RuleFor(x => x.Width, x => x.Random.ULong(minValue, maxValue))
			.RuleFor(x => x.Height, x => x.Random.ULong(minValue, maxValue))
			.RuleFor(x => x.X, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.Y, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.Z, x => x.Random.ULong(0, byte.MaxValue));

		var bin = binFaker.Generate(1).FirstOrDefault()!;
		var items = itemFaker.Generate(3).ToList();
		
		var encodingInfo = EncodingInfoHelper.CreateEncodingInfo<Bin<ulong>, Item<ulong>, ulong>(bin, items);
		
		encodingInfo.BinDimensionsBitSize.ShouldBe(BitSize.Eight);
		encodingInfo.ItemDimensionsBitSize.ShouldBe(expectedBitSize);
		encodingInfo.ItemCoordinatesBitSize.ShouldBe(BitSize.Eight);
	}
	
	[Theory]
	[InlineData(byte.MinValue + 1, byte.MaxValue, BitSize.Eight)]
	[InlineData(byte.MaxValue + 1, ushort.MaxValue, BitSize.Sixteen)]
	[InlineData(ushort.MaxValue + 1, uint.MaxValue, BitSize.ThirtyTwo)]
	[InlineData((ulong)uint.MaxValue + 1, ulong.MaxValue, BitSize.SixtyFour)]
	public void CreateEncodingInfo_Creates_EncodingInfo_With_Correct_ItemCoordinatesBitSize(
		ulong minValue, 
		ulong maxValue,
		BitSize expectedBitSize
	)
	{
		var binFaker = new Faker<Bin<ulong>>()
			.RuleFor(x => x.Length, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.Width, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.Height, x => x.Random.ULong(0, byte.MaxValue));
		
		var itemFaker = new Faker<Item<ulong>>()
			.RuleFor(x => x.Length, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.Width, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.Height, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.X, x => x.Random.ULong(minValue, maxValue))
			.RuleFor(x => x.Y, x => x.Random.ULong(minValue, maxValue))
			.RuleFor(x => x.Z, x => x.Random.ULong(minValue, maxValue));

		var bin = binFaker.Generate(1).FirstOrDefault()!;
		var items = itemFaker.Generate(3).ToList();
		
		var encodingInfo = EncodingInfoHelper.CreateEncodingInfo<Bin<ulong>, Item<ulong>, ulong>(bin, items);
		
		encodingInfo.BinDimensionsBitSize.ShouldBe(BitSize.Eight);
		encodingInfo.ItemDimensionsBitSize.ShouldBe(BitSize.Eight);
		encodingInfo.ItemCoordinatesBitSize.ShouldBe(expectedBitSize);
	}
	
	[Theory]
	[ClassData(typeof(EncodingInfoByteDataProvider))]
	public void ToByte_Returns_Correct_Byte(
		Version version,
		BitSize binDimensionsBitSize, 
		BitSize itemDimensionsBitSize, 
		BitSize itemCoordinatesBitSize,
		byte expectedByte)
	{
		var encodingInfo = new EncodingInfo
		{
			Version = version,
			BinDimensionsBitSize = binDimensionsBitSize,
			ItemDimensionsBitSize = itemDimensionsBitSize,
			ItemCoordinatesBitSize = itemCoordinatesBitSize
		};
		
		var actualByte = EncodingInfoHelper.ToByte(encodingInfo);
		
		actualByte.ShouldBe(expectedByte);
	}
	
	[Theory]
	[ClassData(typeof(EncodingInfoByteDataProvider))]
	public void FromByte_Returns_Correct_EncodingInfo(
		Version expectedVersion,
		BitSize expectedBinDimensionsBitSize, 
		BitSize expectedItemDimensionsBitSize, 
		BitSize expectedItemCoordinatesBitSize,
		byte firstByte)
	{
		var expectedEncodingInfo = new EncodingInfo
		{
			Version = expectedVersion,
			BinDimensionsBitSize = expectedBinDimensionsBitSize,
			ItemDimensionsBitSize = expectedItemDimensionsBitSize,
			ItemCoordinatesBitSize = expectedItemCoordinatesBitSize
		};
		
		var actualEncodingInfo = EncodingInfoHelper.FromByte(firstByte);
		
		actualEncodingInfo.ShouldBe(expectedEncodingInfo);
	}
	
}
