using System.Text.Json;

namespace ChrisMavrommatis.SwaggerExamples;

public class ExamplesFormatterOptions
{
	public JsonSerializerOptions JsonSerializerOptions { get; set; } = new();
}
