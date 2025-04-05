using Binacle.Net.Api.Kernel.OpenApi;

namespace Binacle.Net.Api.v1;

internal class ApiV1Document : IOpenApiDocument
{
	public const string DocumentName = "v1";
	public string Name => DocumentName;
	public string Title => $"Binacle.Net API {DocumentName}";
	public string Version => "1.0";
	public bool IsDeprecated => true;
	public bool IsExperimental => false;
	public void Add(IServiceCollection services)
	{
		services.AddOpenApi(this.Name, options =>
		{
			options.AddDocumentTransformer((document, context, cancellationToken) =>
			{
				ApiDocument.Transform(this, document.Info);
				return Task.CompletedTask;
			});
			options.AddOperationTransformer<ResponseDescriptionOperationTransformer>();
			options.AddExamples();
		});
	}
	

}
