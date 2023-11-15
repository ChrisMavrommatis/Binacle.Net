using System.Text.Json.Serialization;

namespace Binacle.Net.Api.Responses
{
    [JsonDerivedType(typeof(FieldError))]
    public class FieldError : IApiError
    {
        public string Field { get; set; }
        public string Mesasage { get; set; }
    }
}
