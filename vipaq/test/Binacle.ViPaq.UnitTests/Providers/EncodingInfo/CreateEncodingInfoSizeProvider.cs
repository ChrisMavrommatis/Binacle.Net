using System.Collections;

namespace Binacle.ViPaq.UnitTests.Providers;

// Every combination of the three section sizes: bin, item dimensions, item coordinates.
// 4 sizes each -> 64 rows. Used to check CreateEncodingInfo sets each field from its own input.
internal class CreateEncodingInfoSizeProvider : IEnumerable<object[]>
{
	private static readonly BitSize[] sizes =
		[BitSize.Eight, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.SixtyFour];

	public IEnumerator<object[]> GetEnumerator()
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
