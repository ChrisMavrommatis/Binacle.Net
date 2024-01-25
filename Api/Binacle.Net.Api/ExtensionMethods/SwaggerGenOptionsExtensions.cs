using Asp.Versioning.ApiExplorer;
using Binacle.Net.Api.Examples;
using Binacle.Net.Api.Models.Responses.Errors;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Binacle.Net.Api.ExtensionMethods;

public static class SwaggerGenOptionsExtensions
{
    public static SwaggerGenOptions AddApiVersionDocuments(this SwaggerGenOptions options, IReadOnlyList<ApiVersionDescription> apiVersionDescriptions)
    {
        foreach (var description in apiVersionDescriptions)
        {

            var info = new OpenApiInfo()
            {
                Title = $"Binacle API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "Binacle.Net.Api",
                Contact = new OpenApiContact() { Name = "Chris Mavrommatis", Email = "" },
                License = new OpenApiLicense() { Name = "GPLv3", Url = new Uri("https://www.gnu.org/licenses/gpl-3.0.en.html") }
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated. Please use one of the new APIs available from the explorer.";
            }

            options.SwaggerDoc(description.GroupName, info);
        }

        return options;
    }
    public static SwaggerGenOptions IncludeXmlCommentsFromAssembly(this SwaggerGenOptions options)
    {
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        return options;
    }

    public static SwaggerGenOptions AddRequestExamples(this SwaggerGenOptions options)
    {
        options.SchemaFilter<PresetQueryRequestExampleSchemaFilter>();
        //options.SchemaFilter<SwaggerExamplesSchemaFilter>();
        return options;
    }

    public static SwaggerGenOptions AddPolymorphicResponseTypes(this SwaggerGenOptions options)
    {
        options.SelectSubTypesUsing(baseType =>
        {
            if (baseType == typeof(IApiError))
            {
                return new[]
                {
                    typeof(FieldValidationError),
                    typeof(ParameterError),
                };
            }
            return Enumerable.Empty<Type>();
        });

        return options;
    }
}
