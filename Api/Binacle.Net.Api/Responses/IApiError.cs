using System.Text.Json.Serialization;

namespace Binacle.Net.Api.Responses
{
    [JsonPolymorphic]
    [JsonDerivedType(typeof(FieldError))]
    public interface IApiError
    {

    }
}
