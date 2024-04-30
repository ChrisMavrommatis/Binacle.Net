using ChrisMavrommatis.SwaggerExamples.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChrisMavrommatis.SwaggerExamples;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSwaggerExamples(this IServiceCollection services)
		=> AddSwaggerExamples(services, null);

	public static IServiceCollection AddSwaggerExamples(this IServiceCollection services, Action<ExamplesFormatterOptions>? setupAction = null)
	{
		if(setupAction is not null)
		{
			services.Configure(setupAction);

		}
		services.AddSingleton<ExamplesFormatter>();
		services.AddSingleton<ExamplesWriter>();
		services.AddSingleton<SwaggerExamplesOperationFilter>();
		return services;
	}
}
