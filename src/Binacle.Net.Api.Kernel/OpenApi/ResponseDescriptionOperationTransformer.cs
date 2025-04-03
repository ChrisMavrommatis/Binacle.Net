using Binacle.Net.Api.Kernel.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Binacle.Net.Api.Kernel.OpenApi;

public class ResponseDescriptionOperationTransformer : IOpenApiOperationTransformer
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
		
		foreach (var response in metadata)
		{
			if (operation.Responses.ContainsKey(response.StatusCode.ToString()))
			{
				operation.Responses[response.StatusCode.ToString()].Description = response.Description;
			}
			else
			{
				operation.Responses.Add(
					response.StatusCode.ToString(),
					new OpenApiResponse { Description = response.Description }
				);
			}
		}
		return Task.CompletedTask;
	}
}
