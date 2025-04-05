using Microsoft.OpenApi.Models;

namespace OpenApiExamples.Abstractions;

internal interface IOpenApiExamplesWriter
{
	ValueTask WriteAsync(OpenApiMediaType content, string itemContentType, Type itemProviderType);
}
