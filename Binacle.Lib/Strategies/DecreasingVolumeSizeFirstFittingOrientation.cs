using Binacle.Lib.Components.Models;
using Binacle.Lib.Components.Strategies;

namespace Binacle.Lib.Strategies
{
    internal sealed class DecreasingVolumeSizeFirstFittingOrientation :
        IBinFittingStrategy,
        IBinFittingStrategyWithBins,
        IBinFittingStrategyWithBinsAndItems,
        IBinFittingOperation
    {

        private List<VolumetricItem> availableSpace;
        private IEnumerable<Bin> bins;
        private IEnumerable<Item> items;

        internal DecreasingVolumeSizeFirstFittingOrientation()
        {
        }

        public IBinFittingStrategyWithBins WithBins(List<Bin> bins)
        {
            this.bins = bins;
            return this;
        }

        public IBinFittingStrategyWithBinsAndItems AndItems(List<Item> items)
        {
            this.items = items;
            return this;
        }

        public IBinFittingOperation Build()
        {
            //if (!(this.bins?.Any() ?? false))
            //    throw new System.ArgumentNullException($"{nameof(bins)} is empty. At least one bin is required");

            //if (!(this.items?.Any() ?? false))
            //    throw new ArgumentNullException($"{nameof(items)} is empty. At least one item is required");

            //if(this.bins.Any(x => x.Volume <= 0))
            //    throw new ArgumentNullException($"You cannot have a bin with negative or 0 length dimensions");

            //if (this.items.Any(x => x.Volume <= 0))
            //    throw new ArgumentNullException($"You cannot have an item with negative or 0 length dimensions");

            //this.ResetAvailableSpace();
            //return this;

            throw new NotImplementedException();

        }

        

        public BinFittingOperationResult Execute()
        {
            throw new NotImplementedException();
        }

        private void ResetAvailableSpace()
        {
            this.availableSpace = new List<VolumetricItem>();

        }
    }
}
