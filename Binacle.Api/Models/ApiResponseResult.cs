using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Binacle.Api.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ApiResponseResult
    {
        [EnumMember(Value = nameof(Success))]
        Success,
        [EnumMember(Value = nameof(Error))]
        Error
    }

}
