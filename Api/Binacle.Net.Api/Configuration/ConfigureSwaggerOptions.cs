using Asp.Versioning.ApiExplorer;
using Binacle.Net.Api.Examples;
using Binacle.Net.Api.Models.Responses.Errors;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

namespace Binacle.Net.Api.Configuration;

public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
	private readonly IApiVersionDescriptionProvider _provider;

	private static Dictionary<Type, Type[]> polymorphicTypeMappings = new()
	{
		{ typeof(IApiError), new[] { typeof(FieldValidationError), typeof(ParameterError), typeof(ExceptionError), } }
	};

	public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
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
		options.IncludeXmlCommentsFromAssembly(Assembly.GetExecutingAssembly());
		AddRequestExamples(options);
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
				Title = $"Binacle API {description.ApiVersion}",
				Version = description.ApiVersion.ToString(),
				Description = "Binacle API is an API that provides a way to to solve the bin fitting problem in one dimension only.",
				Contact = new OpenApiContact() { Name = "Chris Mavrommatis", Email = "" },
				License = new OpenApiLicense() { Name = "GPLv3", Url = new Uri("https://www.gnu.org/licenses/gpl-3.0.en.html") }
			};

			if (description.IsDeprecated)
			{
				info.Description += " This API version has been deprecated. Please use one of the new APIs available from the explorer.";
			}

			options.SwaggerDoc(description.GroupName, info);
		}
	}

	private void AddRequestExamples(SwaggerGenOptions options)
	{
		options.SchemaFilter<PresetQueryRequestExampleSchemaFilter>();
	}

	internal static void ConfigureSwaggerUIOptions(WebApplication app, SwaggerUIOptions options)
	{
		var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

		options.RoutePrefix = "swagger";
		foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
		{
			options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Binacle API {description.GroupName.ToUpperInvariant()}");
		}
	}
}
