using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenApiExamples.Abstractions;

namespace OpenApiExamples.Services;

internal class OpenApiExamplesWritter : IOpenApiExamplesWriter
{
	private readonly IServiceProvider serviceProvider;
	private readonly IOptions<OpenApiExamplesOptions> options;
	private readonly ILogger<OpenApiExamplesWritter> logger;

	public OpenApiExamplesWritter(
		IServiceProvider serviceProvider,
		IOptions<OpenApiExamplesOptions> options,
		ILogger<OpenApiExamplesWritter> logger
	)
	{
		this.serviceProvider = serviceProvider;
		this.options = options;
		this.logger = logger;
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

		content.Example = await formatter.FormatAsync(example);
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
			var formattedExample = await formatter.FormatAsync(example);
			content.Examples.Add(example.Key, new OpenApiExample()
			{
				Summary = example.Summary,
				Description = example.Description,
				Value = formattedExample,
			});
		}
	}
}
