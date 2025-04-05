using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OpenApiExamples.Abstractions;
using OpenApiExamples.Models;

namespace OpenApiExamples;

internal class RequestExamplesOperationTransformer : IOpenApiOperationTransformer
{
	public async Task TransformAsync(
		OpenApiOperation operation,
		OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken
	)
	{
		var metadata = context.Description.ActionDescriptor.EndpointMetadata
			.OfType<RequestExampleMetadata>()
			.ToList();

		if (metadata.Count == 0)
		{
			return;
		}

		var examplesWriter = context.ApplicationServices
			.GetRequiredService<IOpenApiExamplesWriter>();

		foreach (var item in metadata)
		{
			if (!operation.RequestBody.Content.TryGetValue(item.ContentType, out var content))
			{
				continue;
			}

			await examplesWriter.WriteAsync(content, item.ContentType, item.ProviderType);
		}
	}
}
