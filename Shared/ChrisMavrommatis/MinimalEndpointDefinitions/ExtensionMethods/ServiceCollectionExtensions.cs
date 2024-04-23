using Microsoft.Extensions.DependencyInjection;

namespace ChrisMavrommatis.MinimalEndpointDefinitions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMinimalEndpointDefinitions<T>(this IServiceCollection services)
	{
		// discover all IEndpointDefinition from T
		var endpointDefinitions = typeof(T).Assembly.GetTypes()
			.Where(t => typeof(IEndpointDefinition).IsAssignableFrom(t) && !t.IsInterface)
			.Select(t => (IEndpointDefinition)Activator.CreateInstance(t))
			.ToList();

		services.AddSingleton(endpointDefinitions as IReadOnlyCollection<IEndpointDefinition>);
		return services;
	}
}
