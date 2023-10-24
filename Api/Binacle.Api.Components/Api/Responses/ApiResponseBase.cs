using Binacle.Api.Components.Models;
using System.Text.Json.Serialization;

namespace Binacle.Api.Components.Api.Responses
{
    public abstract class ApiResponseBase : IApiResponse
    {
        [JsonPropertyOrder(0)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ApiResponseResult Result { get; set; }

        public string Message { get; set; }

    }
}
