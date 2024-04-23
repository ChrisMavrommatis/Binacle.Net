using Binacle.Net.Api.v1.Models.Errors;

namespace Binacle.Net.Api.v1;

internal static class ModuleApiVersion
{
	internal static Dictionary<Type, Type[]> PolymorphicTypeMappings = new()
	{
		{ typeof(IApiError), new[] { typeof(FieldValidationError), typeof(ParameterError), typeof(ExceptionError), } }
	};
}

