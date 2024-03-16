namespace Binacle.Net.Api.Configuration.Models;

public class BinPresetOptions
{
	public const string SectionName = "PresetOptions";
	public const string FilePath = "Presets.json";

	public Dictionary<string, BinPresetOption> Presets { get; set; }
}
