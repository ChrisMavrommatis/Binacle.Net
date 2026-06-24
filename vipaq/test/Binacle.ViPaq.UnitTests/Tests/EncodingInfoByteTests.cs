using Binacle.ViPaq.UnitTests.Providers;
using Version = Binacle.ViPaq.Version;

namespace Binacle.ViPaq.UnitTests;

// The header byte. ToByte packs the version and the three section sizes into one byte,
// FromByte unpacks it back. One data table (version, three sizes, expected byte) grades
// both directions and the round trip between them.
[Trait("Result Tests", "Ensures results are as expected")]
public class EncodingInfoByteTests
{
	[Theory]
	[ClassData(typeof(EncodingInfoByteDataProvider))]
	public void ToByte_Returns_Correct_Byte(
		Version version,
		BitSize binSize,
		BitSize itemDimensionsSize,
		BitSize itemCoordinatesSize,
		byte expectedByte)
	{
		var encodingInfo = new EncodingInfo
		{
			Version = version,
			BinDimensionsBitSize = binSize,
			ItemDimensionsBitSize = itemDimensionsSize,
			ItemCoordinatesBitSize = itemCoordinatesSize
		};

		EncodingInfoHelper.ToByte(encodingInfo).ShouldBe(expectedByte);
	}

	[Theory]
	[ClassData(typeof(EncodingInfoByteDataProvider))]
	public void FromByte_Returns_Correct_EncodingInfo(
		Version version,
		BitSize binSize,
		BitSize itemDimensionsSize,
		BitSize itemCoordinatesSize,
		byte firstByte)
	{
		var expected = new EncodingInfo
		{
			Version = version,
			BinDimensionsBitSize = binSize,
			ItemDimensionsBitSize = itemDimensionsSize,
			ItemCoordinatesBitSize = itemCoordinatesSize
		};

		EncodingInfoHelper.FromByte(firstByte).ShouldBe(expected);
	}

	[Theory]
	[ClassData(typeof(EncodingInfoByteDataProvider))]
	public void ToByte_Then_FromByte_Returns_Original(
		Version version,
		BitSize binSize,
		BitSize itemDimensionsSize,
		BitSize itemCoordinatesSize,
		byte expectedByte)
	{
		var original = new EncodingInfo
		{
			Version = version,
			BinDimensionsBitSize = binSize,
			ItemDimensionsBitSize = itemDimensionsSize,
			ItemCoordinatesBitSize = itemCoordinatesSize
		};

		var asByte = EncodingInfoHelper.ToByte(original);
		asByte.ShouldBe(expectedByte);

		var roundTripped = EncodingInfoHelper.FromByte(asByte);
		roundTripped.ShouldBe(original);
	}
}
