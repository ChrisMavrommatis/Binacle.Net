using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Binacle.Net.DiagnosticsModule.Services;

internal class SystemHealthCheck : IHealthCheck
{
	private readonly IHostEnvironment hostEnvironment;
	private readonly IOptions<FeatureOptions> featureOptions;
	private readonly TimeProvider timeProvider;

	public SystemHealthCheck(
		IHostEnvironment hostEnvironment,
		IOptions<FeatureOptions> featureOptions,
		TimeProvider timeProvider
	)
	{
		this.hostEnvironment = hostEnvironment;
		this.featureOptions = featureOptions;
		this.timeProvider = timeProvider;
	}

	public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
		CancellationToken cancellationToken = default)
	{
		try
		{
			// Version and uptime answer the two questions you cannot answer from outside the container:
			// whether the running image is the one you deployed, and whether a config change took effect.
			var startedAt = new DateTimeOffset(Process.GetCurrentProcess().StartTime.ToUniversalTime());

			var data = new Dictionary<string, object>()
			{
				{"Version", Metadata.Version},
				{"Environment", this.hostEnvironment.EnvironmentName},
				{"StartedAt", startedAt.ToString("O")},
				{"Uptime", (this.timeProvider.GetUtcNow() - startedAt).ToString(@"d\.hh\:mm\:ss")},
				{"Processors", Environment.ProcessorCount},
				{"Features", this.featureOptions.Value.EnabledFeatures.Order().ToArray()},
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
