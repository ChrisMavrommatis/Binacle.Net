using System.Collections;

namespace Binacle.ViPaq.UnitTests.Providers;

// Every combination of the three section widths: bin, item dimensions, item coordinates.
// Two widths each (Eight/Sixteen) -> 8 rows. Used to check Header.Create sets each field from its own input.
internal class HeaderWidthComboProvider : IEnumerable<object[]>
{
	private static readonly Width[] widths = [Width.Eight, Width.Sixteen];

	public IEnumerator<object[]> GetEnumerator()
	{
		foreach (var binWidth in widths)
		foreach (var itemDimensionsWidth in widths)
		foreach (var itemCoordinatesWidth in widths)
		{
			yield return [binWidth, itemDimensionsWidth, itemCoordinatesWidth];
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
