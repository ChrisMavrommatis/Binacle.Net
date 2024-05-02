using Binacle.Net.Api.Models;
using Binacle.Net.Api.v2.Models.Errors;
using Binacle.Net.Api.v2.Responses;
using ChrisMavrommatis.Swashbuckle;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Binacle.Net.Api.v2;

internal static class ApiVersion
{
	internal const string Number = "2.0";

	private static Dictionary<Type, Type[]> _polymorphicTypeMappings = new()
	{
		{ typeof(IApiError), new[] { typeof(FieldValidationError), typeof(ParameterError), typeof(ExceptionError), } }
	};

	private static Dictionary<Type, string> _schemaTypeNameMappings = new()
	{
		{ typeof(Response<Dictionary<string, List<Bin>>>), "PresetListResponse" },
		{ typeof(Response<List<IApiError>>), "ErrorResponse" },
		{ typeof(Response<Bin?>), "QueryResponse" }
	};

	private class CustomSchemaTypeNamesSchemaFilter : ISchemaFilter
	{
		public void Apply(OpenApiSchema schema, SchemaFilterContext context)
		{
			if (_schemaTypeNameMappings.TryGetValue(context.Type, out var name))
			{
				schema.Title = name;
			}
		}
	}

	internal static void ConfigureSwaggerOptions(SwaggerGenOptions options)
	{
		options.AddPolymorphicTypeMappings(_polymorphicTypeMappings);
		options.SchemaFilter<CustomSchemaTypeNamesSchemaFilter>();
	}
}
