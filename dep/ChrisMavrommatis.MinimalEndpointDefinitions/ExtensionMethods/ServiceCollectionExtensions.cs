using Microsoft.Extensions.DependencyInjection;

namespace ChrisMavrommatis.MinimalEndpointDefinitions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMinimalEndpointDefinitions<T>(this IServiceCollection services)
	{
		var assemblyTypes = typeof(T).Assembly.GetTypes();

		// discover all IEndpointDefinition from T
		var endpointDefinitions = assemblyTypes
			.Where(t => typeof(IEndpointDefinition).IsAssignableFrom(t) && !t.IsInterface)
				.Where(t => t.GetInterfaces().Any(i => !i.GetGenericArguments().Any()))
			.Select(t => (IEndpointDefinition)Activator.CreateInstance(t))
			.ToList();

		services.AddSingleton(endpointDefinitions as IReadOnlyCollection<IEndpointDefinition>);



		var groupDefinitions = new List<Models.EndpointGroupDefinition>();

		var endpointGroupDefinitions = assemblyTypes
			.Where(t => typeof(IEndpointGroupDefinition).IsAssignableFrom(t) && !t.IsInterface)
			.Select(t => (IEndpointGroupDefinition)Activator.CreateInstance(t))
			.ToList();

		foreach (var endpointGroupDefinition in endpointGroupDefinitions)
		{
			var group = new Models.EndpointGroupDefinition();
			group.Definition = endpointGroupDefinition;

			var groupEndpoints = assemblyTypes
				.Where(t => typeof(IPartOfGroup).IsAssignableFrom(t) && !t.IsInterface)
				.Where(t => t.GetInterfaces().Any(i => i.GetGenericArguments().Any(g => g == endpointGroupDefinition.GetType())))
				.Select(t => (IPartOfGroup)Activator.CreateInstance(t))
				.ToList();

			group.EndpointDefinitions = groupEndpoints;

			groupDefinitions.Add(group);
		}
		services.AddSingleton(groupDefinitions as IReadOnlyCollection<Models.EndpointGroupDefinition>);

		return services;
	}
}
