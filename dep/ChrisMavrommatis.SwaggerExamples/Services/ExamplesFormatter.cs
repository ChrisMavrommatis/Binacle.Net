using ChrisMavrommatis.SwaggerExamples.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text.Json;

namespace ChrisMavrommatis.SwaggerExamples.Services;

internal class ExamplesFormatter
{
	private readonly IOptions<ExamplesFormatterOptions> options;

	public ExamplesFormatter(IOptions<ExamplesFormatterOptions> options)
	{
		this.options = options;
	}

	public IOpenApiAny FormatJsonExample(ISingleSwaggerExamplesProvider provider)
	{
		var example = provider.GetExample();
		var jsonSerializedExample = JsonSerializer.Serialize(example, this.options.Value.JsonSerializerOptions);
		return new OpenApiString(jsonSerializedExample);
	}

	public IDictionary<string, OpenApiExample> FormatJsonExamples(IMultipleSwaggerExamplesProvider provider)
	{
		var uniqueExamples = provider.GetExamples().DistinctBy(x => x.Key);
		var examples = new Dictionary<string, OpenApiExample>();
		foreach (var example in uniqueExamples)
		{
			var jsonSerializedExample = JsonSerializer.Serialize(example.Value, this.options.Value.JsonSerializerOptions);
			examples.Add(example.Key, new OpenApiExample
			{
				Summary = example.Summary,
				Description = example.Description,
				Value = new OpenApiString(jsonSerializedExample)
			});
		}

		return examples;
	}
}
