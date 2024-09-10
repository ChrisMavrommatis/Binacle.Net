using Swashbuckle.AspNetCore.SwaggerGen;
using ChrisMavrommatis.Swashbuckle;

namespace Binacle.Net.Api.v1;

internal static class ApiVersion
{
	internal const string Number = "1.0";

	internal static Dictionary<Type, Type[]> _polymorphicTypeMappings = new()
	{
		{ typeof(v1.Models.Errors.IApiError), [ 
				typeof(v1.Models.Errors.FieldValidationError), 
				typeof(v1.Models.Errors.ParameterError), 
				typeof(v1.Models.Errors.ExceptionError)
			]
		}
	};

	internal static void ConfigureSwaggerOptions(SwaggerGenOptions options)
	{
		options.AddPolymorphicTypeMappings(_polymorphicTypeMappings);
	}
}

