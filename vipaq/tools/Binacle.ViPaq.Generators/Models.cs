using Binacle.ViPaq.Abstractions;

namespace Binacle.ViPaq.Generators;

// The tool always works in long — long holds the whole interoperable range [0, 2^53-1] exactly — so
// Bin/Item are concrete long types, no generic T. The tool stays standalone: it references only the
// ViPaq library, not the test project. If we ever need to couple them we can revisit.
public class Bin : IWithDimensions<long>
{
	public long Length { get; set; }
	public long Width { get; set; }
	public long Height { get; set; }
}

public class Item : IWithDimensions<long>, IWithCoordinates<long>
{
	public long Length { get; set; }
	public long Width { get; set; }
	public long Height { get; set; }
	public long X { get; set; }
	public long Y { get; set; }
	public long Z { get; set; }
}
