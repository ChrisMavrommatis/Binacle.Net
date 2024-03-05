using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.Models;

public class Bin : IItemWithDimensions<int>
{
	public string ID { get; set; }
	public int Length { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
}
