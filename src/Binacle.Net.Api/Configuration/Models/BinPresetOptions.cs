namespace Binacle.Net.Api.Configuration.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class BinPresetOptions
{
	public const string SectionName = "PresetOptions";
	public const string FilePath = "Presets.json";

	public Dictionary<string, BinPresetOption> Presets { get; set; } = new();
}
