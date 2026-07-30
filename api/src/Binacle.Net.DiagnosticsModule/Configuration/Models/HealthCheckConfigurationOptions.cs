using Binacle.Net.Kernel.Configuration.Models;

namespace Binacle.Net.DiagnosticsModule.Configuration.Models;

internal class HealthCheckConfigurationOptions : IConfigurationOptions
{
	public static string FilePath => "DiagnosticsModule/HealthChecks.json";
	public static string SectionName => "HealthChecks";
	public static bool Optional => false;
	// False because nothing here can act on a reload. The middleware parses RestrictedIPs once in its constructor,
	// and Enabled and Path are read once when the pipeline is built, so an edit changed nothing while claiming it
	// would. Reload would also skip validation, which only runs at startup, and land a bad entry straight in the
	// middleware. Turning this back on means moving the middleware to IOptionsMonitor first.
	public static bool ReloadOnChange => false;
	public static string GetEnvironmentFilePath(string environment) => $"DiagnosticsModule/HealthChecks.{environment}.json";

	public bool Enabled { get; set; }
	public string? Path { get; set; }
	public string[]? RestrictedIPs { get; set; }
	public string[]? RestrictedChecks { get; set; }
}

