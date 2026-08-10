namespace Binacle.CompactNotation;

// Which block a compact string is, as decided by CompactNotationParser.Detect.
public enum CompactNotationKind
{
	Dimensions,
	Coordinates,
	Quantity,
}
