using Asp.Versioning.ApiExplorer;
using ChrisMavrommatis.Swashbuckle;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Binacle.Net.Api.v3;

internal class ApiVersion : IApiVersion
{
	public const string Number = "3.0";
	public const bool IsDeprecated = false;
	public const bool IsExperimental = true;

	public int MajorNumber => int.Parse(Number.Split('.')[0]);
	public bool Deprecated => IsDeprecated;
	public bool Experimental => IsExperimental;

	public void ConfigureSwaggerOptions(SwaggerGenOptions options, IApiVersionDescriptionProvider provider)
	{
		options.AddPolymorphicTypeMappings(_polymorphicTypeMappings);
		//options.SchemaFilter<CustomSchemaTypeNamesSchemaFilter>();
			
		var description = provider.ApiVersionDescriptions
			.FirstOrDefault(x => x.ApiVersion.MajorVersion == this.MajorNumber)!;
		
		var info = ApiDocument.CreateApiInfo(this, description);
		
		options.SwaggerDoc(description.GroupName, info);
	}

	public void ConfigureSwaggerUI(SwaggerUIOptions options, IApiVersionDescriptionProvider provider)
	{
		var description = provider.ApiVersionDescriptions
			.FirstOrDefault(x => x.ApiVersion.MajorVersion == this.MajorNumber);

		if (description is not null)
		{
			options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Binacle.Net API {description.GroupName}");
		}
	}
	
	private static Dictionary<Type, Type[]> _polymorphicTypeMappings = new()
	{
		{ typeof(v3.Models.Errors.IApiError), [
				typeof(v3.Models.Errors.FieldValidationError), 
				typeof(v3.Models.Errors.ParameterError), 
				typeof(v3.Models.Errors.ExceptionError)
			] 
		}
	};

	//private static Dictionary<Type, string> _schemaTypeNameMappings = new()
	//{
	//	{ typeof(Response<Dictionary<string, List<Bin>>>), "PresetListResponse" },
	//	{ typeof(Response<List<IApiError>>), "ErrorResponse" },
	//	{ typeof(Response<Bin?>), "QueryResponse" }
	//};

	//private class CustomSchemaTypeNamesSchemaFilter : ISchemaFilter
	//{
	//	public void Apply(OpenApiSchema schema, SchemaFilterContext context)
	//	{
	//		if (_schemaTypeNameMappings.TryGetValue(context.Type, out var name))
	//		{
	//			schema.Title = name;
	//		}
	//	}
	//}
}
