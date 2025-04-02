using Binacle.Net.Api.Kernel.OpenApi;

namespace Binacle.Net.Api.v3;

internal class ApiV3Document : IOpenApiDocument
{
	public const string DocumentName = "v3";
	public string Name => DocumentName;
	public string Version => "3.0";
	public bool IsDeprecated => false;
	public bool IsExperimental => true;
	public void Add(IServiceCollection services)
	{
		services.AddOpenApi(this.Name, options =>
		{
			options.AddDocumentTransformer((document, context, cancellationToken) =>
			{
				document.Info = ApiDocument.CreateApiInfo(this);
				return Task.CompletedTask;
			});
		});
	}
	
}
