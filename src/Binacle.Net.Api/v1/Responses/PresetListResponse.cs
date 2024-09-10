namespace Binacle.Net.Api.v1.Responses;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class PresetListResponse : v1.Models.ResponseBase
{
	public Dictionary<string, List<v1.Models.Bin>> Presets { get; set; }

	public static PresetListResponse Create(Dictionary<string, List<v1.Models.Bin>> presets)
	{
		return new PresetListResponse
		{
			Presets = presets
		};
	}
}
