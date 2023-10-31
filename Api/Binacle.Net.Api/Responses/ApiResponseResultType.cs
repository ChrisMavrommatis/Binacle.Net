using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Binacle.Net.Api.Responses
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ApiResponseResultType
    {
        [EnumMember(Value = nameof(Success))]
        Success,
        [EnumMember(Value = nameof(Error))]
        Error
    }

}
