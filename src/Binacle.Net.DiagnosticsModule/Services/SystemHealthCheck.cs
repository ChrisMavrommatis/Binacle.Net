using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Binacle.Net.DiagnosticsModule.Services;

internal class SystemHealthCheck : IHealthCheck
{
	public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
		CancellationToken cancellationToken = default)
	{
		try
		{
			var data = new Dictionary<string, object>()
			{
				{"Processors", Environment.ProcessorCount},
			};
			return Task.FromResult(HealthCheckResult.Healthy("System Info", data));
		}
		catch (Exception ex)
		{
			return Task.FromResult(
				new HealthCheckResult(
					context.Registration.FailureStatus, 
					"System health check failed",
					ex
				)
			);
		}
	}
}
