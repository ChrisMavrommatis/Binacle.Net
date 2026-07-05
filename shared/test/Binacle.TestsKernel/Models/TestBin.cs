using Binacle.Lib.Abstractions.Models;

namespace Binacle.TestsKernel.Models;

public sealed class TestBin : IWithID, IWithDimensions
{
	public TestBin()
	{
		this.ID = string.Empty;
	}

	public TestBin(string id, IWithReadOnlyDimensions item)
	{
		this.ID = id;
		this.Length = item.Length;
		this.Width = item.Width;
		this.Height = item.Height;
	}

	public TestBin(string id, Binacle.CompactNotation.IWithDimensions<int> dimensions)
	{
		this.ID = id;
		this.Length = dimensions.Length;
		this.Width = dimensions.Width;
		this.Height = dimensions.Height;
	}

	// "LxWxH" — a bin carries no quantity. Parsed via the shared Binacle.CompactNotation notation.
	public static TestBin FromCompactString(string compact)
		=> new(compact, Binacle.CompactNotation.CompactNotationParser.ParseDimensions<int>(compact));

	public string ID { get; set; }
	public int Length { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
}
