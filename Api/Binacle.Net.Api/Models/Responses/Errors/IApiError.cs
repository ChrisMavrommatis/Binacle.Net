using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace Binacle.Net.Api.Models.Responses.Errors;

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
