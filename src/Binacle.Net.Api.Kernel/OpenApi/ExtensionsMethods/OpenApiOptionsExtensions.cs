using Microsoft.AspNetCore.OpenApi;

namespace Binacle.Net.Api;

public static class OpenApiOptionsExtensions
{
	public static OpenApiOptions AddExamples(this OpenApiOptions options)
	{
		options.AddOperationTransformer<OpenApiExamples.ResponseExamplesOperationTransformer>();
		options.AddOperationTransformer<OpenApiExamples.RequestExamplesOperationTransformer>();
		return options;
	}
}
