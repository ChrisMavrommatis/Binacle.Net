using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public abstract class ResponseBase<TModel>
{
	[JsonPropertyOrder(0)]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	[Description(SchemaDescriptions.Result)]
	public required ResultType Result { get; set; }

	[Description(SchemaDescriptions.Data)]
	public required TModel Data { get; set; }
}

[Description("Whether the operation succeeded or failed.")]
public enum ResultType
{
	Success,
	Failure
}
