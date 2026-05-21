using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.UnitTests.Models;

namespace Binacle.ViPaq.UnitTests;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class BitSizeHelperBehaviorTests
{
	public BitSizeHelperBehaviorTests()
	{
		Randomizer.Seed	= new Random(605080);
	}
	
	#region Dimensions
	
	[Fact]
	public void GetDimensionsBitSize_Throws_ArgumentOutOfRangeException_When_Length_Is_Zero()
	{
		var binFaker = new Faker<Bin<int>>()
			.RuleFor(x => x.Length, x => 0)
			.RuleFor(x => x.Width, x => x.Random.Int(1))
			.RuleFor(x => x.Height, x => x.Random.Int(1));
		
		var bin = binFaker.Generate(1).FirstOrDefault()!;
		
		Should.Throw<ArgumentOutOfRangeException>(() => BitSizeHelper.GetDimensionsBitSize<Bin<int>, int>(bin))
			.ParamName.ShouldBe(nameof(bin.Length));
	}
	
	[Fact]
	public void GetDimensionsBitSize_Throws_ArgumentOutOfRangeException_When_Width_Is_Zero()
	{
		var binFaker = new Faker<Bin<int>>()
			.RuleFor(x => x.Length, x => x.Random.Int(1))
			.RuleFor(x => x.Width, x => 0)
			.RuleFor(x => x.Height, x => x.Random.Int(1));
		
		var bin = binFaker.Generate(1).FirstOrDefault()!;
		
		Should.Throw<ArgumentOutOfRangeException>(() => BitSizeHelper.GetDimensionsBitSize<Bin<int>, int>(bin))
			.ParamName.ShouldBe(nameof(bin.Width));
	}
	
	[Fact]
	public void GetDimensionsBitSize_Throws_ArgumentOutOfRangeException_When_Height_Is_Zero()
	{
		var binFaker = new Faker<Bin<int>>()
			.RuleFor(x => x.Length, x => x.Random.Int(1))
			.RuleFor(x => x.Width, x => x.Random.Int(1))
			.RuleFor(x => x.Height, x => 0);
		
		var bin = binFaker.Generate(1).FirstOrDefault()!;
		
		Should.Throw<ArgumentOutOfRangeException>(() => BitSizeHelper.GetDimensionsBitSize<Bin<int>, int>(bin))
			.ParamName.ShouldBe(nameof(bin.Height));
	}
	#endregion

	#region  Coordinates
	[Fact]
	public void GetCoordinatesBitSize_Throws_ArgumentOutOfRangeException_When_X_Is_Less_Than_Zero()
	{
		var itemFaker = new Faker<Item<int>>()
			.RuleFor(x => x.Length, x => x.Random.Int())
			.RuleFor(x => x.Width, x => x.Random.Int())
			.RuleFor(x => x.Height, x => x.Random.Int())
			.RuleFor(x => x.X, x => -1)
			.RuleFor(x => x.Y, x => x.Random.Int(0))
			.RuleFor(x => x.Z, x => x.Random.Int(0));
		
		var item = itemFaker.Generate(1).FirstOrDefault()!;
		
		Should.Throw<ArgumentOutOfRangeException>(() => BitSizeHelper.GetCoordinatesBitSize<Item<int>, int>(item))
			.ParamName.ShouldBe(nameof(item.X));
	}
	
	[Fact]
	public void GetCoordinatesBitSize_Throws_ArgumentOutOfRangeException_When_Y_Is_Less_Than_Zero()
	{
		var itemFaker = new Faker<Item<int>>()
			.RuleFor(x => x.Length, x => x.Random.Int())
			.RuleFor(x => x.Width, x => x.Random.Int())
			.RuleFor(x => x.Height, x => x.Random.Int())
			.RuleFor(x => x.X, x => x.Random.Int(0))
			.RuleFor(x => x.Y, x => -1)
			.RuleFor(x => x.Z, x => x.Random.Int(0));
		
		var item = itemFaker.Generate(1).FirstOrDefault()!;
		
		Should.Throw<ArgumentOutOfRangeException>(() => BitSizeHelper.GetCoordinatesBitSize<Item<int>, int>(item))
			.ParamName.ShouldBe(nameof(item.Y));
	}
	
	[Fact]
	public void GetCoordinatesBitSize_Throws_ArgumentOutOfRangeException_When_Z_Is_Less_Than_Zero()
	{
		var itemFaker = new Faker<Item<int>>()
			.RuleFor(x => x.Length, x => x.Random.Int())
			.RuleFor(x => x.Width, x => x.Random.Int())
			.RuleFor(x => x.Height, x => x.Random.Int())
			.RuleFor(x => x.X, x => x.Random.Int(0))
			.RuleFor(x => x.Y, x => x.Random.Int(0))
			.RuleFor(x => x.Z, x => -1);
		
		var item = itemFaker.Generate(1).FirstOrDefault()!;
		
		Should.Throw<ArgumentOutOfRangeException>(() => BitSizeHelper.GetCoordinatesBitSize<Item<int>, int>(item))
			.ParamName.ShouldBe(nameof(item.Z));
	}
	#endregion
}
