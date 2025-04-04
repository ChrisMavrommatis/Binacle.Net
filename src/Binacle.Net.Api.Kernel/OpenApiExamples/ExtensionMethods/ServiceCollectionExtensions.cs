using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenApiExamples.Abstractions;
using OpenApiExamples.Services;

namespace OpenApiExamples;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddOpenApiExamples(
		this IServiceCollection services,
		Action<OpenApiExamplesOptions>? configureOptions = null,
		Action<JsonExamplesFormatterOptions>? configureJsonOptions = null
		)
	{

		configureJsonOptions ??= _ => { };
		services.Configure(configureJsonOptions);

		services
			.AddOptions<OpenApiExamplesOptions>()
			.Configure<IServiceProvider>((options, sp) =>
			{
				var logger = sp.GetService<ILoggerFactory>()?
					.CreateLogger("OpenApiExamples");
				
				var formatters = sp.GetServices<IOpenApiExamplesFormatter>();
				foreach (var formatter in formatters)
				{
					foreach (var contentType in formatter.SupportedContentTypes)
					{
						if(options.Formatters.ContainsKey(contentType))
						{
							logger?.LogWarning(
								"Formatter for content type {contentType} already exists. Overriding with {formatterName}.",
								contentType,
								formatter.GetType().Name
							);
						}

						options.Formatters[contentType] = formatter;

					}
					
				}
				configureOptions?.Invoke(options);
			});


		services.AddTransient<IOpenApiExamplesWriter, OpenApiExamplesWritter>();
		services.AddTransient<IOpenApiExamplesFormatter, JsonOpenApiExamplesFormatter>();
		services.AddTransient<IOpenApiExamplesFormatter, XmlOpenApiExamplesFormatter>();
		return services;
	}
	
	public static IServiceCollection AddExamplesFormatter<T>(
		this IServiceCollection services,
		string contentType)
		where T : class, IOpenApiExamplesFormatter
	{
		if (services.Any(s => s.ServiceType == typeof(T)))
		{
			throw new InvalidOperationException($"Formatter for content type '{contentType}' already exists.");
		}

		services.AddTransient<IOpenApiExamplesFormatter, T>();
		return services;
	}


}
