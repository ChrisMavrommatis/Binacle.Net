using Binacle.Net.Lib.Abstractions.Models;
using Xunit.Abstractions;

namespace Binacle.Net.TestsKernel.Models;

public sealed class TestItem : IWithID, IWithDimensions, IWithQuantity
{
	public TestItem()
	{

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

	public string ID { get; set; }
	public int Quantity { get; set; }
	public int Length { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
}
