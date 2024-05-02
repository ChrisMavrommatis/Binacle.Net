using Binacle.Net.Api.v1.Models.Errors;
using Swashbuckle.AspNetCore.SwaggerGen;
using ChrisMavrommatis.Swashbuckle;

namespace Binacle.Net.Api.v1;

internal static class ApiVersion
{
	internal const string Number = "1.0";

	internal static Dictionary<Type, Type[]> _polymorphicTypeMappings = new()
	{
		{ typeof(IApiError), new[] { typeof(FieldValidationError), typeof(ParameterError), typeof(ExceptionError), } }
	};

	internal static void ConfigureSwaggerOptions(SwaggerGenOptions options)
	{
		options.AddPolymorphicTypeMappings(_polymorphicTypeMappings);
	}
}

