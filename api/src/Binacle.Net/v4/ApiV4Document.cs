using Binacle.Net.Kernel.OpenApi;
using Microsoft.AspNetCore.OpenApi;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v4;

internal class ApiV4Document : IOpenApiDocument
{
	public const string DocumentName = "v4";
	public string Name => DocumentName;
	public string Title => $"Binacle.Net API {DocumentName}";
	public string Version => "4.0";
	public bool IsDeprecated => false;
	// v4 ships experimental in 3.0.0 so its contracts can still change; it goes stable in 3.1.0, once it has run
	// in a real deployment and gained an endpoint without reshaping an existing one.
	public bool IsExperimental => true;

	public void Configure(OpenApiOptions options)
	{
		options.AddDocumentTransformer((document, context, cancellationToken) =>
		{
			ApiDocument.Transform(this, document);
			return Task.CompletedTask;
		});
		options.AddResponseDescription();
		options.AddOperationIds();
		options.AddExamples();
		options.AddJwtAuthentication();
		options.AddRateLimiterResponse();
		options.AddEnumStringsSchema();
		options.AddNumericSchemas();
		options.AddProblemDetailsDescriptions();
		options.AddRequiredNonNullableProperties();
	}
}
