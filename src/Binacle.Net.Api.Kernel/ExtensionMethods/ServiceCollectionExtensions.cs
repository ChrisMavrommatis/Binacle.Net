using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace Binacle.Net.Api.Kernel;

public static class ServiceCollectionExtensions
{
	public static void AddHealthCheck<T>


	public static void AddHealthCheck(
		IServiceCollection services,
		string name,
		Func<IServiceProvider, IHealthCheck> factory,
		HealthStatus failureStatus,
		string[]? tags,
		TimeSpan? timeout = null
	)
	{
		services.Configure<HealthCheckServiceOptions>(options =>
		{
			options.Registrations.Add(new HealthCheckRegistration(
				name,
				factory,
				failureStatus,
				tags,
				timeout
			));
		});

	}
}
