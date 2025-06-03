using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Binacle.Net.DiagnosticsModule.Services;

internal class SystemHealthCheck : IHealthCheck
{
	public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
		CancellationToken cancellationToken = default)
	{
		try
		{
			var data = new Dictionary<string, object>()
			{
				{"Processors", Environment.ProcessorCount},
			};
			return HealthCheckResult.Healthy("System Info", data);
		}
		catch (Exception ex)
		{
			return new HealthCheckResult(context.Registration.FailureStatus, "Health check for Azure Tables failed",
				ex);
		}
	}
}
