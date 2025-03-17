using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace Binacle.Net.Api.v2.Models.Errors;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[SwaggerDiscriminator("$type")]

[JsonDerivedType(typeof(FieldValidationError), nameof(FieldValidationError))]
[SwaggerSubType(typeof(FieldValidationError), DiscriminatorValue = nameof(FieldValidationError))]

[JsonDerivedType(typeof(ParameterError), nameof(ParameterError))]
[SwaggerSubType(typeof(ParameterError), DiscriminatorValue = nameof(ParameterError))]

[JsonDerivedType(typeof(ExceptionError), nameof(ExceptionError))]
[SwaggerSubType(typeof(ExceptionError), DiscriminatorValue = nameof(ExceptionError))]
public interface IApiError
{
	public static string TypeName { get; set; }
}
