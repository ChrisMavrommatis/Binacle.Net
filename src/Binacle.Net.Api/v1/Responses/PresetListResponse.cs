using Binacle.Net.Api.Models;
using Binacle.Net.Api.v1.Models;

namespace Binacle.Net.Api.v1.Responses;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class PresetListResponse : ResponseBase
{
	public Dictionary<string, List<Bin>> Presets { get; set; }

	public static PresetListResponse Create(Dictionary<string, List<Bin>> presets)
	{
		return new PresetListResponse
		{
			Presets = presets
		};

	}
}
