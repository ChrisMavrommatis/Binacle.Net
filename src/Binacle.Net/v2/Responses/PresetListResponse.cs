using Binacle.Net.v2.Models;

namespace Binacle.Net.v2.Responses;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class PresetListResponse : ResponseBase<Dictionary<string, List<v2.Models.Bin>>>
{
	public static PresetListResponse Create(Dictionary<string, List<v2.Models.Bin>> presets)
	{
		return new PresetListResponse
		{
			Data = presets,
			Result = Models.ResultType.Success
		};
	}
}
