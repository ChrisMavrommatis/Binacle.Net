namespace Binacle.Net.Api.Models.Responses;

public class PresetListResponse: ResponseBase
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
