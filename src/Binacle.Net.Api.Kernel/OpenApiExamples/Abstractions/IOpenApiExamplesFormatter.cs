using Microsoft.OpenApi.Any;

namespace OpenApiExamples.Abstractions;

public interface IOpenApiExamplesFormatter
{
	IEnumerable<string> SupportedContentTypes { get; }
	ValueTask<IOpenApiAny> FormatAsync(object example);
}
