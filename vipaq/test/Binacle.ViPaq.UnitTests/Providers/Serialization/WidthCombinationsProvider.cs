using System.Collections;

namespace Binacle.ViPaq.UnitTests.Providers;

// Every (bin, item-dim, item-coord) width combination: the 2x2x2 set, mixed widths included.
//
// The numeric type is picked by the test method, one per type, not carried as a fourth column - a type column
// forces a dictionary lookup and a delegate call, which buries the assertion. Width is internal, so a public
// [Theory] cannot name it (CS0051): the widths ride as object and the test casts them back.
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
