using System.Numerics;
using Binacle.Geometry;

namespace Binacle.ViPaq.Layouts;

// Each field for every item before the next field — six runs, each `count` values long (PROTOCOL.md §3.2):
//
//   L L L ... | W W W ... | H H H ... | X X X ... | Y Y Y ... | Z Z Z ...
//
// Like magnitudes sit next to each other, which usually compresses better. Uncompressed it is exactly as long
// as row-major. Whether it pays is unmeasured.
internal sealed class ColumnarCodec<T> : ILayoutEncoder<T>, ILayoutDecoder<T>
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
		}

		foreach (var item in items)
		{
			protocolWriter.WriteValue(item.Width, dimensionsWidth);
		}

		foreach (var item in items)
		{
			protocolWriter.WriteValue(item.Height, dimensionsWidth);
		}

		foreach (var item in items)
		{
			protocolWriter.WriteValue(item.X, coordinatesWidth);
		}

		foreach (var item in items)
		{
			protocolWriter.WriteValue(item.Y, coordinatesWidth);
		}

		foreach (var item in items)
		{
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
		}

		for (var index = 0; index < items.Length; index++)
		{
			items[index].Width = protocolReader.ReadValue(dimensionsWidth);
		}

		for (var index = 0; index < items.Length; index++)
		{
			items[index].Height = protocolReader.ReadValue(dimensionsWidth);
		}

		for (var index = 0; index < items.Length; index++)
		{
			items[index].X = protocolReader.ReadValue(coordinatesWidth);
		}

		for (var index = 0; index < items.Length; index++)
		{
			items[index].Y = protocolReader.ReadValue(coordinatesWidth);
		}

		for (var index = 0; index < items.Length; index++)
		{
			items[index].Z = protocolReader.ReadValue(coordinatesWidth);
		}
	}
}
