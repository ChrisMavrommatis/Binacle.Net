using System.Text.Json;
using OpenApiExamples.Abstractions;

namespace OpenApiExamples;

public class OpenApiExamplesOptions
{
	public Dictionary<string, IOpenApiExamplesFormatter> Formatters { get; } = new();
	
	public JsonSerializerOptions JsonSerializerOptions { get; set; } = new();

	public XmlSerializerOptions XmlSerializerOptions { get; set; } = new();
}
