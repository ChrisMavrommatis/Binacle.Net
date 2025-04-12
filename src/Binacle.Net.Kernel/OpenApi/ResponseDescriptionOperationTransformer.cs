using Binacle.Net.Kernel.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Binacle.Net.Kernel.OpenApi;

internal class ResponseDescriptionOperationTransformer : IOpenApiOperationTransformer
{
	public Task TransformAsync(
		OpenApiOperation operation,
		OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken
	)
	{
		var metadata = context.Description.ActionDescriptor.EndpointMetadata
			.OfType<ResponseDescriptionMetadata>()
			.ToList();
		if (metadata.Count == 0)
		{
			return Task.CompletedTask;
		}

		foreach (var item in metadata)
		{
			if (operation.Responses.ContainsKey(item.StatusCode.ToString()))
			{
				operation.Responses[item.StatusCode.ToString()].Description = item.Description;
			}
			else
			{
				operation.Responses.Add(
					item.StatusCode.ToString(),
					new OpenApiResponse { Description = item.Description }
				);
			}
		}

		return Task.CompletedTask;
	}
}
