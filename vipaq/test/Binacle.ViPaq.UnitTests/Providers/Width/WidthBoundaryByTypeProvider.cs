using System.Collections;

namespace Binacle.ViPaq.UnitTests.Providers;

// Each supported type at the value boundaries it can reach.
// The width steps up from Eight to Sixteen at 256. There is no wider width, so a value above the 65535
// ceiling is a reject (see WidthInvalidProvider), not a bucket here. A type caps at its own range, so a
// byte can never get to 256. Row: numeric type, value, expected width.
internal class WidthBoundaryByTypeProvider : IEnumerable<object[]>
{
	public IEnumerator<object[]> GetEnumerator()
	{
		// 8 bit types: only ever Eight.
		yield return [typeof(byte), 1UL, Width.Eight];
		yield return [typeof(byte), 255UL, Width.Eight];
		yield return [typeof(sbyte), 1UL, Width.Eight];
		yield return [typeof(sbyte), 127UL, Width.Eight];

		// 16 bit capable types: Eight up to 255, then Sixteen up to the 65535 ceiling.
		yield return [typeof(ushort), 255UL, Width.Eight];
		yield return [typeof(ushort), 256UL, Width.Sixteen];
		yield return [typeof(ushort), 65535UL, Width.Sixteen];
		yield return [typeof(short), 255UL, Width.Eight];
		yield return [typeof(short), 256UL, Width.Sixteen];
		yield return [typeof(short), 32767UL, Width.Sixteen];
		yield return [typeof(uint), 255UL, Width.Eight];
		yield return [typeof(uint), 256UL, Width.Sixteen];
		yield return [typeof(uint), 65535UL, Width.Sixteen];
		yield return [typeof(int), 255UL, Width.Eight];
		yield return [typeof(int), 256UL, Width.Sixteen];
		yield return [typeof(int), 65535UL, Width.Sixteen];
		yield return [typeof(ulong), 255UL, Width.Eight];
		yield return [typeof(ulong), 256UL, Width.Sixteen];
		yield return [typeof(ulong), 65535UL, Width.Sixteen];
		yield return [typeof(long), 255UL, Width.Eight];
		yield return [typeof(long), 256UL, Width.Sixteen];
		yield return [typeof(long), 65535UL, Width.Sixteen];
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
