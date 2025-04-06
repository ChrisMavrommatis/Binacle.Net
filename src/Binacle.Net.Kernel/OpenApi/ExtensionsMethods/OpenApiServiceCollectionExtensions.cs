using Binacle.Net.Kernel.Endpoints;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.Kernel.OpenApi.ExtensionsMethods;

public static class OpenApiServiceCollectionExtensions
{
	public static IServiceCollection AddOpenApiDocumentsFromAssemblyContaining<T>(this IServiceCollection services)
	{
		var assemblyTypes = typeof(T).Assembly.GetTypes();

		var openApiDocuments = assemblyTypes
			.Where(t => typeof(IOpenApiDocument).IsAssignableFrom(t) && !t.IsInterface)
			.Where(t => t.GetInterfaces().Any(i => !i.GetGenericArguments().Any()))
			.Select(t => (IOpenApiDocument)Activator.CreateInstance(t)!)
			.ToList();
		
		// Register the endpoint definitions
		foreach(var openApiDocument in openApiDocuments)
		{
			services.AddSingleton(openApiDocument);
			openApiDocument.Add(services);
		}

		return services;
	}
}
