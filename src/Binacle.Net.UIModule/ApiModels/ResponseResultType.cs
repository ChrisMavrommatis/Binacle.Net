using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Binacle.Net.UIModule.ApiModels;

[JsonConverter(typeof(JsonStringEnumConverter))]
internal enum ResponseResultType
{
	[EnumMember(Value = nameof(Success))]
	Success,
	[EnumMember(Value = nameof(Failure))]
	Failure,
	[EnumMember(Value = nameof(Error))]
	Error
}
