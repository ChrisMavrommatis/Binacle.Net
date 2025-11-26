using System.Text;
using Binacle.Net.Kernel.OpenApi;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.ServiceModule.v0;

internal class ServiceModuleApiDocument : IOpenApiDocument
{
	public const string DocumentName = "service";
	public string Name => DocumentName;
	public string Title => "Binacle.Net Service";
	public string Version => "1.0";
	public bool IsDeprecated => false;
	public bool IsExperimental => false;

	public void Configure(OpenApiOptions options)
	{
		options.AddDocumentTransformer((document, context, cancellationToken) =>
		{
			document.Info.Title = this.Title;
			document.Info.Version = this.Version;
			document.Info.Description = __description__;
			// gpl 3 license
			document.Info.License = new OpenApiLicense
			{
				Name = "GNU General Public License v3.0",
				Url = new Uri("https://www.gnu.org/licenses/gpl-3.0.html")
			};

			document.Servers?.Clear();
			return Task.CompletedTask;
		});
		options.AddResponseDescription();
		options.AddExamples();
		options.AddJwtAuthentication();
		options.AddRateLimiterResponse();
		options.AddEnumStringsSchema();

	}

	private static string __description__ = new StringBuilder()
		.AppendLine("**Binacle.Net Service API**")
		.AppendLine()
		.AppendLine("This api is designed only for when Binacle.Net is used as public service.")
		.AppendLine()
		.AppendLine($"[View on Github]({Binacle.Net.Metadata.GitHub})")
		.AppendLine()
		.AppendLine($"[🐳 Binacle.Net on Dockerhub]({Binacle.Net.Metadata.Dockerhub})")
		.AppendLine()
		.AppendLine($"[Get Postman collection]({Binacle.Net.Metadata.Postman})")
		.AppendLine()
		.ToString();
}
