using System.Text.Json.Serialization;

namespace Binacle.Net.Api.Responses
{
    public class ApiErrorResponse : ApiResponseBase
    {
        new public ApiResponseResultType Result { get; set; } = ApiResponseResultType.Error;

        [JsonPropertyOrder(99)]
        public List<string> Errors { get; set; }
    }
}
