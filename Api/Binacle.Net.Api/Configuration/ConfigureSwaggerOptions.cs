using Asp.Versioning.ApiExplorer;
using Binacle.Net.Api.ExtensionMethods;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Binacle.Net.Api.Configuration;

public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

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
        options.AddApiVersionDocuments(_provider.ApiVersionDescriptions);
        options.IncludeXmlCommentsFromAssembly();
        options.AddRequestExamples();
        options.UseOneOfForPolymorphism();
        options.AddPolymorphicResponseTypes();
    }
   
}