﻿using Binacle.Net.Api.Kernel.OpenApi;

namespace Binacle.Net.Api.v2;

internal class ApiV2Document : IOpenApiDocument
{
	public const string DocumentName = "v2";
	public string Name => DocumentName;
	public string Version => "2.0";
	public bool IsDeprecated => false;
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
