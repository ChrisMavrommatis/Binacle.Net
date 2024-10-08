using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace Binacle.Net.Api.Kernel;

public static class ServiceCollectionExtensions
{
	public static void AddHealthCheck<T>(
		this IServiceCollection services,
		string name,
		HealthStatus? failureStatus,
		string[]? tags,
		TimeSpan? timeout = null
	)
		where T : IHealthCheck
	{
		services.Configure<HealthCheckServiceOptions>(options =>
		{
			options.Registrations.Add(new HealthCheckRegistration(
				name,
				sp => ActivatorUtilities.CreateInstance<T>(sp),
				failureStatus,
				tags,
				timeout
			));
		});
	}

	public static void AddHealthCheck(
		this IServiceCollection services,
		string name,
		Func<IServiceProvider, IHealthCheck> factory,
		HealthStatus? failureStatus,
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
