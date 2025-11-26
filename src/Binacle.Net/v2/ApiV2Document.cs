using Binacle.Net.Kernel.OpenApi;
using Microsoft.AspNetCore.OpenApi;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v2;

internal class ApiV2Document : IOpenApiDocument
{
	public const string DocumentName = "v2";
	public string Name => DocumentName;
	public string Title => $"Binacle.Net API {DocumentName}";
	public string Version => "2.1";
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
