using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.v2.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class ResultBox : IWithID
{
	public string ID { get; set; } = string.Empty;

	public Dimensions Dimensions { get; set; }
	public Coordinates? Coordinates { get; set; }
}
