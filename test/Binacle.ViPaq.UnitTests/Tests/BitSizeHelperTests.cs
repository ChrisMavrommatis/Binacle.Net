using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.Models;
using Bogus;

namespace Binacle.ViPaq.UnitTests;

[Trait("Result Tests", "Ensures results are as expected")]
public class BitSizeHelperTests
{
	public BitSizeHelperTests()
	{
		Randomizer.Seed = new Random(605080);
	}
	#region Dimensions

	[Fact]
	public void GetDimensionsBitSize_Returns_Eight_BitSize_When_Dimensions_Are_Less_Than_Or_Equal_To_Byte_MaxValue()
	{
		var binFaker = new Bogus.Faker<Models.Bin<ulong>>()
			.RuleFor(x => x.Length, x => x.Random.ULong())
			.RuleFor(x => x.Width, x => x.Random.ULong())
			.RuleFor(x => x.Height, x => x.Random.ULong())
			.RuleFor(x => x.Length, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.Width, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.Height, x => x.Random.ULong(0, byte.MaxValue));
		
		var bin = binFaker.Generate(1).FirstOrDefault()!;
		
		BitSizeHelper.GetDimensionsBitSize<Models.Bin<ulong>, ulong>(bin)
			.ShouldBe(BitSize.Eight);
	}
	
	[Fact]
	public void GetDimensionsBitSize_Returns_Sixteen_BitSize_When_Dimensions_Are_Less_Than_Or_Equal_To_UShort_MaxValue()
	{
		var binFaker = new Bogus.Faker<Models.Bin<ulong>>()
			.RuleFor(x => x.Length, x => x.Random.ULong())
			.RuleFor(x => x.Width, x => x.Random.ULong())
			.RuleFor(x => x.Height, x => x.Random.ULong())
			.RuleFor(x => x.Length, x => x.Random.ULong(byte.MaxValue + 1, ushort.MaxValue))
			.RuleFor(x => x.Width, x => x.Random.ULong(byte.MaxValue + 1, ushort.MaxValue))
			.RuleFor(x => x.Height, x => x.Random.ULong(byte.MaxValue + 1, ushort.MaxValue));
		
		var bin = binFaker.Generate(1).FirstOrDefault()!;
		
		BitSizeHelper.GetDimensionsBitSize<Models.Bin<ulong>, ulong>(bin)
			.ShouldBe(BitSize.Sixteen);
	}
	
	[Fact]
	public void GetDimensionsBitSize_Returns_ThirtyTwo_BitSize_When_Dimensions_Are_Less_Than_Or_Equal_To_UInt_MaxValue()
	{
		var binFaker = new Bogus.Faker<Models.Bin<ulong>>()
			.RuleFor(x => x.Length, x => x.Random.ULong())
			.RuleFor(x => x.Width, x => x.Random.ULong())
			.RuleFor(x => x.Height, x => x.Random.ULong())
			.RuleFor(x => x.Length, x => x.Random.ULong(ushort.MaxValue + 1, uint.MaxValue))
			.RuleFor(x => x.Width, x => x.Random.ULong(ushort.MaxValue + 1, uint.MaxValue))
			.RuleFor(x => x.Height, x => x.Random.ULong(ushort.MaxValue + 1, uint.MaxValue));
		
		var bin = binFaker.Generate(1).FirstOrDefault()!;
		
		BitSizeHelper.GetDimensionsBitSize<Models.Bin<ulong>, ulong>(bin)
			.ShouldBe(BitSize.ThirtyTwo);
	}
	
	[Fact]
	public void GetDimensionsBitSize_Returns_SixtyFour_BitSize_When_Dimensions_Are_Less_Than_Or_Equal_To_ULong_MaxValue()
	{
		var binFaker = new Bogus.Faker<Models.Bin<ulong>>()
			.RuleFor(x => x.Length, x => x.Random.ULong())
			.RuleFor(x => x.Width, x => x.Random.ULong())
			.RuleFor(x => x.Height, x => x.Random.ULong())
			.RuleFor(x => x.Length, x => x.Random.ULong((ulong)uint.MaxValue + 1, ulong.MaxValue))
			.RuleFor(x => x.Width, x => x.Random.ULong((ulong)uint.MaxValue + 1, ulong.MaxValue))
			.RuleFor(x => x.Height, x => x.Random.ULong((ulong)uint.MaxValue + 1, ulong.MaxValue));
		
		var bin = binFaker.Generate(1).FirstOrDefault()!;
		
		BitSizeHelper.GetDimensionsBitSize<Models.Bin<ulong>, ulong>(bin)
			.ShouldBe(BitSize.SixtyFour);
	}
	#endregion

	#region  Coordinates
	[Fact]
	public void GetCoordinatesBitSize_Returns_Eight_BitSize_When_Coordinates_Are_Less_Than_Or_Equal_To_Byte_MaxValue()
	{
		var itemFaker = new Bogus.Faker<Models.Item<ulong>>()
			.RuleFor(x => x.Length, x => x.Random.ULong())
			.RuleFor(x => x.Width, x => x.Random.ULong())
			.RuleFor(x => x.Height, x => x.Random.ULong())
			.RuleFor(x => x.X, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.Y, x => x.Random.ULong(0, byte.MaxValue))
			.RuleFor(x => x.Z, x => x.Random.ULong(0, byte.MaxValue));
		
		var item = itemFaker.Generate(1).FirstOrDefault()!;
		
		BitSizeHelper.GetCoordinatesBitSize<Models.Item<ulong>, ulong>(item)
			.ShouldBe(BitSize.Eight);
	}
	
	[Fact]
	public void GetCoordinatesBitSize_Returns_Sixteen_BitSize_When_Coordinates_Are_Less_Than_Or_Equal_To_UShort_MaxValue()
	{
		var itemFaker = new Bogus.Faker<Models.Item<ulong>>()
			.RuleFor(x => x.Length, x => x.Random.ULong())
			.RuleFor(x => x.Width, x => x.Random.ULong())
			.RuleFor(x => x.Height, x => x.Random.ULong())
			.RuleFor(x => x.X, x => x.Random.ULong(byte.MaxValue + 1, ushort.MaxValue))
			.RuleFor(x => x.Y, x => x.Random.ULong(byte.MaxValue + 1, ushort.MaxValue))
			.RuleFor(x => x.Z, x => x.Random.ULong(byte.MaxValue + 1, ushort.MaxValue));
		
		var item = itemFaker.Generate(1).FirstOrDefault()!;
		
		BitSizeHelper.GetCoordinatesBitSize<Models.Item<ulong>, ulong>(item)
			.ShouldBe(BitSize.Sixteen);
	}
	
	[Fact]
	public void GetCoordinatesBitSize_Returns_ThirtyTwo_BitSize_When_Coordinates_Are_Less_Than_Or_Equal_To_UInt_MaxValue()
	{
		var itemFaker = new Bogus.Faker<Models.Item<ulong>>()
			.RuleFor(x => x.Length, x => x.Random.ULong())
			.RuleFor(x => x.Width, x => x.Random.ULong())
			.RuleFor(x => x.Height, x => x.Random.ULong())
			.RuleFor(x => x.X, x => x.Random.ULong(ushort.MaxValue + 1, uint.MaxValue))
			.RuleFor(x => x.Y, x => x.Random.ULong(ushort.MaxValue + 1, uint.MaxValue))
			.RuleFor(x => x.Z, x => x.Random.ULong(ushort.MaxValue + 1, uint.MaxValue));
		
		var item = itemFaker.Generate(1).FirstOrDefault()!;
		
		BitSizeHelper.GetCoordinatesBitSize<Models.Item<ulong>, ulong>(item)
			.ShouldBe(BitSize.ThirtyTwo);
	}
	
	[Fact]
	public void GetCoordinatesBitSize_Returns_SixtyFour_BitSize_When_Coordinates_Are_Less_Than_Or_Equal_To_ULong_MaxValue()
	{
		var itemFaker = new Bogus.Faker<Models.Item<ulong>>()
			.RuleFor(x => x.Length, x => x.Random.ULong())
			.RuleFor(x => x.Width, x => x.Random.ULong())
			.RuleFor(x => x.Height, x => x.Random.ULong())
			.RuleFor(x => x.X, x => x.Random.ULong((ulong)uint.MaxValue + 1, ulong.MaxValue))
			.RuleFor(x => x.Y, x => x.Random.ULong((ulong)uint.MaxValue + 1, ulong.MaxValue))
			.RuleFor(x => x.Z, x => x.Random.ULong((ulong)uint.MaxValue + 1, ulong.MaxValue));
		
		var item = itemFaker.Generate(1).FirstOrDefault()!;
		
		BitSizeHelper.GetCoordinatesBitSize<Models.Item<ulong>, ulong>(item)
			.ShouldBe(BitSize.SixtyFour);
	}
	#endregion
}
