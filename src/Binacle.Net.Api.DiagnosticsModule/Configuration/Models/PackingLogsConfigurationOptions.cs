using Binacle.Net.Api.Kernel.Models;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class PackingLogsConfigurationOptions : IConfigurationOptions
{
	public static string FilePath => "DiagnosticsModule/PackingLogs.json";
	public static string SectionName => "PackingLogs";
	public static bool Optional => false;
	public static bool ReloadOnChange => true;
	public static string GetEnvironmentFilePath(string environment) => $"DiagnosticsModule/PackingLogs.{environment}.json";
	
	public bool Enabled { get; set; }
	public PackingLogOptions? LegacyPacking { get; set; }
	public PackingLogOptions? LegacyFitting { get; set; }
	public PackingLogOptions? Packing { get; set; }

}

internal class PackingLogOptions
{
	public string? Path { get; set; }
	public string? FileName { get; set; }
	public string? DateFormat { get; set; }
	public int? ChannelLimit { get; set; }
}
