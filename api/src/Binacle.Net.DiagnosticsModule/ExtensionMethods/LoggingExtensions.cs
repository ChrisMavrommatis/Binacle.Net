using Serilog;
using Serilog.Configuration;

namespace Binacle.Net.DiagnosticsModule.ExtensionMethods;

public static class LoggingExtensions
{
	public static LoggerConfiguration WithBinacleVersion(
		this LoggerEnrichmentConfiguration enrich)
	{
		ArgumentNullException.ThrowIfNull(enrich);

		return enrich.With<Services.BinacleVersionEnricher>();
	}
}
