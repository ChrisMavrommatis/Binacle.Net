using Asp.Versioning.ApiExplorer;
using Binacle.Net.Api.Models.Responses.Errors;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.SwaggerExamples;
using ChrisMavrommatis.Swashbuckle;

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
				Title = $"Binacle API {description.ApiVersion}",
				Version = description.ApiVersion.ToString(),
				Description = "Binacle API is an API that provides a way to to solve the bin fitting problem in one dimension only.",
				License = new OpenApiLicense() { Name = "View on Github", Url = new Uri("https://github.com/ChrisMavrommatis/Binacle.Net") },
			};

			if (description.IsDeprecated)
			{
				info.Description += " This API version has been deprecated. Please use one of the new APIs available from the explorer.";
			}

			options.SwaggerDoc(description.GroupName, info);
		}
	}

	public static void ConfigureSwaggerUI(SwaggerUIOptions options, WebApplication app)
	{
		options.RoutePrefix = "swagger";

		var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

		foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
		{
			options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Binacle API {description.GroupName.ToUpperInvariant()}");
		}
	}
}
