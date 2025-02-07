using Binacle.ViPaq.Models;
using Version = Binacle.ViPaq.Models.Version;

namespace Binacle.ViPaq.UnitTests;

[Trait("Result Tests", "Ensures results are as expected")]
public class EncodingInfoTests
{
	[Theory]
	[ClassData(typeof(Providers.EncodingInfoByteDataProvider))]
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
		
		var actualByte = EncodingInfo.ToByte(encodingInfo);
		
		actualByte.ShouldBe(expectedByte);
	}
	
	[Theory]
	[ClassData(typeof(Providers.EncodingInfoByteDataProvider))]
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
		
		var actualEncodingInfo = EncodingInfo.FromByte(firstByte);
		
		actualEncodingInfo.ShouldBe(expectedEncodingInfo);
	}

}
