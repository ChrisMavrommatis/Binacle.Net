namespace Binacle.Net.Api.Configuration.Models;

public class BinPresetOptions
{
	public const string SectionName = "PresetOptions";
	public const string Path = "Presets.json";

	public Dictionary<string, BinPresetOption> Presets { get; set; }
}
