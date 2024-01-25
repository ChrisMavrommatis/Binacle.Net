using Binacle.Net.Api.Models.Responses;
using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.Services;

public interface ILockerService
{
    public QueryResponse FindFittingBin<TBin, TBox>(List<TBin> bins, List<TBox> items)
        where TBin : class, IItemWithReadOnlyDimensions<int>
        where TBox : class, IItemWithReadOnlyDimensions<int>, IWithQuantity<int>;

}
