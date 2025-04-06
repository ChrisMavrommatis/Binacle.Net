using Binacle.Net.v3.Models;
using Binacle.Net.v3.Models.Errors;

namespace Binacle.Net.v3.Responses;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class ErrorResponse : ResponseBase<List<IApiError>>
{
}
