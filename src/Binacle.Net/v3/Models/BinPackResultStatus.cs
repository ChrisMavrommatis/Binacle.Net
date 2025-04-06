using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Binacle.Net.v3.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BinPackResultStatus
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

