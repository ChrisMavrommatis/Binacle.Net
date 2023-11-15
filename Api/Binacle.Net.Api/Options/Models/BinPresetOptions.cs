namespace Binacle.Net.Api.Options.Models
{
    public class BinPresetOptions
    {
        public const string SectionName = "PresetOptions";
        public const string Path = "Presets.json";


        public Dictionary<string, BinPresetOption> Presets { get; set; }
    }
}
