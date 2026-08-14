using Binacle.Net.Kernel.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net;

public static class EndpointsWebApplicationExtensions
{
	public static WebApplication RegisterEndpointsFromAssemblyContaining<T>(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();
		var serviceProvider = scope.ServiceProvider;

		var assemblyTypes = typeof(T).Assembly.GetTypes();
		app
			.RegisterEndpoints(assemblyTypes, serviceProvider)
			.RegisterEndpointsWithGroups(assemblyTypes, serviceProvider);
		return app;
	}

	private static WebApplication RegisterEndpoints(
		this WebApplication app,
		Type[] assemblyTypes,
		IServiceProvider serviceProvider
	)
	{
		var endpointTypes = assemblyTypes
			.Where(t => typeof(IEndpoint).IsAssignableFrom(t))
			.Where(t => t is { IsClass: true, IsAbstract: false, IsInterface: false })
			.ToList();

		foreach (var endpointType in endpointTypes)
		{
			var endpointInstance =(IEndpoint)ActivatorUtilities.CreateInstance(
				serviceProvider,
				endpointType
			);
			endpointInstance.DefineEndpoint(app);
		}
		return app;
	}
	
	private static WebApplication RegisterEndpointsWithGroups(
		this WebApplication app,
		Type[] assemblyTypes,
		IServiceProvider serviceProvider
	)
	{
		var endpointGroupTypes = assemblyTypes
			.Where(t => typeof(IEndpointGroup).IsAssignableFrom(t))
			.Where(t => t is {IsClass: true, IsAbstract:false, IsInterface: false})
			.ToList();

		var groups = new Dictionary<Type, RouteGroupBuilder>();

		// None registered means no module is here to contribute one, and no endpoint gets anything it did not
		// ask for itself.
		var conventions = serviceProvider.GetServices<IEndpointConvention>().ToList();

		foreach (var groupType in endpointGroupTypes)
		{
			var groupInstance = (IEndpointGroup)ActivatorUtilities.CreateInstance(serviceProvider, groupType);
			var routeGroup = groupInstance.DefineEndpointGroup(app);

			if (conventions.Count > 0)
			{
				// Finally, never Add: an Add convention on the group runs before each endpoint's own, so
				// anything reading metadata reads it half written. Owned here so no module has to know.
				((IEndpointConventionBuilder)routeGroup).Finally(endpointBuilder =>
				{
					foreach (var convention in conventions)
					{
						convention.Apply(endpointBuilder);
					}
				});
			}

			groups[groupType] = routeGroup;
		}
		
		var endpointTypes = assemblyTypes
			.Where(t => typeof(IGroupedEndpoint).IsAssignableFrom(t))
			.Where(t => t is { IsClass: true, IsAbstract:false, IsInterface: false })
			.ToList();

		foreach (var endpointType in endpointTypes)
		{
			var endpointInstance = (IGroupedEndpoint)ActivatorUtilities.CreateInstance(serviceProvider, endpointType);
			var groupType = endpointType.GetInterfaces()
				.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IGroupedEndpoint<>))
				?.GetGenericArguments()
				.FirstOrDefault();

			if (groupType is not null && groups.TryGetValue(groupType, out var routeGroup))
			{
				endpointInstance.DefineEndpoint(routeGroup);
			}
		}
		return app;
	}
}
