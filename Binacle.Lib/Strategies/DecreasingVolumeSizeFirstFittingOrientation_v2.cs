using Binacle.Lib.Components.Exceptions;
using Binacle.Lib.Components.Models;
using Binacle.Lib.Components.Strategies;
using Binacle.Lib.Extensions;

namespace Binacle.Lib.Strategies
{
    internal sealed class DecreasingVolumeSizeFirstFittingOrientation_v2 :
        IBinFittingStrategy,
        IBinFittingStrategyWithBins,
        IBinFittingStrategyWithBinsAndItems,
        IBinFittingOperation
    {
        private IEnumerable<Bin> bins;
        private IEnumerable<Item> items;

        internal DecreasingVolumeSizeFirstFittingOrientation_v2()
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
            int totalItemsToFit = this.items.Count();

            var largestBinByVolume = (this.bins.OrderByDescending(x => x.Volume).FirstOrDefault())!;
            if (this.items.Sum(x => x.Volume) > largestBinByVolume.Volume)
                return BinFittingOperationResult.CreateFailedResult(BinFitFailedResultReason.TotalVolumeExceeded);

            var itemsNotFittingDueToLongestDimension = this.items.Where(x => x.LongestDimension > largestBinByVolume.LongestDimension).ToList();
            if (itemsNotFittingDueToLongestDimension.Any())
                return BinFittingOperationResult.CreateFailedResult(BinFitFailedResultReason.ItemDimensionExceeded, notFittedItems: itemsNotFittingDueToLongestDimension);

            this.bins = this.bins.OrderBy(x => x.Volume);
            this.items = this.items.OrderByDescending(x => x.Volume);

            foreach (var bin in this.bins)
            {
                var fittedItems = this.Fit(bin, items);
               
                if (fittedItems.Count == totalItemsToFit)
                {
                    return BinFittingOperationResult.CreateSuccessfullResult(bin, fittedItems);
                }
            }
            return BinFittingOperationResult.CreateFailedResult(BinFitFailedResultReason.DidNotFit, fittedItems: new List<Item>());
        }

        private List<Item> Fit(Bin bin, IEnumerable<Item> items)
        {
            var availableSpace = new List<VolumetricItemStruct>
                {
                    new VolumetricItemStruct(bin)
                };

            var fittedItems = new List<Item>();

            foreach (var item in items)
            {
                var fitted = false;
                foreach (var orientation in item.GetOrientationsStruct())
                {
                    var availableSpaceQuadrant = availableSpace.FirstOrDefault(x => x.Length >= orientation.Length && x.Width >= orientation.Width && x.Height >= orientation.Height);
                    if (availableSpaceQuadrant.HasDimensions)
                    {
                        var newAvailableSpaces = new List<VolumetricItemStruct>();

                        var remainingLength = availableSpaceQuadrant.Length - orientation.Length;
                        var remainingWidth = availableSpaceQuadrant.Width - orientation.Width;
                        var remainingHeight = availableSpaceQuadrant.Height - orientation.Height;

                        if (remainingLength > 0)
                            newAvailableSpaces.Add(new VolumetricItemStruct(remainingLength, availableSpaceQuadrant.Width, availableSpaceQuadrant.Height));

                        if (remainingWidth > 0)
                            newAvailableSpaces.Add(new VolumetricItemStruct(orientation.Length, remainingWidth, availableSpaceQuadrant.Height));

                        if (remainingHeight > 0)
                            newAvailableSpaces.Add(new VolumetricItemStruct(orientation.Length, orientation.Width, remainingHeight));

                        availableSpace.Remove(availableSpaceQuadrant);
                        if (newAvailableSpaces.Any())
                        {
                            availableSpace.AddRange(newAvailableSpaces);
                        }
                        fitted = true;
                        fittedItems.Add(item);
                        break;
                    }
                }
                if (!fitted)
                    break;
            }

            return fittedItems;
        }
    }
}
