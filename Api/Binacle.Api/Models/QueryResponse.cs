using Binacle.Api.Components.Api.Responses;
using Binacle.Api.Glockers.Models;
using Binacle.Lib.Components.Models;

namespace Binacle.Api.Models
{
    public class QueryResponse : ApiResponseBase
    {
        private QueryResponse()
        {
            
        }
        public Container Container { get; set; }

        public static QueryResponse CreateFrom(BinFittingOperationResult result)
        {
            var response = new QueryResponse();

            if (result.Status == BinFitResultStatus.Success)
            {
                response.Container = new Container(result.FoundBin.ID, result.FoundBin);
                response.Result = Components.Models.ApiResponseResult.Success;
                return response;
            }

            response.Result = Components.Models.ApiResponseResult.Error;
            response.Message = $"Failed to find bin. Reason: {result.Reason.ToString()}";
            return response;
        }
    }
}
