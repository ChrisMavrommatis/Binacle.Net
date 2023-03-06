using Binacle.Lib.Components.Models;

namespace Binacle.Api.Components.Services
{
    public interface ILockerService
    {
        public Task<BinFittingOperationResult> FindFittingBinAsync(List<Bin> bins, List<Item> items);
    }
}
