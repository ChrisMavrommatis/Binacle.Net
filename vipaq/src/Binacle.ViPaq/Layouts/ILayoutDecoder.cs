using System.Numerics;
using Binacle.Geometry;

namespace Binacle.ViPaq.Layouts;

// Reads the item fields back out of one layout (PROTOCOL.md §3). Decoding fills items in place, so unlike
// ILayoutEncoder it needs the settable interfaces.
//
// One class implements both halves. The interfaces split only because the two directions want different things
// of TItem, not because the halves are independent.
internal interface ILayoutDecoder<T>
	where T : struct, IBinaryInteger<T>
{
	void Read<TItem>(ProtocolReader<T> protocolReader, TItem[] items, Header header)
		where TItem : IWithDimensions<T>, IWithCoordinates<T>;
}
