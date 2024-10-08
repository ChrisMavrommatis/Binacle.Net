using ChrisMavrommatis.Swashbuckle;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Binacle.Net.Api.v2;

internal class ApiVersion : IApiVersion
{
	public const string Number = "2.0";
	public const bool IsDeprecated = false;
	public const bool IsExperimental = false;

	public int MajorNumber => int.Parse(Number.Split('.')[0]);
	public bool Deprecated => IsDeprecated;
	public bool Experimental => IsExperimental;

	private static Dictionary<Type, Type[]> _polymorphicTypeMappings = new()
	{
		{ typeof(v2.Models.Errors.IApiError), [
				typeof(v2.Models.Errors.FieldValidationError), 
				typeof(v2.Models.Errors.ParameterError), 
				typeof(v2.Models.Errors.ExceptionError)
			] 
		}
	};

	public void ConfigureSwaggerOptions(SwaggerGenOptions options)
	{
		options.AddPolymorphicTypeMappings(_polymorphicTypeMappings);
	}
}
