using Microsoft.OpenApi.Models;

namespace OpenApiExamples.Abstractions;

public interface IOpenApiExamplesWriter
{
	ValueTask WriteSingleExampleAsync(OpenApiMediaType content, string contentType, IOpenApiExample example);

	ValueTask WriteMultipleExamplesAsync(OpenApiMediaType content, string contentType, IEnumerable<IOpenApiExample> examples);
}
