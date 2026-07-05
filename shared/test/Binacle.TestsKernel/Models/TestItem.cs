using Binacle.Lib.Abstractions.Models;

namespace Binacle.TestsKernel.Models;

// [Migrate-Review] geometry migration — review the ctor overloads (IWithReadOnlyDimensions vs
// Binacle.Geometry.IWithDimensions<int>) for redundancy (see .agents/plans/shared-geometry-extraction.md).
public sealed class TestItem : IWithID, IWithDimensions, IWithQuantity
{
	public TestItem()
	{
		this.ID = string.Empty;
	}

	public TestItem(string id, IWithReadOnlyDimensions item)
		: this(id, item, 1)
	{
	}

	public TestItem(string id, IWithReadOnlyDimensions item, int quantity)
	{
		this.ID = id;
		this.Length = item.Length;
		this.Width = item.Width;
		this.Height = item.Height;
		this.Quantity = quantity;
	}

	public TestItem(string id, Binacle.Geometry.IWithDimensions<int> dimensions, int quantity)
	{
		this.ID = id;
		this.Length = dimensions.Length;
		this.Width = dimensions.Width;
		this.Height = dimensions.Height;
		this.Quantity = quantity;
	}

	// "LxWxH" or "LxWxH [Q]" (quantity default 1), parsed via the shared Binacle.CompactNotation notation.
	public static TestItem FromCompactString(string compact)
	{
		var parsed = Binacle.CompactNotation.CompactNotationParser.ParseDimensionsAndQuantity<int>(compact);
		return new TestItem(compact, parsed, parsed.Quantity);
	}

	public string ID { get; set; }
	public int Quantity { get; set; }
	public int Length { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
}
