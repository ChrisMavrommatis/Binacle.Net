using Binacle.Api.Components.Api.Responses;
using Binacle.Lib.Components.Models;

namespace Binacle.Api.Glockers.Models
{
    public class GlockersQueryResponse : ApiResponseBase
    {
        public Locker Locker { get; set; }

        internal static GlockersQueryResponse CreateFrom(BinFittingOperationResult result)
        {
            var response = new GlockersQueryResponse();

            if(result.Status == BinFitResultStatus.Success)
            {
                response.Locker = Models.Locker.From(result.FoundBin!);
                response.Result = Components.Models.ApiResponseResult.Success;
            }
            else
            {
                response.Result = Components.Models.ApiResponseResult.Error;
                response.Message = $"Failed to find bin. Reason: {result.Reason.ToString()}";
            }

            return response;
        }
    }
    
}
