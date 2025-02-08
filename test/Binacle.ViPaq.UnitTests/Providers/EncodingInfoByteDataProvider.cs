using System.Collections;
using Version = Binacle.ViPaq.Version;

namespace Binacle.ViPaq.UnitTests.Providers;

internal class EncodingInfoByteDataProvider : IEnumerable<object[]>
{
	public IEnumerator<object[]> GetEnumerator()
	{
		yield return new object[] { Version.Uncompressed, BitSize.Eight, BitSize.Eight, BitSize.Eight, 0b00_00_00_00 };
		yield return new object[] { Version.Uncompressed, BitSize.Eight, BitSize.Eight, BitSize.Sixteen, 0b00_00_00_01 };
		yield return new object[] { Version.Uncompressed, BitSize.Eight, BitSize.Eight, BitSize.ThirtyTwo, 0b00_00_00_10 };
		yield return new object[] { Version.Uncompressed, BitSize.Eight, BitSize.Eight, BitSize.SixtyFour, 0b00_00_00_11 };
		
		yield return new object[] { Version.Uncompressed, BitSize.Eight, BitSize.Sixteen, BitSize.Eight, 0b00_00_01_00 };
		yield return new object[] { Version.Uncompressed, BitSize.Eight, BitSize.Sixteen, BitSize.Sixteen, 0b00_00_01_01 };
		yield return new object[] { Version.Uncompressed, BitSize.Eight, BitSize.Sixteen, BitSize.ThirtyTwo, 0b00_00_01_10 };
		yield return new object[] { Version.Uncompressed, BitSize.Eight, BitSize.Sixteen, BitSize.SixtyFour, 0b00_00_01_11 };
		
		yield return new object[] { Version.Uncompressed, BitSize.Eight, BitSize.ThirtyTwo, BitSize.Eight, 0b00_00_10_00 };
		yield return new object[] { Version.Uncompressed, BitSize.Eight, BitSize.ThirtyTwo, BitSize.Sixteen, 0b00_00_10_01 };
		yield return new object[] { Version.Uncompressed, BitSize.Eight, BitSize.ThirtyTwo, BitSize.ThirtyTwo, 0b00_00_10_10 };
		yield return new object[] { Version.Uncompressed, BitSize.Eight, BitSize.ThirtyTwo, BitSize.SixtyFour, 0b00_00_10_11 };
		
		yield return new object[] { Version.Uncompressed, BitSize.Eight, BitSize.SixtyFour, BitSize.Eight, 0b00_00_11_00 };
		yield return new object[] { Version.Uncompressed, BitSize.Eight, BitSize.SixtyFour, BitSize.Sixteen, 0b00_00_11_01 };
		yield return new object[] { Version.Uncompressed, BitSize.Eight, BitSize.SixtyFour, BitSize.ThirtyTwo, 0b00_00_11_10 };
		yield return new object[] { Version.Uncompressed, BitSize.Eight, BitSize.SixtyFour, BitSize.SixtyFour, 0b00_00_11_11 };
		
		yield return new object[] { Version.Uncompressed, BitSize.Sixteen, BitSize.Eight, BitSize.Eight, 0b00_01_00_00 };
		yield return new object[] { Version.Uncompressed, BitSize.Sixteen, BitSize.Eight, BitSize.Sixteen, 0b00_01_00_01 };
		yield return new object[] { Version.Uncompressed, BitSize.Sixteen, BitSize.Eight, BitSize.ThirtyTwo, 0b00_01_00_10 };
		yield return new object[] { Version.Uncompressed, BitSize.Sixteen, BitSize.Eight, BitSize.SixtyFour, 0b00_01_00_11 };
		
		yield return new object[] { Version.Uncompressed, BitSize.Sixteen, BitSize.Sixteen, BitSize.Eight, 0b00_01_01_00 };
		yield return new object[] { Version.Uncompressed, BitSize.Sixteen, BitSize.Sixteen, BitSize.Sixteen, 0b00_01_01_01 };
		yield return new object[] { Version.Uncompressed, BitSize.Sixteen, BitSize.Sixteen, BitSize.ThirtyTwo, 0b00_01_01_10 };
		yield return new object[] { Version.Uncompressed, BitSize.Sixteen, BitSize.Sixteen, BitSize.SixtyFour, 0b00_01_01_11 };
		
		yield return new object[] { Version.Uncompressed, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.Eight, 0b00_01_10_00 };
		yield return new object[] { Version.Uncompressed, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.Sixteen, 0b00_01_10_01 };
		yield return new object[] { Version.Uncompressed, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.ThirtyTwo, 0b00_01_10_10 };
		yield return new object[] { Version.Uncompressed, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.SixtyFour, 0b00_01_10_11 };
		
		yield return new object[] { Version.Uncompressed, BitSize.Sixteen, BitSize.SixtyFour, BitSize.Eight, 0b00_01_11_00 };
		yield return new object[] { Version.Uncompressed, BitSize.Sixteen, BitSize.SixtyFour, BitSize.Sixteen, 0b00_01_11_01 };
		yield return new object[] { Version.Uncompressed, BitSize.Sixteen, BitSize.SixtyFour, BitSize.ThirtyTwo, 0b00_01_11_10 };
		yield return new object[] { Version.Uncompressed, BitSize.Sixteen, BitSize.SixtyFour, BitSize.SixtyFour, 0b00_01_11_11 };
		
		yield return new object[] { Version.Uncompressed, BitSize.ThirtyTwo, BitSize.Eight, BitSize.Eight, 0b00_10_00_00 };
		yield return new object[] { Version.Uncompressed, BitSize.ThirtyTwo, BitSize.Eight, BitSize.Sixteen, 0b00_10_00_01 };
		yield return new object[] { Version.Uncompressed, BitSize.ThirtyTwo, BitSize.Eight, BitSize.ThirtyTwo, 0b00_10_00_10 };
		yield return new object[] { Version.Uncompressed, BitSize.ThirtyTwo, BitSize.Eight, BitSize.SixtyFour, 0b00_10_00_11 };
		
		yield return new object[] { Version.Uncompressed, BitSize.ThirtyTwo, BitSize.Sixteen, BitSize.Eight, 0b00_10_01_00 };
		yield return new object[] { Version.Uncompressed, BitSize.ThirtyTwo, BitSize.Sixteen, BitSize.Sixteen, 0b00_10_01_01 };
		yield return new object[] { Version.Uncompressed, BitSize.ThirtyTwo, BitSize.Sixteen, BitSize.ThirtyTwo, 0b00_10_01_10 };
		yield return new object[] { Version.Uncompressed, BitSize.ThirtyTwo, BitSize.Sixteen, BitSize.SixtyFour, 0b00_10_01_11 };
		
		yield return new object[] { Version.Uncompressed, BitSize.ThirtyTwo, BitSize.ThirtyTwo, BitSize.Eight, 0b00_10_10_00 };
		yield return new object[] { Version.Uncompressed, BitSize.ThirtyTwo, BitSize.ThirtyTwo, BitSize.Sixteen, 0b00_10_10_01 };
		yield return new object[] { Version.Uncompressed, BitSize.ThirtyTwo, BitSize.ThirtyTwo, BitSize.ThirtyTwo, 0b00_10_10_10 };
		yield return new object[] { Version.Uncompressed, BitSize.ThirtyTwo, BitSize.ThirtyTwo, BitSize.SixtyFour, 0b00_10_10_11 };
		
		yield return new object[] { Version.Uncompressed, BitSize.ThirtyTwo, BitSize.SixtyFour, BitSize.Eight, 0b00_10_11_00 };
		yield return new object[] { Version.Uncompressed, BitSize.ThirtyTwo, BitSize.SixtyFour, BitSize.Sixteen, 0b00_10_11_01 };
		yield return new object[] { Version.Uncompressed, BitSize.ThirtyTwo, BitSize.SixtyFour, BitSize.ThirtyTwo, 0b00_10_11_10 };
		yield return new object[] { Version.Uncompressed, BitSize.ThirtyTwo, BitSize.SixtyFour, BitSize.SixtyFour, 0b00_10_11_11 };
		
		yield return new object[] { Version.Uncompressed, BitSize.SixtyFour, BitSize.Eight, BitSize.Eight, 0b00_11_00_00 };
		yield return new object[] { Version.Uncompressed, BitSize.SixtyFour, BitSize.Eight, BitSize.Sixteen, 0b00_11_00_01 };
		yield return new object[] { Version.Uncompressed, BitSize.SixtyFour, BitSize.Eight, BitSize.ThirtyTwo, 0b00_11_00_10 };
		yield return new object[] { Version.Uncompressed, BitSize.SixtyFour, BitSize.Eight, BitSize.SixtyFour, 0b00_11_00_11 };
		
		yield return new object[] { Version.Uncompressed, BitSize.SixtyFour, BitSize.Sixteen, BitSize.Eight, 0b00_11_01_00 };
		yield return new object[] { Version.Uncompressed, BitSize.SixtyFour, BitSize.Sixteen, BitSize.Sixteen, 0b00_11_01_01 };
		yield return new object[] { Version.Uncompressed, BitSize.SixtyFour, BitSize.Sixteen, BitSize.ThirtyTwo, 0b00_11_01_10 };
		yield return new object[] { Version.Uncompressed, BitSize.SixtyFour, BitSize.Sixteen, BitSize.SixtyFour, 0b00_11_01_11 };
		
		yield return new object[] { Version.Uncompressed, BitSize.SixtyFour, BitSize.ThirtyTwo, BitSize.Eight, 0b00_11_10_00 };
		yield return new object[] { Version.Uncompressed, BitSize.SixtyFour, BitSize.ThirtyTwo, BitSize.Sixteen, 0b00_11_10_01 };
		yield return new object[] { Version.Uncompressed, BitSize.SixtyFour, BitSize.ThirtyTwo, BitSize.ThirtyTwo, 0b00_11_10_10 };
		yield return new object[] { Version.Uncompressed, BitSize.SixtyFour, BitSize.ThirtyTwo, BitSize.SixtyFour, 0b00_11_10_11 };
		
		yield return new object[] { Version.Uncompressed, BitSize.SixtyFour, BitSize.SixtyFour, BitSize.Eight, 0b00_11_11_00 };
		yield return new object[] { Version.Uncompressed, BitSize.SixtyFour, BitSize.SixtyFour, BitSize.Sixteen, 0b00_11_11_01 };
		yield return new object[] { Version.Uncompressed, BitSize.SixtyFour, BitSize.SixtyFour, BitSize.ThirtyTwo, 0b00_11_11_10 };
		yield return new object[] { Version.Uncompressed, BitSize.SixtyFour, BitSize.SixtyFour, BitSize.SixtyFour, 0b00_11_11_11 };
		
		yield return new object[] { Version.CompressedGzip, BitSize.Eight, BitSize.Eight, BitSize.Eight, 0b01_00_00_00 };
		yield return new object[] { Version.CompressedGzip, BitSize.Eight, BitSize.Eight, BitSize.Sixteen, 0b01_00_00_01 };
		yield return new object[] { Version.CompressedGzip, BitSize.Eight, BitSize.Eight, BitSize.ThirtyTwo, 0b01_00_00_10 };
		yield return new object[] { Version.CompressedGzip, BitSize.Eight, BitSize.Eight, BitSize.SixtyFour, 0b01_00_00_11 };
		
		yield return new object[] { Version.CompressedGzip, BitSize.Eight, BitSize.Sixteen, BitSize.Eight, 0b01_00_01_00 };
		yield return new object[] { Version.CompressedGzip, BitSize.Eight, BitSize.Sixteen, BitSize.Sixteen, 0b01_00_01_01 };
		yield return new object[] { Version.CompressedGzip, BitSize.Eight, BitSize.Sixteen, BitSize.ThirtyTwo, 0b01_00_01_10 };
		yield return new object[] { Version.CompressedGzip, BitSize.Eight, BitSize.Sixteen, BitSize.SixtyFour, 0b01_00_01_11 };
		
		yield return new object[] { Version.CompressedGzip, BitSize.Eight, BitSize.ThirtyTwo, BitSize.Eight, 0b01_00_10_00 };
		yield return new object[] { Version.CompressedGzip, BitSize.Eight, BitSize.ThirtyTwo, BitSize.Sixteen, 0b01_00_10_01 };
		yield return new object[] { Version.CompressedGzip, BitSize.Eight, BitSize.ThirtyTwo, BitSize.ThirtyTwo, 0b01_00_10_10 };
		yield return new object[] { Version.CompressedGzip, BitSize.Eight, BitSize.ThirtyTwo, BitSize.SixtyFour, 0b01_00_10_11 };
		
		yield return new object[] { Version.CompressedGzip, BitSize.Eight, BitSize.SixtyFour, BitSize.Eight, 0b01_00_11_00 };
		yield return new object[] { Version.CompressedGzip, BitSize.Eight, BitSize.SixtyFour, BitSize.Sixteen, 0b01_00_11_01 };
		yield return new object[] { Version.CompressedGzip, BitSize.Eight, BitSize.SixtyFour, BitSize.ThirtyTwo, 0b01_00_11_10 };
		yield return new object[] { Version.CompressedGzip, BitSize.Eight, BitSize.SixtyFour, BitSize.SixtyFour, 0b01_00_11_11 };
		
		yield return new object[] { Version.CompressedGzip, BitSize.Sixteen, BitSize.Eight, BitSize.Eight, 0b01_01_00_00 };
		yield return new object[] { Version.CompressedGzip, BitSize.Sixteen, BitSize.Eight, BitSize.Sixteen, 0b01_01_00_01 };
		yield return new object[] { Version.CompressedGzip, BitSize.Sixteen, BitSize.Eight, BitSize.ThirtyTwo, 0b01_01_00_10 };
		yield return new object[] { Version.CompressedGzip, BitSize.Sixteen, BitSize.Eight, BitSize.SixtyFour, 0b01_01_00_11 };
		
		yield return new object[] { Version.CompressedGzip, BitSize.Sixteen, BitSize.Sixteen, BitSize.Eight, 0b01_01_01_00 };
		yield return new object[] { Version.CompressedGzip, BitSize.Sixteen, BitSize.Sixteen, BitSize.Sixteen, 0b01_01_01_01 };
		yield return new object[] { Version.CompressedGzip, BitSize.Sixteen, BitSize.Sixteen, BitSize.ThirtyTwo, 0b01_01_01_10 };
		yield return new object[] { Version.CompressedGzip, BitSize.Sixteen, BitSize.Sixteen, BitSize.SixtyFour, 0b01_01_01_11 };
		
		yield return new object[] { Version.CompressedGzip, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.Eight, 0b01_01_10_00 };
		yield return new object[] { Version.CompressedGzip, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.Sixteen, 0b01_01_10_01 };
		yield return new object[] { Version.CompressedGzip, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.ThirtyTwo, 0b01_01_10_10 };
		yield return new object[] { Version.CompressedGzip, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.SixtyFour, 0b01_01_10_11 };
		
		yield return new object[] { Version.CompressedGzip, BitSize.Sixteen, BitSize.SixtyFour, BitSize.Eight, 0b01_01_11_00 };
		yield return new object[] { Version.CompressedGzip, BitSize.Sixteen, BitSize.SixtyFour, BitSize.Sixteen, 0b01_01_11_01 };
		yield return new object[] { Version.CompressedGzip, BitSize.Sixteen, BitSize.SixtyFour, BitSize.ThirtyTwo, 0b01_01_11_10 };
		yield return new object[] { Version.CompressedGzip, BitSize.Sixteen, BitSize.SixtyFour, BitSize.SixtyFour, 0b01_01_11_11 };
		
		yield return new object[] { Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.Eight, BitSize.Eight, 0b01_10_00_00 };
		yield return new object[] { Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.Eight, BitSize.Sixteen, 0b01_10_00_01 };
		yield return new object[] { Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.Eight, BitSize.ThirtyTwo, 0b01_10_00_10 };
		yield return new object[] { Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.Eight, BitSize.SixtyFour, 0b01_10_00_11 };
		
		yield return new object[] { Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.Sixteen, BitSize.Eight, 0b01_10_01_00 };
		yield return new object[] { Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.Sixteen, BitSize.Sixteen, 0b01_10_01_01 };
		yield return new object[] { Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.Sixteen, BitSize.ThirtyTwo, 0b01_10_01_10 };
		yield return new object[] { Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.Sixteen, BitSize.SixtyFour, 0b01_10_01_11 };
		
		yield return new object[] { Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.ThirtyTwo, BitSize.Eight, 0b01_10_10_00 };
		yield return new object[] { Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.ThirtyTwo, BitSize.Sixteen, 0b01_10_10_01 };
		yield return new object[] { Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.ThirtyTwo, BitSize.ThirtyTwo, 0b01_10_10_10 };
		yield return new object[] { Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.ThirtyTwo, BitSize.SixtyFour, 0b01_10_10_11 };
		
		yield return new object[] { Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.SixtyFour, BitSize.Eight, 0b01_10_11_00 };
		yield return new object[] { Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.SixtyFour, BitSize.Sixteen, 0b01_10_11_01 };
		yield return new object[] { Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.SixtyFour, BitSize.ThirtyTwo, 0b01_10_11_10 };
		yield return new object[] { Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.SixtyFour, BitSize.SixtyFour, 0b01_10_11_11 };
		
		yield return new object[] { Version.CompressedGzip, BitSize.SixtyFour, BitSize.Eight, BitSize.Eight, 0b01_11_00_00 };
		yield return new object[] { Version.CompressedGzip, BitSize.SixtyFour, BitSize.Eight, BitSize.Sixteen, 0b01_11_00_01 };
		yield return new object[] { Version.CompressedGzip, BitSize.SixtyFour, BitSize.Eight, BitSize.ThirtyTwo, 0b01_11_00_10 };
		yield return new object[] { Version.CompressedGzip, BitSize.SixtyFour, BitSize.Eight, BitSize.SixtyFour, 0b01_11_00_11 };
		
		yield return new object[] { Version.CompressedGzip, BitSize.SixtyFour, BitSize.Sixteen, BitSize.Eight, 0b01_11_01_00 };
		yield return new object[] { Version.CompressedGzip, BitSize.SixtyFour, BitSize.Sixteen, BitSize.Sixteen, 0b01_11_01_01 };
		yield return new object[] { Version.CompressedGzip, BitSize.SixtyFour, BitSize.Sixteen, BitSize.ThirtyTwo, 0b01_11_01_10 };
		yield return new object[] { Version.CompressedGzip, BitSize.SixtyFour, BitSize.Sixteen, BitSize.SixtyFour, 0b01_11_01_11 };
		
		yield return new object[] { Version.CompressedGzip, BitSize.SixtyFour, BitSize.ThirtyTwo, BitSize.Eight, 0b01_11_10_00 };
		yield return new object[] { Version.CompressedGzip, BitSize.SixtyFour, BitSize.ThirtyTwo, BitSize.Sixteen, 0b01_11_10_01 };
		yield return new object[] { Version.CompressedGzip, BitSize.SixtyFour, BitSize.ThirtyTwo, BitSize.ThirtyTwo, 0b01_11_10_10 };
		yield return new object[] { Version.CompressedGzip, BitSize.SixtyFour, BitSize.ThirtyTwo, BitSize.SixtyFour, 0b01_11_10_11 };
		
		yield return new object[] { Version.CompressedGzip, BitSize.SixtyFour, BitSize.SixtyFour, BitSize.Eight, 0b01_11_11_00 };
		yield return new object[] { Version.CompressedGzip, BitSize.SixtyFour, BitSize.SixtyFour, BitSize.Sixteen, 0b01_11_11_01 };
		yield return new object[] { Version.CompressedGzip, BitSize.SixtyFour, BitSize.SixtyFour, BitSize.ThirtyTwo, 0b01_11_11_10 };
		yield return new object[] { Version.CompressedGzip, BitSize.SixtyFour, BitSize.SixtyFour, BitSize.SixtyFour, 0b01_11_11_11 };
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	
}
