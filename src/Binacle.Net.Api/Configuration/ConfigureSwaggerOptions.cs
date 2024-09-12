using Asp.Versioning.ApiExplorer;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.SwaggerExamples;
using ChrisMavrommatis.Swashbuckle;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Binacle.Net.Api.Configuration;

internal class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
	private readonly IApiVersionDescriptionProvider _provider;
	
	public ConfigureSwaggerOptions(
		IApiVersionDescriptionProvider provider
		)
	{
		_provider = provider;
	}

	public void Configure(string? name, SwaggerGenOptions options)
	{
		this.ConfigureSwaggerGenOptions(options);
	}

	public void Configure(SwaggerGenOptions options)
	{
		this.ConfigureSwaggerGenOptions(options);
	}

	private void ConfigureSwaggerGenOptions(SwaggerGenOptions options)
	{
		AddApiVersionDocuments(options, _provider.ApiVersionDescriptions);
		options.IncludeXmlCommentsFromAssemblyContaining<IApiMarker>();
		options.UseSwaggerExamples();
		options.UseOneOfForPolymorphism();

		options.TagActionsByEndpointNamespaceOrDefault();
		options.DescribeAllParametersInCamelCase();

		v1.ApiVersion.ConfigureSwaggerOptions(options);
		v2.ApiVersion.ConfigureSwaggerOptions(options);
	}

	private void AddApiVersionDocuments(SwaggerGenOptions options, IReadOnlyList<ApiVersionDescription> apiVersionDescriptions)
	{
		foreach (var description in apiVersionDescriptions.Where(x => x.GroupName.StartsWith("v")))
		{
			var info = new OpenApiInfo()
			{
				Title = $"Binacle.Net API",
				Version = $"{description.ApiVersion.ToString()}",
				Description = __description__,
				// gpl 3 license
				License = new OpenApiLicense
				{
					Name = "GNU General Public License v3.0",
					Url = new Uri("https://www.gnu.org/licenses/gpl-3.0.html")
				}
			};

			// TODO: Obsolete v1
			info.Description = info.Description
				.Replace("{{status}}", description.ApiVersion.MajorVersion == 2 ? "**Warning: This api version is experimental!**": string.Empty)
				.Replace("{{deprecated}}", description.IsDeprecated ? __deprecatedMessage__: string.Empty);

			options.SwaggerDoc(description.GroupName, info);
		}
	}

	public static void ConfigureSwaggerUI(SwaggerUIOptions options, WebApplication app)
	{
		options.RoutePrefix = "swagger";

		var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

		foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
		{
			options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Binacle.Net API {description.GroupName}");
		}
	}

	private const string __description__ = """
		Binacle.NET is an API created to address the 3D Bin Packing Problem in a Bin Selection Variation.
		
		{{status}}

		{{deprecated}}


		[View on Github](https://github.com/ChrisMavrommatis/Binacle.Net)

		[🐳 Binacle.Net on Dockerhub](https://hub.docker.com/r/chrismavrommatis/binacle-net)

		[Get Postman collection](https://www.postman.com/chrismavrommatis/workspace/binacle-net)

		""";

	private const string __deprecatedMessage__ = """

		*This API version has been deprecated. Please use one of the new APIs available from the explorer.*

		""";
}
