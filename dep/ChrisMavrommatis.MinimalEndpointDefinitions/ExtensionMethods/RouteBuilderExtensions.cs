using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChrisMavrommatis.MinimalEndpointDefinitions;

public static class WebApplicationExtensions
{
	public static T UseMinimalEndpointDefinitions<T>(this T app)
		where T : IHost, IEndpointRouteBuilder
	{
		var endpointDefinitions = app.Services.GetService<IReadOnlyCollection<IEndpointDefinition>>();
		foreach (var endpointDefinition in endpointDefinitions)
		{
			endpointDefinition.DefineEndpoint(app);
		}


		var endpointGroupDefinitions = app.Services.GetService<IReadOnlyCollection<Models.EndpointGroupDefinition>>();

		foreach (var endpointGroupDefinition in endpointGroupDefinitions)
		{
			if (!endpointGroupDefinition.EndpointDefinitions.Any())
			{
				continue;
			}

			var groupRouteBuilder = endpointGroupDefinition.Definition.DefineEndpointGroup(app);
			foreach (var endpointDefinition in endpointGroupDefinition.EndpointDefinitions)
			{
				endpointDefinition.DefineEndpoint(groupRouteBuilder);
			}
		}

		return app;
	}
}
