using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace Binacle.Net.Api.v3.Models.Errors;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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
