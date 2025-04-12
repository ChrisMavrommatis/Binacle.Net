using Binacle.Net.Kernel.OpenApi;
using OpenApiExamples;

namespace Binacle.Net.v3;

internal class ApiV3Document : IOpenApiDocument
{
	public const string DocumentName = "v3";
	public string Name => DocumentName;
	public string Title => $"Binacle.Net API {DocumentName}";
	public string Version => "3.0";
	public bool IsDeprecated => false;
	public bool IsExperimental => true;
	public void Add(IServiceCollection services)
	{
		services.AddOpenApi(this.Name, options =>
		{
			options.AddDocumentTransformer((document, context, cancellationToken) =>
			{
				ApiDocument.Transform(this, document.Info);
				return Task.CompletedTask;
			});
			options.AddResponseDescription();
			options.AddExamples();
			options.AddJwtAuthentication();
			options.AddRateLimiterResponse();
		});
	}
	
}
