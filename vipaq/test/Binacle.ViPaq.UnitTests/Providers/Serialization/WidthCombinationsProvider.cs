using System.Collections;

namespace Binacle.ViPaq.UnitTests.Providers;

// Every (bin, item-dim, item-coord) width combination. There are only two widths now (Eight/Sixteen), so this
// is the 2x2x2 set, and it covers mixed widths too (e.g. small dimensions, large coordinates).
//
// The numeric type used to be a fourth column here, which forced the test to look the type up in a dictionary
// and call a generic helper through a delegate - which buried the assertion. The type is now picked by the
// test method instead (one per type), so each row is just widths and the test can arrange, act and assert in
// plain sight. Width is internal, so a public [Theory] cannot name it (CS0051): the widths ride as object and
// the test casts them back.
internal class WidthCombinationsProvider : IEnumerable<object[]>
{
	private static readonly Width[] widths = [Width.Eight, Width.Sixteen];

	public IEnumerator<object[]> GetEnumerator()
	{
		IEnumerable<object[]> rows = Combos(widths);
		return rows.GetEnumerator();
	}

	private static IEnumerable<object[]> Combos(Width[] sizes)
	{
		foreach (var binSize in sizes)
		foreach (var itemDimensionsSize in sizes)
		foreach (var itemCoordinatesSize in sizes)
		{
			yield return [binSize, itemDimensionsSize, itemCoordinatesSize];
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
