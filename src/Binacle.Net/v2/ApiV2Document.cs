using Binacle.Net.Kernel.OpenApi;

namespace Binacle.Net.v2;

internal class ApiV2Document : IOpenApiDocument
{
	public const string DocumentName = "v2";
	public string Name => DocumentName;
	public string Title => $"Binacle.Net API {DocumentName}";
	public string Version => "2.0";
	public bool IsDeprecated => false;
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
