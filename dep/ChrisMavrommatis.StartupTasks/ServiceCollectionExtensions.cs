using Microsoft.Extensions.DependencyInjection;

namespace ChrisMavrommatis.StartupTasks;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddStartupTask<TStartupTask>(this IServiceCollection services)
		where TStartupTask : class, IStartupTask
	{
		services.AddTransient<IStartupTask, TStartupTask>();
		return services;

	}
}
