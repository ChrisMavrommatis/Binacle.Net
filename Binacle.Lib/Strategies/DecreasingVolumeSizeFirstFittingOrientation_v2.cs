using Binacle.Lib.Components.Exceptions;
using Binacle.Lib.Components.Models;
using Binacle.Lib.Components.Strategies;
using Binacle.Lib.Extensions;
using Binacle.Lib.Models;
using ExternalItem = Binacle.Lib.Components.Models.Item;
using Item = Binacle.Lib.Models.Item;

namespace Binacle.Lib.Strategies
{
    internal sealed class DecreasingVolumeSizeFirstFittingOrientation_v2 :
        IBinFittingStrategy,
        IBinFittingStrategyWithBins,
        IBinFittingStrategyWithBinsAndItems,
        IBinFittingOperation
    {
        private List<VolumetricItem> availableSpace;
        private List<Item> fittedItems;
        private IEnumerable<Bin> bins;
        private IEnumerable<Item> items;

        internal DecreasingVolumeSizeFirstFittingOrientation_v2()
        {
        }

        public IBinFittingStrategyWithBins WithBins(IEnumerable<ExternalItem> bins)
        {
            this.bins = bins.Select(x => new Bin(x.ID, x)).ToList();
            return this;
        }

        public IBinFittingStrategyWithBinsAndItems AndItems(IEnumerable<ExternalItem> items)
        {
            this.items = items.Select(x => new Item(x.ID, x)).ToList();
            return this;
        }

        public IBinFittingOperation Build()
        {
            if (!(this.bins?.Any() ?? false))
                throw new System.ArgumentNullException($"{nameof(bins)} is empty. At least one bin is required");

            if (!(this.items?.Any() ?? false))
                throw new ArgumentNullException($"{nameof(items)} is empty. At least one item is required");

            if (this.bins.Any(x => x.Volume <= 0))
                throw new DimensionException("Volume", "You cannot have a bin with negative or 0 volume");

            if (this.items.Any(x => x.Volume <= 0))
                throw new DimensionException("Volume", "You cannot have an item with negative or 0 volume");

            return this;
        }

        public BinFittingOperationResult Execute()
        {
            Bin? foundBin = null;
            int totalItemsToFit = this.items.Count();

            var largestBinByVolume = (this.bins.OrderByDescending(x => x.Volume).FirstOrDefault())!;
            if (this.items.Sum(x => x.Volume) > largestBinByVolume.Volume)
                return BinFittingOperationResult.CreateFailedResult(BinFitFailedResultReason.TotalVolumeExceeded);

            var itemsNotFittingDueToLongestDimension = this.items.Where(x => x.LongestDimension > largestBinByVolume.LongestDimension);
            if (itemsNotFittingDueToLongestDimension.Any())
                return BinFittingOperationResult.CreateFailedResult(BinFitFailedResultReason.ItemDimensionExceeded, this.Convert(itemsNotFittingDueToLongestDimension));

            this.bins = this.bins.OrderBy(x => x.Volume);
            this.items = this.items.OrderByDescending(x => x.Volume);

            foreach (var bin in this.bins)
            {
                this.availableSpace = new List<VolumetricItem>
                {
                    new VolumetricItem(bin)
                };

                this.fittedItems = new List<Item>();

                foreach (var item in this.items)
                {
                    if (!this.TryFit(item))
                        break;
                }
                if (this.fittedItems.Count == totalItemsToFit)
                {
                    foundBin = bin;
                    break;
                }
            }

            if (foundBin != null)
            {
                return BinFittingOperationResult.CreateSuccessfullResult(this.Convert(foundBin), this.Convert(this.fittedItems));
            }

            return BinFittingOperationResult.CreateFailedResult(BinFitFailedResultReason.DidNotFit, fittedItems: this.Convert(this.fittedItems));
        }

        private ExternalItem Convert(Bin bin)
        {
            return new ExternalItem(bin.ID, bin);
        }

        private List<ExternalItem> Convert(IEnumerable<Item> items)
        {
            if (!(items?.Any() ?? false))
                return new List<ExternalItem>();

            return items.Select(x => new ExternalItem(x.ID, x)).ToList();
        }

        public bool TryFit(Item item)
        {
            foreach (var orientation in item.GetOrientations())
            {
                var availableSpaceQuadrant = this.FindAvailableSpace(orientation);
                if (availableSpaceQuadrant != null)
                {
                    this.Fit(availableSpaceQuadrant, orientation, item);
                    return true;
                }
            }
            return false;
        }

        private VolumetricItem? FindAvailableSpace(VolumetricItem orientation)
        {
            foreach(var space in this.availableSpace)
            {
                if (space.Length >= orientation.Length && space.Width >= orientation.Width && space.Height >= orientation.Height)
                    return space;
            }

            return null;
        }

        private void Fit(VolumetricItem spaceQuadrant, VolumetricItem orientation, Item item)
        {
            var newAvailableSpaces = this.SplitSpaceQuadrant(spaceQuadrant, orientation);
            this.availableSpace.Remove(spaceQuadrant);
            if (newAvailableSpaces.Any())
            {
                this.availableSpace.AddRange(newAvailableSpaces);
            }
            this.fittedItems.Add(item);
        }

        private List<VolumetricItem> SplitSpaceQuadrant(VolumetricItem spaceQuadrant, VolumetricItem orientation)
        {
            var newAvailableSpaces = new List<VolumetricItem>();

            var remainingLength = (ushort)(spaceQuadrant.Length - orientation.Length);
            var remainingWidth = (ushort)(spaceQuadrant.Width - orientation.Width);
            var remainingHeight = (ushort)(spaceQuadrant.Height - orientation.Height);

            if (remainingLength > 0)
                newAvailableSpaces.Add(new VolumetricItem(remainingLength, spaceQuadrant.Width, spaceQuadrant.Height));

            if (remainingWidth > 0)
                newAvailableSpaces.Add(new VolumetricItem(orientation.Length, remainingWidth, spaceQuadrant.Height));

            if (remainingHeight > 0)
                newAvailableSpaces.Add(new VolumetricItem(orientation.Length, orientation.Width, remainingHeight));

            return newAvailableSpaces;
        }
    }
}
