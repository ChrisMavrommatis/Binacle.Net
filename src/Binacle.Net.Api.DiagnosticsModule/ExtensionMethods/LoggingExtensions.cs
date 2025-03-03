using Serilog;
using Serilog.Configuration;

namespace Binacle.Net.Api.DiagnosticsModule.ExtensionMethods;

public static class LoggingExtensions
{
	public static LoggerConfiguration WithBinacleVersion(
		this LoggerEnrichmentConfiguration enrich)
	{
		if (enrich == null)
			throw new ArgumentNullException(nameof(enrich));

		return enrich.With<Services.BinacleVersionEnricher>();
	}
}
