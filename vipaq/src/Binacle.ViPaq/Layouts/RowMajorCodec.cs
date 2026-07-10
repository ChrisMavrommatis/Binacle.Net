using System.Numerics;
using Binacle.Geometry;

namespace Binacle.ViPaq.Layouts;

// Each item whole, then the next item (PROTOCOL.md §3.1):
//
//   L W H X Y Z | L W H X Y Z | L W H X Y Z | ...
//
// Easier to read in a hex dump than columnar, and exactly the same length uncompressed.
internal sealed class RowMajorCodec<T> : ILayoutEncoder<T>, ILayoutDecoder<T>
	where T : struct, IBinaryInteger<T>
{
	public void Write<TItem>(ProtocolWriter<T> protocolWriter, IReadOnlyList<TItem> items, Header header)
		where TItem : IWithReadOnlyDimensions<T>, IWithReadOnlyCoordinates<T>
	{
		var dimensionsWidth = header.ItemDimensionsWidth;
		var coordinatesWidth = header.ItemCoordinatesWidth;

		foreach (var item in items)
		{
			protocolWriter.WriteValue(item.Length, dimensionsWidth);
			protocolWriter.WriteValue(item.Width, dimensionsWidth);
			protocolWriter.WriteValue(item.Height, dimensionsWidth);
			protocolWriter.WriteValue(item.X, coordinatesWidth);
			protocolWriter.WriteValue(item.Y, coordinatesWidth);
			protocolWriter.WriteValue(item.Z, coordinatesWidth);
		}
	}

	public void Read<TItem>(ProtocolReader<T> protocolReader, TItem[] items, Header header)
		where TItem : IWithDimensions<T>, IWithCoordinates<T>
	{
		var dimensionsWidth = header.ItemDimensionsWidth;
		var coordinatesWidth = header.ItemCoordinatesWidth;

		for (var index = 0; index < items.Length; index++)
		{
			items[index].Length = protocolReader.ReadValue(dimensionsWidth);
			items[index].Width = protocolReader.ReadValue(dimensionsWidth);
			items[index].Height = protocolReader.ReadValue(dimensionsWidth);
			items[index].X = protocolReader.ReadValue(coordinatesWidth);
			items[index].Y = protocolReader.ReadValue(coordinatesWidth);
			items[index].Z = protocolReader.ReadValue(coordinatesWidth);
		}
	}
}
