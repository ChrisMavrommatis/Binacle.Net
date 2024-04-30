using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.Configuration.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class BinOption : IItemWithDimensions<int>
{
	public string ID { get; set; } = string.Empty;
	public int Length { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
}
