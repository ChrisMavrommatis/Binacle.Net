using Binacle.Api.Models;
using System.Text.Json.Serialization;

namespace Binacle.Api.Responses
{
    public abstract class ApiResponseBase : IApiResponse
    {
        [JsonPropertyOrder(0)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ApiResponseResult Result { get; set; }
    }
}
