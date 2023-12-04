using Binacle.Net.Api.Models;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Models;

namespace Binacle.Net.Api.Services
{
    public interface ILockerService
    {
        public QueryResponse FindFittingBin<TBin, TBox>(List<TBin> bins, List<TBox> items)
            where TBin : class, IWithID, IWithReadOnlyDimensions<int>
            where TBox : class, IWithID, IWithReadOnlyDimensions<int>, IWithQuantity<int>;

    }
}
