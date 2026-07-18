using Binacle.Net.Kernel.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.OpenApi.Transformers;

internal class OperationIdOperationTransformer : IOpenApiOperationTransformer
{
	public Task TransformAsync(
		OpenApiOperation operation,
		OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken
	)
	{
		var metadata = context.Description.ActionDescriptor.EndpointMetadata
			.OfType<OperationIdMetadata>()
			.LastOrDefault();
		if (metadata is not null)
		{
			operation.OperationId = metadata.OperationId;
		}

		return Task.CompletedTask;
	}
}
