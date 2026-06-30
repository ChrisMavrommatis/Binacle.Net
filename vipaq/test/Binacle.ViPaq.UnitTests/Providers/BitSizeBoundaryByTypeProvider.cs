using System.Collections;

namespace Binacle.ViPaq.UnitTests.Providers;

// Each supported type at the value boundaries it can reach.
// The storage size steps up at 256, 65536 and 4294967296. A type caps at its own width, so each
// type only reaches the boundaries its range allows (a byte can never get to 256).
// Row: numeric type, value, expected size.
internal class BitSizeBoundaryByTypeProvider : IEnumerable<object[]>
{
	public IEnumerator<object[]> GetEnumerator()
	{
		// 8 bit types: only ever Eight.
		yield return [typeof(byte), 1UL, BitSize.Eight];
		yield return [typeof(byte), 255UL, BitSize.Eight];
		yield return [typeof(sbyte), 1UL, BitSize.Eight];
		yield return [typeof(sbyte), 127UL, BitSize.Eight];

		// 16 bit types: Eight up to 255, then Sixteen.
		yield return [typeof(ushort), 255UL, BitSize.Eight];
		yield return [typeof(ushort), 256UL, BitSize.Sixteen];
		yield return [typeof(ushort), 65535UL, BitSize.Sixteen];
		yield return [typeof(short), 255UL, BitSize.Eight];
		yield return [typeof(short), 256UL, BitSize.Sixteen];
		yield return [typeof(short), 32767UL, BitSize.Sixteen];

		// 32 bit types: up to ThirtyTwo.
		yield return [typeof(uint), 255UL, BitSize.Eight];
		yield return [typeof(uint), 256UL, BitSize.Sixteen];
		yield return [typeof(uint), 65535UL, BitSize.Sixteen];
		yield return [typeof(uint), 65536UL, BitSize.ThirtyTwo];
		yield return [typeof(uint), 4294967295UL, BitSize.ThirtyTwo];
		yield return [typeof(int), 255UL, BitSize.Eight];
		yield return [typeof(int), 256UL, BitSize.Sixteen];
		yield return [typeof(int), 65535UL, BitSize.Sixteen];
		yield return [typeof(int), 65536UL, BitSize.ThirtyTwo];
		yield return [typeof(int), 2147483647UL, BitSize.ThirtyTwo];

		// 64 bit types: up to SixtyFour.
		yield return [typeof(ulong), 255UL, BitSize.Eight];
		yield return [typeof(ulong), 256UL, BitSize.Sixteen];
		yield return [typeof(ulong), 65535UL, BitSize.Sixteen];
		yield return [typeof(ulong), 65536UL, BitSize.ThirtyTwo];
		yield return [typeof(ulong), 4294967295UL, BitSize.ThirtyTwo];
		yield return [typeof(ulong), 4294967296UL, BitSize.SixtyFour];
		yield return [typeof(ulong), 9007199254740991UL, BitSize.SixtyFour]; // MaxInteger (2^53 - 1), top of the bucket
		yield return [typeof(long), 255UL, BitSize.Eight];
		yield return [typeof(long), 256UL, BitSize.Sixteen];
		yield return [typeof(long), 65535UL, BitSize.Sixteen];
		yield return [typeof(long), 65536UL, BitSize.ThirtyTwo];
		yield return [typeof(long), 4294967295UL, BitSize.ThirtyTwo];
		yield return [typeof(long), 4294967296UL, BitSize.SixtyFour];
		yield return [typeof(long), 9007199254740991UL, BitSize.SixtyFour]; // MaxInteger (2^53 - 1), top of the bucket
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
