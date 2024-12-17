using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Binacle.Net.Api.v3.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResultType
{
	[EnumMember(Value = nameof(Success))]
	Success,
	[EnumMember(Value = nameof(Failure))]
	Failure,
	[EnumMember(Value = nameof(Error))]
	Error
}
