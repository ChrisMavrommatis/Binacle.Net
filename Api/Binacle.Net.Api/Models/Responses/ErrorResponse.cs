using Binacle.Net.Api.Models.Responses.Errors;
using System.Text.Json.Serialization;

namespace Binacle.Net.Api.Models.Responses;

public class ErrorResponse : ResponseBase
{
    public ErrorResponse()
    {
        this.Result = ResultType.Error;
        this.Errors = new List<IApiError>();
    }

    [JsonPropertyOrder(99)]
    public List<IApiError> Errors { get; set; }
}
