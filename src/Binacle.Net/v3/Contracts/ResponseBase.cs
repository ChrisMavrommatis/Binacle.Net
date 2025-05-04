using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public abstract class ResponseBase<TModel>
{
	[JsonPropertyOrder(0)]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required ResultType Result { get; set; }

	public required TModel Data { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResultType
{
	[EnumMember(Value = nameof(Success))]
	Success,
	[EnumMember(Value = nameof(Failure))]
	Failure
}
