namespace Binacle.ViPaq;

// How the items are arranged in the body (PROTOCOL.md §3). Neither is normative: the header bit says which one
// the encoder used. Public because a caller picks it through `ViPaqSerializationOptions`; the rest of the
// header stays internal.
public enum Layout
{
	// Each item whole: L W H X Y Z, then the next item.
	RowMajor = 0,

	// Each field for every item before the next field: six runs, each `count` values long.
	Columnar = 1
}
