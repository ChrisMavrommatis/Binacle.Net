using Binacle.Api.BoxNow.Models;
using Binacle.Api.Components.Api.Responses;

namespace Binacle.Api.BoxNow.Responses
{
    public class BoxNowLockerQueryResponse : ApiResponseBase
    {
        public LockerBin Locker { get; set; }
    }
}
