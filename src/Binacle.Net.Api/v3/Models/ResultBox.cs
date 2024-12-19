using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.v3.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class ResultBox : IWithID
{
	public required string ID { get; set; }

	public int? Quantity { get; set; }
	public Dimensions? Dimensions { get; set; }
	public Coordinates? Coordinates { get; set; }
}
