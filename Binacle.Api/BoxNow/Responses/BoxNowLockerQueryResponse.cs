using Binacle.Api.BoxNow.Models;
using Binacle.Api.Responses;

namespace Binacle.Api.BoxNow.Responses
{
    public class BoxNowLockerQueryResponse : ApiResponseBase
    {
        public LockerBin Locker { get; set; }
    }
}
