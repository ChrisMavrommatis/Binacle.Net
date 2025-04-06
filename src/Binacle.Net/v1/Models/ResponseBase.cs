using System.Text.Json.Serialization;

namespace Binacle.Net.v1.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public abstract class ResponseBase
{
	[JsonPropertyOrder(0)]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public ResultType Result { get; set; }

	public string? Message { get; set; }
}
