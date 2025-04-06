using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenApiExamples.Abstractions;

namespace OpenApiExamples.Services;

internal class OpenApiExamplesWriter : IOpenApiExamplesWriter
{
	private readonly IServiceProvider serviceProvider;
	private readonly IOptions<OpenApiExamplesOptions> options;
	private readonly ILogger<OpenApiExamplesWriter> logger;

	public OpenApiExamplesWriter(
		IServiceProvider serviceProvider,
		IOptions<OpenApiExamplesOptions> options,
		ILogger<OpenApiExamplesWriter> logger
	)
	{
		this.serviceProvider = serviceProvider;
		this.options = options;
		this.logger = logger;
	}

	public async ValueTask WriteAsync(
		OpenApiMediaType content,
		string itemContentType,
		Type itemProviderType
	)
	{
		var providerInstance = ActivatorUtilities.CreateInstance(
			this.serviceProvider,
			itemProviderType
		);

		if (providerInstance is ISingleOpenApiExamplesProvider singleExamplesProvider)
		{
			var example = singleExamplesProvider.GetExample();
			await this.WriteSingleExampleAsync(content, itemContentType, example);
		}
		else if (providerInstance is IMultipleOpenApiExamplesProvider multipleExamplesProvider)
		{
			var examples = multipleExamplesProvider.GetExamples();
			await this.WriteMultipleExamplesAsync(content, itemContentType, examples);
		}
		else
		{
			this.logger?.LogWarning(
				"Provider type {ProviderType} should be either ISingleOpenApiExamplesProvider or IMultipleOpenApiExamplesProvider",
				providerInstance.GetType().Name
			);
		}
	}

	public async ValueTask WriteSingleExampleAsync(
		OpenApiMediaType content,
		string contentType,
		IOpenApiExample example
	)
	{
		if (!this.options.Value.Formatters.TryGetValue(contentType, out var formatter))
		{
			this.logger.LogWarning(
				"Formatter for content type {contentType} not found. Skipping example.",
				contentType
			);
			return;
		}

		var formattedExample = await formatter.FormatAsync(example.Value);
		content.Example = formattedExample;
	}

	public async ValueTask WriteMultipleExamplesAsync(
		OpenApiMediaType content,
		string contentType,
		IEnumerable<IOpenApiExample> examples
	)
	{
		if (!this.options.Value.Formatters.TryGetValue(contentType, out var formatter))
		{
			this.logger.LogWarning(
				"Formatter for content type {contentType} not found. Skipping example.",
				contentType
			);
			return;
		}

		foreach (var example in examples)
		{
			var formattedExample = await formatter.FormatAsync(example.Value);
			content.Examples.Add(example.Key, new Microsoft.OpenApi.Models.OpenApiExample()
			{
				Summary = example.Summary,
				Description = example.Description,
				Value = formattedExample,
			});
		}
	}
}
