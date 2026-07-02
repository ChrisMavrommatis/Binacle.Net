using System.Collections;

namespace Binacle.ViPaq.UnitTests.Providers;

// Round-trip matrix: a numeric type crossed with every (bin, item-dim, item-coord) size combo it
// can hold. ushort reaches 16-bit, int reaches 32-bit, ulong reaches 64-bit. Each row is one
// serialize -> deserialize check. Covers mixed widths too (e.g. small dimensions, large coordinates).
// Row: numeric type, bin size, item dimensions size, item coordinates size.
internal class SerializationRoundTripProvider : IEnumerable<object[]>
{
	private static readonly BitSize[] upTo16 = [BitSize.Eight, BitSize.Sixteen];
	private static readonly BitSize[] upTo32 = [BitSize.Eight, BitSize.Sixteen, BitSize.ThirtyTwo];
	private static readonly BitSize[] upTo64 = [BitSize.Eight, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.SixtyFour];

	public IEnumerator<object[]> GetEnumerator()
	{
		IEnumerable<object[]> rows =
		[
			.. Combos(typeof(ushort), upTo16),
			.. Combos(typeof(int), upTo32),
			.. Combos(typeof(ulong), upTo64),
		];
		return rows.GetEnumerator();
	}

	private static IEnumerable<object[]> Combos(Type numericType, BitSize[] sizes)
	{
		foreach (var binSize in sizes)
		foreach (var itemDimensionsSize in sizes)
		foreach (var itemCoordinatesSize in sizes)
		{
			yield return [numericType, binSize, itemDimensionsSize, itemCoordinatesSize];
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
