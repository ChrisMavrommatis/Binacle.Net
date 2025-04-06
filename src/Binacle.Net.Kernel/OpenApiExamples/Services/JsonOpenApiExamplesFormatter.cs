using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using OpenApiExamples.Abstractions;

namespace OpenApiExamples.Services;

internal class JsonOpenApiExamplesFormatter : IOpenApiExamplesFormatter
{
	private readonly IOptions<OpenApiExamplesOptions> options;

	public JsonOpenApiExamplesFormatter(
		IOptions<OpenApiExamplesOptions> options
	)
	{
		this.options = options;
	}
	public IEnumerable<string> SupportedContentTypes =>
	[
		"application/json",
	];
	
	public ValueTask<IOpenApiAny> FormatAsync(object example)
	{
		var serializedExample = JsonSerializer.Serialize(example, this.options.Value.JsonSerializerOptions);
		var openApiExample = new OpenApiString(serializedExample);
		return ValueTask.FromResult<IOpenApiAny>(openApiExample);
	}
}
