using Binacle.Api.Models;
using System.Text.Json.Serialization;

namespace Binacle.Api.Responses
{
    public class ApiErrorResponse : ApiResponseBase
    {
        new public ApiResponseResult Result { get; set; } = ApiResponseResult.Error;

        [JsonPropertyOrder(99)]
        public List<string> Errors { get; set; }
    }
}
