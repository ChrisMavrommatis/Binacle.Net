using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Binacle.Net.v2.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BinFitResultStatus
{
	[EnumMember(Value = nameof(AllItemsFit))]
	AllItemsFit,
	[EnumMember(Value = nameof(NotAllItemsFit))]
	NotAllItemsFit,
	[EnumMember(Value = nameof(EarlyFail_TotalVolumeExceeded))]
	EarlyFail_TotalVolumeExceeded,
	[EnumMember(Value = nameof(EarlyFail_ItemDimensionExceeded))]
	EarlyFail_ItemDimensionExceeded
}

