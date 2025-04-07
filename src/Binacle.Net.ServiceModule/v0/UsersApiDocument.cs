using System.Text;
using Binacle.Net.Kernel.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0;

internal class UsersApiDocument : IOpenApiDocument
{
	public const string DocumentName = "users";
	public string Name => DocumentName;
	public string Title => "Binacle.Net Users API";
	public string Version => "1.0";
	public bool IsDeprecated => false;
	public bool IsExperimental => false;

	public void Add(IServiceCollection services)
	{
		services.AddOpenApi(this.Name, options =>
		{
			options.AddDocumentTransformer((document, context, cancellationToken) =>
			{
				document.Info.Title = "Binacle.Net Users API";
				document.Info.Version = this.Version;
				document.Info.Description = __description__;
				// gpl 3 license
				document.Info.License = new OpenApiLicense
				{
					Name = "GNU General Public License v3.0",
					Url = new Uri("https://www.gnu.org/licenses/gpl-3.0.html")
				};
				return Task.CompletedTask;
			});
			options.AddResponseDescription();
			options.AddExamples();
			options.AddJwtAuthentication();
		});
	}
	
	private static string __description__ = new StringBuilder()
		.AppendLine("**Binacle.Net Users API for User Management**")
		.AppendLine()
		.AppendLine("This section is designed only for when Binacle.Net is used as public service.")
		.AppendLine()
		.AppendLine("User Management is done only by a user of Admin Group.")
		.AppendLine()
		.AppendLine($"[View on Github]({Binacle.Net.Metadata.GitHub})")
		.AppendLine()
		.AppendLine($"[🐳 Binacle.Net on Dockerhub]({Binacle.Net.Metadata.Dockerhub})")
		.AppendLine()
		.AppendLine($"[Get Postman collection]({Binacle.Net.Metadata.Postman})")
		.AppendLine()
		.ToString();
}
