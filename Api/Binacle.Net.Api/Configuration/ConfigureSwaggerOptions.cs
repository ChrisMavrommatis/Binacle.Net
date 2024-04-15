using Asp.Versioning.ApiExplorer;
using Binacle.Net.Api.Models.Responses.Errors;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.SwaggerExamples;
using ChrisMavrommatis.Swashbuckle;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Binacle.Net.Api.Configuration;

public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
	private readonly IApiVersionDescriptionProvider _provider;
	private static Dictionary<Type, Type[]> polymorphicTypeMappings = new()
	{
		{ typeof(IApiError), new[] { typeof(FieldValidationError), typeof(ParameterError), typeof(ExceptionError), } }
	};

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
		// options.SchemaFilter<PresetQueryRequestExampleSchemaFilter>();
		options.UseOneOfForPolymorphism();
		options.AddPolymorphicTypeMappings(polymorphicTypeMappings);
		options.TagActionsByEndpointNamespaceOrDefault();
		options.DescribeAllParametersInCamelCase();
	}

	private void AddApiVersionDocuments(SwaggerGenOptions options, IReadOnlyList<ApiVersionDescription> apiVersionDescriptions)
	{
		foreach (var description in apiVersionDescriptions)
		{
			var info = new OpenApiInfo()
			{
				Title = $"Binacle.Net API {description.ApiVersion}",
				Version = description.ApiVersion.ToString(),
				Description = __description__,
				// gpl 3 license
				License = new OpenApiLicense
				{
					Name = "GNU General Public License v3.0",
					Url = new Uri("https://www.gnu.org/licenses/gpl-3.0.html")
				},

			};

			info.Description = info.Description.Replace("{{deprecated}}", description.IsDeprecated ? __deprecatedMessage__: string.Empty);

			options.SwaggerDoc(description.GroupName, info);
		}
	}

	public static void ConfigureSwaggerUI(SwaggerUIOptions options, WebApplication app)
	{
		options.RoutePrefix = "swagger";

		var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

		foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
		{
			options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Binacle.Net API {description.GroupName.ToUpperInvariant()}");
		}
	}

	private const string __description__ = """
		Binacle.Net is an API that provides a way to to solve the bin fitting problem in one dimension only.
		
		{{deprecated}}

		[View on Github](https://github.com/ChrisMavrommatis/Binacle.Net)

		[Get Postman collection](https://www.postman.com/chrismavrommatis/workspace/binacle-net)

		""";

	private const string __deprecatedMessage__ = """

		*This API version has been deprecated. Please use one of the new APIs available from the explorer.*

		""";
}
