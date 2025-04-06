using Binacle.Net.Kernel.Configuration.Models;

namespace Binacle.Net.Configuration.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class BinPresetOptions : IConfigurationOptions
{
	public static string FilePath => "Presets.json";
	public static string SectionName => "PresetOptions";
	public static bool Optional => false;
	public static bool ReloadOnChange => true;
	public static string? GetEnvironmentFilePath(string environment) => null;


	public Dictionary<string, BinPresetOption> Presets { get; set; } = new();
}
