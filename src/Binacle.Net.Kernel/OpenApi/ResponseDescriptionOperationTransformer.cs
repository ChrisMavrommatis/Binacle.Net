using Binacle.Net.Kernel.OpenApi.Helpers;
using Binacle.Net.Kernel.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

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
			var statusCode = item.StatusCode.ToString();
			var formattedDescription = ResponseDescription.Format(item);
			if (operation.Responses.ContainsKey(statusCode))
			{
				operation.Responses[statusCode].Description = formattedDescription;
			}
			else
			{
				operation.Responses.Add(
					statusCode,
					new OpenApiResponse { Description = formattedDescription }
				);
			}
		}

		return Task.CompletedTask;
	}
}
