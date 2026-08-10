using Binacle.Net.Kernel.OpenApi;
using Scalar.AspNetCore;

namespace Binacle.Net.ExtensionMethods;

internal static class OpenApiUiExtensions
{
	public static void MapApiDocumentUi(this WebApplication app, bool swaggerEnabled, bool scalarEnabled)
	{
		if (!swaggerEnabled && !scalarEnabled)
		{
			return;
		}

		const string openApiEndpointPattern = "/openapi/{documentName}.json";
		app.MapOpenApi(openApiEndpointPattern);

		var openApiDocuments = app.Services.GetServices<IOpenApiDocument>();

		if (swaggerEnabled)
		{
			app.UseSwaggerUI(options =>
			{
				foreach (var openApiDocument in openApiDocuments)
				{
					var endpoint = openApiEndpointPattern.Replace("{documentName}", openApiDocument.Name);
					options.SwaggerEndpoint(endpoint, openApiDocument.Title);
				}

				options.EnablePersistAuthorization();
				options.EnableValidator();
				options.EnableDeepLinking();
				options.DisplayRequestDuration();
			});
		}

		if (scalarEnabled)
		{
			app.MapScalarApiReference(options =>
			{
				foreach (var openApiDocument in openApiDocuments)
				{
					options.AddDocument(openApiDocument.Name, openApiDocument.Title);
				}
			});
		}
	}
}
