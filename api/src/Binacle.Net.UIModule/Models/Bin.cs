using Binacle.Lib.Abstractions.Models;

namespace Binacle.Net.UIModule.Models;

// [Migrate-Review] drop the explicit Binacle.Geometry.IWithDimensions<int> below once lib's IWithDimensions
// shim reaches the mutable leaf generic (see .agents/plans/shared-geometry-extraction.md).
public class Bin :
	IWithID,
	IWithDimensions,
	Binacle.Geometry.IWithDimensions<int>
{
	public Bin()
	{
		this.ID = string.Empty;
	}

	public Bin(string id, IWithReadOnlyDimensions item)
		: this(id, item.Length, item.Width, item.Height)
	{

	}

	public Bin(string id, int length, int width, int height)
	{
		this.ID = id;
		this.Length = length;
		this.Width = width;
		this.Height = height;
	}

	public string ID { get; set; }
	public int Length { get; set; }

	public int Width { get; set; }

	public int Height { get; set; }
}
