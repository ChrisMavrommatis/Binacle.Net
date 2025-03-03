using Serilog.Core;
using Serilog.Events;

namespace Binacle.Net.Api.DiagnosticsModule.Services;

internal class BinacleVersionEnricher : ILogEventEnricher
{
	public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
	{
		logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
			"Version", Environment.GetEnvironmentVariable("BINACLE_VERSION") ?? "Unknown"));
	}
}
