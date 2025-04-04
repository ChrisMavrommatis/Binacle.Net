using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenApiExamples.Abstractions;
using OpenApiExamples.Models;

namespace OpenApiExamples;

public class ResponseExamplesOperationTransformer : IOpenApiOperationTransformer
{
	public async Task TransformAsync(
		OpenApiOperation operation,
		OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken
	)
	{
		var metadata = context.Description.ActionDescriptor.EndpointMetadata
			.OfType<ResponseExampleMetadata>()
			.ToList();
		if (metadata.Count == 0)
		{
			return;
		}

		var examplesWritter = context.ApplicationServices
			.GetRequiredService<IOpenApiExamplesWriter>();

		var logger = context.ApplicationServices
			.GetRequiredService<ILoggerFactory>()
			?.CreateLogger(nameof(ResponseExamplesOperationTransformer));
		
		foreach (var item in metadata)
		{
			if (!operation.Responses.TryGetValue(item.StatusCode, out var response))
			{
				continue;
			}

			if (!response.Content.TryGetValue(item.ContentType, out var content))
			{
				continue;
			}

			var providerInstance = ActivatorUtilities.CreateInstance(
				context.ApplicationServices,
				item.ProviderType
			);

			if (providerInstance is ISingleOpenApiExamplesProvider singleExamplesProvider)
			{
				var example = singleExamplesProvider.GetExample();
				await examplesWritter.WriteSingleExampleAsync(content, item.ContentType, example);
			}
			else if (providerInstance is IMultipleOpenApiExamplesProvider multipleExamplesProvider)
			{
				var examples = multipleExamplesProvider.GetExamples();
				await examplesWritter.WriteMultipleExamplesAsync(content, item.ContentType, examples);
			}
			else
			{
				logger?.LogWarning(
					"Provider type {ProviderType} should be either ISingleOpenApiExamplesProvider or IMultipleOpenApiExamplesProvider",
					providerInstance.GetType().Name
				);
			}

		}
	}
}
