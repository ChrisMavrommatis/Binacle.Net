using System.Text;
using Binacle.Net.Api.Kernel.OpenApi;
using Microsoft.OpenApi.Models;

namespace Binacle.Net.Api;


internal static class ApiDocument
{
	internal static void Transform(IOpenApiDocument apiDocument, OpenApiInfo documentInfo)
	{
		documentInfo.Title = $"Binacle.Net API";
		documentInfo.Version = apiDocument.Version;
		documentInfo.Description = __description__
			.Replace("{{status}}", apiDocument.IsExperimental ? __experimentalMessage__: string.Empty)
			.Replace("{{deprecated}}", apiDocument.IsDeprecated ? __deprecatedMessage__: string.Empty);
		documentInfo.License = new OpenApiLicense
		{
			Name = "GNU General Public License v3.0",
			Url = new Uri("https://www.gnu.org/licenses/gpl-3.0.html")
		};
	}
	
	private static string __description__ = new StringBuilder()
		.AppendLine(Binacle.Net.Api.Metadata.Description)
		.AppendLine()
		.AppendLine("{{status}}")
		.AppendLine()
		.AppendLine("{{deprecated}}")
		.AppendLine()
		.AppendLine($"[View on Github]({Binacle.Net.Api.Metadata.GitHub})")
		.AppendLine()
		.AppendLine($"[🐳 Binacle.Net on Dockerhub]({Binacle.Net.Api.Metadata.Dockerhub})")
		.AppendLine()
		.AppendLine($"[Get Postman collection]({Binacle.Net.Api.Metadata.Postman})")
		.AppendLine()
		.ToString();
	
	private const string __experimentalMessage__ =
		"**Warning: This API version is experimental and may change any time, introducing breaking changes.**";
	
	private const string __deprecatedMessage__ =
		"**This API version has been deprecated and will be removed on the next major version.** <br/>" +
		"**Please use one of the newer APIs available from the explorer.**";
}
