using Binacle.Net.Configuration;

namespace Binacle.Net.v4.Contracts.Presets;

#pragma warning disable CS1591

public class PresetListResponse
{
	public required IDictionary<string, List<Bin>> Presets { get; init; }

	public static PresetListResponse From(IDictionary<string, List<Bin>> presets)
	{
		return new PresetListResponse
		{
			Presets = presets
		};
	}
	
	public static PresetListResponse From(IDictionary<string, BinPresetOption> presetOptions)
	{
		var presets = presetOptions
			.ToDictionary(
				x => x.Key,
				x => x.Value.Bins
					.Select(bin => new Bin()
					{
						ID = bin.ID,
						Length = bin.Length,
						Height = bin.Height,
						Width = bin.Width
					}).ToList()
			);
		return From(presets);
	}
}
