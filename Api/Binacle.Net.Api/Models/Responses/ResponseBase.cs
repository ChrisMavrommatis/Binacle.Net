using System.Text.Json.Serialization;

namespace Binacle.Net.Api.Models.Responses;

public abstract class ResponseBase
{
	[JsonPropertyOrder(0)]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public ResultType Result { get; set; }

	public string Message { get; set; }
}
