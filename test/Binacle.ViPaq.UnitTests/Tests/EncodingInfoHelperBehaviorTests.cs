using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.Models;
using Bogus;
using Version = Binacle.ViPaq.Models.Version;

namespace Binacle.ViPaq.UnitTests;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class EncodingInfoHelperBehaviorTests
{
	public EncodingInfoHelperBehaviorTests()
	{
		Randomizer.Seed	= new Random(605080);
	}

	[Fact]
	public void ThrowOnInvalidEncodingInfo_Does_Not_Throw_On_Supported_Types()
	{
		var encodingInfo = new EncodingInfo()
		{
			Version = Version.Uncompressed,
			BinDimensionsBitSize = BitSize.Eight,
			ItemDimensionsBitSize = BitSize.Eight,	
			ItemCoordinatesBitSize = BitSize.Eight   
		};
	    
		Should.NotThrow(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<sbyte>(encodingInfo));
		Should.NotThrow(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<byte>(encodingInfo));
		Should.NotThrow(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<short>(encodingInfo));
		Should.NotThrow(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<ushort>(encodingInfo));
		Should.NotThrow(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<int>(encodingInfo));
		Should.NotThrow(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<uint>(encodingInfo));
		Should.NotThrow(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<long>(encodingInfo));
		Should.NotThrow(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<ulong>(encodingInfo));
	}
	
	[Fact]
	public void ThrowOnInvalidEncodingInfo_Throws_ArgumentException_On_Unsupported_Types()
	{
		var encodingInfo = new EncodingInfo()
		{
			Version = Version.Uncompressed,
			BinDimensionsBitSize = BitSize.Eight,
			ItemDimensionsBitSize = BitSize.Eight,	
			ItemCoordinatesBitSize = BitSize.Eight   
		};
	    
		Should.Throw<ArgumentException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<char>(encodingInfo));
		Should.Throw<ArgumentException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<nint>(encodingInfo));
		Should.Throw<ArgumentException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<nuint>(encodingInfo));
	}
	
	[Fact]
	public void ThrowOnInvalidEncodingInfo_Throws_ArgumentOutOfRangeException_With_Sixteen_BitSize_And_Small_Generic_Types()
	{
		var encodingInfo = new EncodingInfo()
		{
			Version = Version.Uncompressed,
			BinDimensionsBitSize = BitSize.Sixteen,
			ItemDimensionsBitSize = BitSize.Eight,	
			ItemCoordinatesBitSize = BitSize.Eight   
		};
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<sbyte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.BinDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<byte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.BinDimensionsBitSize));
		
		encodingInfo = new EncodingInfo()
		{
			Version = Version.Uncompressed,
			BinDimensionsBitSize = BitSize.Eight,
			ItemDimensionsBitSize = BitSize.Sixteen,	
			ItemCoordinatesBitSize = BitSize.Eight   
		};
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<sbyte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<byte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemDimensionsBitSize));
		
		encodingInfo = new EncodingInfo()
		{
			Version = Version.Uncompressed,
			BinDimensionsBitSize = BitSize.Eight,
			ItemDimensionsBitSize = BitSize.Eight,	
			ItemCoordinatesBitSize = BitSize.Sixteen   
		};
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<sbyte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemCoordinatesBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<byte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemCoordinatesBitSize));
	}
	
	[Fact]
	public void ThrowOnInvalidEncodingInfo_Throws_ArgumentOutOfRangeException_With_ThirtyTwo_BitSize_And_Small_Generic_Types()
	{
		var encodingInfo = new EncodingInfo()
		{
			Version = Version.Uncompressed,
			BinDimensionsBitSize = BitSize.ThirtyTwo,
			ItemDimensionsBitSize = BitSize.Eight,	
			ItemCoordinatesBitSize = BitSize.Eight   
		};
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<sbyte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.BinDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<byte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.BinDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<short>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.BinDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<ushort>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.BinDimensionsBitSize));
		
		encodingInfo = new EncodingInfo()
		{
			Version = Version.Uncompressed,
			BinDimensionsBitSize = BitSize.Eight,
			ItemDimensionsBitSize = BitSize.ThirtyTwo,	
			ItemCoordinatesBitSize = BitSize.Eight   
		};
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<sbyte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<byte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<short>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<ushort>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemDimensionsBitSize));
		
		encodingInfo = new EncodingInfo()
		{
			Version = Version.Uncompressed,
			BinDimensionsBitSize = BitSize.Eight,
			ItemDimensionsBitSize = BitSize.Eight,	
			ItemCoordinatesBitSize = BitSize.ThirtyTwo   
		};
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<sbyte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemCoordinatesBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<byte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemCoordinatesBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<short>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemCoordinatesBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<ushort>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemCoordinatesBitSize));
	}
	
	[Fact]
	public void ThrowOnInvalidEncodingInfo_Throws_ArgumentOutOfRangeException_With_SixtyFour_BitSize_And_Small_Generic_Types()
	{
		var encodingInfo = new EncodingInfo()
		{
			Version = Version.Uncompressed,
			BinDimensionsBitSize = BitSize.SixtyFour,
			ItemDimensionsBitSize = BitSize.Eight,	
			ItemCoordinatesBitSize = BitSize.Eight   
		};
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<sbyte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.BinDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<byte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.BinDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<short>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.BinDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<ushort>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.BinDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<int>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.BinDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<uint>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.BinDimensionsBitSize));
		
		encodingInfo = new EncodingInfo()
		{
			Version = Version.Uncompressed,
			BinDimensionsBitSize = BitSize.Eight,
			ItemDimensionsBitSize = BitSize.SixtyFour,	
			ItemCoordinatesBitSize = BitSize.Eight   
		};
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<sbyte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<byte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<short>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<ushort>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<int>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemDimensionsBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<uint>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemDimensionsBitSize));

		
		encodingInfo = new EncodingInfo()
		{
			Version = Version.Uncompressed,
			BinDimensionsBitSize = BitSize.Eight,
			ItemDimensionsBitSize = BitSize.Eight,	
			ItemCoordinatesBitSize = BitSize.SixtyFour   
		};
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<sbyte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemCoordinatesBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<byte>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemCoordinatesBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<short>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemCoordinatesBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<ushort>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemCoordinatesBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<int>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemCoordinatesBitSize));
		Should.Throw<ArgumentOutOfRangeException>(() => EncodingInfoHelper.ThrowOnInvalidEncodingInfo<uint>(encodingInfo))
			.ParamName.ShouldBe(nameof(EncodingInfo.ItemCoordinatesBitSize));
	}

	
}
