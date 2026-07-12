using System.Collections;

namespace Binacle.ViPaq.UnitTests.Providers;

// Round-trip matrix: a numeric type crossed with every (bin, item-dim, item-coord) width combo. There are
// only two widths now (Eight/Sixteen), and every supported type reaches both, so each type runs the whole
// 2x2x2 set. Each row is one serialize -> deserialize check. Covers mixed widths too (e.g. small dimensions,
// large coordinates). Row: numeric type, bin width, item dimensions width, item coordinates width.
internal class SerializationRoundTripProvider : IEnumerable<object[]>
{
	private static readonly Width[] widths = [Width.Eight, Width.Sixteen];

	public IEnumerator<object[]> GetEnumerator()
	{
		IEnumerable<object[]> rows =
		[
			.. Combos(typeof(ushort), widths),
			.. Combos(typeof(int), widths),
			.. Combos(typeof(ulong), widths),
		];
		return rows.GetEnumerator();
	}

	private static IEnumerable<object[]> Combos(Type numericType, Width[] sizes)
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
