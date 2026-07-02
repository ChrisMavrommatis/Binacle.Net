namespace Binacle.ViPaq.UnitTests.Providers;

// Which picker a bit-size row targets. Shared by BitSizeInvalidProvider and BitSizeSelectionProvider: it
// splits each file's rows into the dimensions set and the coordinates set, and says how a row's Values is
// written so the loader parses it right (Dimensions -> "LxWxH", Coordinates -> "X,Y,Z"). Bound from JSON.
internal enum BitSizeKind
{
	Dimensions,
	Coordinates,
}
