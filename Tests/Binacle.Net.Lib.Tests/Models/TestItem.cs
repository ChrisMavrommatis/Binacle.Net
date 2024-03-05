using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Tests.Models;

public sealed class TestItem : IItemWithDimensions<int>
{
	public TestItem()
	{

	}

	public TestItem(string id, IWithReadOnlyDimensions<int> item)
	{
		this.ID = id;
		this.Length = item.Length;
		this.Width = item.Width;
		this.Height = item.Height;
	}

	public string ID { get; set; }
	public int Quantity { get; set; }
	public int Length { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
}
