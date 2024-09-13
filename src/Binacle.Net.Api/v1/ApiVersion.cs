using Swashbuckle.AspNetCore.SwaggerGen;
using ChrisMavrommatis.Swashbuckle;

namespace Binacle.Net.Api.v1;

internal class ApiVersion : IApiVersion
{
	public const string Number = "1.0";
	public const bool IsDeprecated = true;
	public const bool IsExperimental = false;

	public int MajorNumber => int.Parse(Number.Split('.')[0]);
	public bool Deprecated => IsDeprecated;
	public bool Experimental => IsExperimental;

	internal static Dictionary<Type, Type[]> _polymorphicTypeMappings = new()
	{
		{ typeof(v1.Models.Errors.IApiError), [ 
				typeof(v1.Models.Errors.FieldValidationError), 
				typeof(v1.Models.Errors.ParameterError), 
				typeof(v1.Models.Errors.ExceptionError)
			]
		}
	};

	public void ConfigureSwaggerOptions(SwaggerGenOptions options)
	{
		options.AddPolymorphicTypeMappings(_polymorphicTypeMappings);
	}
}

