using System.Numerics;
using Binacle.Geometry;

namespace Binacle.ViPaq.Layouts;

// Writes the item fields in one layout (PROTOCOL.md §3). Encoding only reads an item, so the read-only
// interfaces are enough - that is what lets a caller encode a type it cannot mutate.
//
// Only the items are laid out; the item count and bin dimensions are the same in every layout.
//
// TItem is a method type parameter, not a class one, so the write and read halves can demand different things
// of it while one class implements both.
internal interface ILayoutEncoder<T>
	where T : struct, IBinaryInteger<T>
{
	void Write<TItem>(ProtocolWriter<T> protocolWriter, IReadOnlyList<TItem> items, Header header)
		where TItem : IWithReadOnlyDimensions<T>, IWithReadOnlyCoordinates<T>;
}
