using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ChrisMavrommatis.MinimalEndpointDefinitions;

public static class WebApplicationExtensions
{
	public static WebApplication UseMinimalEndpointDefinitions(this WebApplication app)
	{
		var endpointDefinitions = app.Services.GetRequiredService<IReadOnlyCollection<IEndpointDefinition>>();
		foreach (var endpointDefinition in endpointDefinitions)
		{
			endpointDefinition.DefineEndpoints(app);
		}

		return app;
	}
}
