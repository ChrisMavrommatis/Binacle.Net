using System.Text.Json.Serialization;

namespace Binacle.Net.Api.Responses
{
    public abstract class ApiResponseBase
    {
        [JsonPropertyOrder(0)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ApiResponseResultType Result { get; set; }

        public string Message { get; set; }
    }
}
