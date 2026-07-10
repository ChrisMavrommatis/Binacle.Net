using System.Numerics;
using Binacle.Geometry;

namespace Binacle.ViPaq.Layouts;

// Writes the item fields in one layout (PROTOCOL.md §3). Encoding only ever reads an item, so it asks for the
// read-only interfaces — that is what lets a caller encode a type it cannot mutate (a packing result, say).
//
// Only the items are laid out. The item count and the bin dimensions are the same in every layout, so
// ProtocolEncoder handles those and hands over once it reaches the items.
//
// TItem is a method type parameter, not a class one, so the write and read halves can demand different things
// of it while a single class still implements both. See ILayoutDecoder.
internal interface ILayoutEncoder<T>
	where T : struct, IBinaryInteger<T>
{
	void Write<TItem>(ProtocolWriter<T> protocolWriter, IReadOnlyList<TItem> items, Header header)
		where TItem : IWithReadOnlyDimensions<T>, IWithReadOnlyCoordinates<T>;
}
