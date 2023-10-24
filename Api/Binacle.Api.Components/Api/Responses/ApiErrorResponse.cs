using Binacle.Api.Components.Models;
using System.Text.Json.Serialization;

namespace Binacle.Api.Components.Api.Responses
{
    public class ApiErrorResponse : ApiResponseBase
    {
        new public ApiResponseResult Result { get; set; } = ApiResponseResult.Error;

        [JsonPropertyOrder(99)]
        public List<string> Errors { get; set; }
    }
}
