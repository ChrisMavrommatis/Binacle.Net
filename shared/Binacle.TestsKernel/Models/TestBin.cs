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

	public string ID { get; set; }
	public int Length { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
}
