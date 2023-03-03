using Binacle.Lib.Components.Models;

namespace Binacle.Api.Components.Services
{
    public interface ILockerService
    {
        public BinFittingOperationResult FindFittingBin(List<Bin> bins, List<Item> items);
    }
}
