using Binacle.Net.Kernel.Configuration.Models;

namespace Binacle.Net.DiagnosticsModule.Configuration.Models;

internal class PackingLogsConfigurationOptions : IConfigurationOptions
{
	public static string FilePath => "DiagnosticsModule/PackingLogs.json";
	public static string SectionName => "PackingLogs";
	public static bool Optional => false;
	public static bool ReloadOnChange => true;
	public static string GetEnvironmentFilePath(string environment) => $"DiagnosticsModule/PackingLogs.{environment}.json";

	public bool Enabled { get; set; }
	public string? Path { get; set; }
	public string? FileName { get; set; }
	public string? DateFormat { get; set; }
	public int? ChannelLimit { get; set; }
	// Null (default) keeps every file. Deletion is left to an external prune.
	// Set it (e.g. 7) to auto-delete local files older than that many days.
	public int? RetentionDays { get; set; }
}
