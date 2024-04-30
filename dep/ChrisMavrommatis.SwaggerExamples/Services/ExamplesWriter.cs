using ChrisMavrommatis.SwaggerExamples.Abstractions;
using Microsoft.OpenApi.Models;

namespace ChrisMavrommatis.SwaggerExamples.Services;

internal class ExamplesWriter
{
	private readonly ExamplesFormatter formatter;
	public ExamplesWriter(ExamplesFormatter formatter)
	{
		this.formatter = formatter;
	}

	public void WriteSingleRequest(OpenApiOperation operation, ISingleSwaggerExamplesProvider provider)
	{
		foreach (var content in operation.RequestBody.Content)
		{
			content.Value.Example = this.formatter.FormatJsonExample(provider);
		}
	}

	public void WriteMultipleRequests(OpenApiOperation operation, IMultipleSwaggerExamplesProvider provider)
	{
		foreach (var content in operation.RequestBody.Content)
		{
			content.Value.Examples = this.formatter.FormatJsonExamples(provider);
		}
	}

	public void WriteSingleResponse(OpenApiOperation operation, int statusCode, ISingleSwaggerExamplesProvider provider)
	{
		var key = statusCode == 0 ? "default" : statusCode.ToString();

		if (!operation.Responses.TryGetValue(key, out var response))
			return;

		foreach (var content in response.Content)
		{
			content.Value.Example = this.formatter.FormatJsonExample(provider);
		}

	}

	public void WriteMultipleResponses(OpenApiOperation operation, int statusCode, IMultipleSwaggerExamplesProvider provider)
	{
		var key = statusCode == 0 ? "default" : statusCode.ToString();

		if (!operation.Responses.TryGetValue(key, out var response))
			return;

		foreach (var content in response.Content)
		{
			content.Value.Examples = this.formatter.FormatJsonExamples(provider);
		}
	}
}
