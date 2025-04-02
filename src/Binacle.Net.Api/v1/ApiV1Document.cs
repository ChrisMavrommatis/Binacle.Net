using Binacle.Net.Api.Kernel.OpenApi;

namespace Binacle.Net.Api.v1;

internal class ApiV1Document : IOpenApiDocument
{
	public const string DocumentName = "v1";
	public string Name => DocumentName;
	public string Version => "1.0";
	public bool IsDeprecated => true;
	public bool IsExperimental => false;
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
