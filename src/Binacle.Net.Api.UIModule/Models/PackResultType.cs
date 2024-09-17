using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Binacle.Net.Api.UIModule.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
internal enum PackResultType
{
	[EnumMember(Value = nameof(Unknown))]
	Unknown,
	[EnumMember(Value = nameof(NotPacked))]
	NotPacked,
	[EnumMember(Value = nameof(PartiallyPacked))]
	PartiallyPacked,
	[EnumMember(Value = nameof(FullyPacked))]
	FullyPacked,
	[EnumMember(Value = nameof(EarlyFail_ContainerVolumeExceeded))]
	EarlyFail_ContainerVolumeExceeded,
	[EnumMember(Value = nameof(EarlyFail_ContainerDimensionExceeded))]
	EarlyFail_ContainerDimensionExceeded,
}
