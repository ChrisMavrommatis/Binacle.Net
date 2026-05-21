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
	public bool IsExperimental => false;

	public void Configure(OpenApiOptions options)
	{
		options.AddDocumentTransformer((document, context, cancellationToken) =>
		{
			ApiDocument.Transform(this, document.Info);
			document.Servers?.Clear();
			return Task.CompletedTask;
		});
		options.AddResponseDescription();
		options.AddExamples();
		options.AddJwtAuthentication();
		options.AddRateLimiterResponse();
		options.AddEnumStringsSchema();
	}
}
