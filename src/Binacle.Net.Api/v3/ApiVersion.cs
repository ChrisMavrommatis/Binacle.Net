using ChrisMavrommatis.Swashbuckle;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Binacle.Net.Api.v3;

internal class ApiVersion : IApiVersion
{
	public const string Number = "3.0";
	public const bool IsDeprecated = false;
	public const bool IsExperimental = true;

	public int MajorNumber => int.Parse(Number.Split('.')[0]);
	public bool Deprecated => IsDeprecated;
	public bool Experimental => IsExperimental;

	//private static Dictionary<Type, Type[]> _polymorphicTypeMappings = new()
	//{
	//	{ typeof(v3.Models.Errors.IApiError), [
	//			typeof(v3.Models.Errors.FieldValidationError), 
	//			typeof(v3.Models.Errors.ParameterError), 
	//			typeof(v3.Models.Errors.ExceptionError)
	//		] 
	//	}
	//};

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

	public void ConfigureSwaggerOptions(SwaggerGenOptions options)
	{
		//options.AddPolymorphicTypeMappings(_polymorphicTypeMappings);
		//options.SchemaFilter<CustomSchemaTypeNamesSchemaFilter>();
	}
}
