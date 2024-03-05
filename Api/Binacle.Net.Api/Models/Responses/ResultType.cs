using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Binacle.Net.Api.Models.Responses;

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
