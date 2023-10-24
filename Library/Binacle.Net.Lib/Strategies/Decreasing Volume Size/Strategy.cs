using Binacle.Net.Lib.Abstractions.Strategies;
using Binacle.Net.Lib.Exceptions;
using Binacle.Net.Lib.Models;
using Binacle.Net.Lib.Strategies.Models;

namespace Binacle.Net.Lib.Strategies
{
    internal sealed partial class DecreasingVolumeSize_v1 : IBinFittingStrategy
    {
        private List<VolumetricItem> _availableSpace;
        private List<Item> _fittedItems;
        private IEnumerable<Bin> _bins;
        private IEnumerable<Item> _items;

        internal DecreasingVolumeSize_v1()
        {

        }

        public IBinFittingStrategyWithBins WithBins(IEnumerable<Lib.Models.Item> bins)
        {
            _bins = bins.Select(x => new Bin(x.ID, x)).ToList();
            return this;
        }

        public IBinFittingStrategyWithBinsAndItems AndItems(IEnumerable<Lib.Models.Item> items)
        {
            _items = items.Select(x => new Item(x.ID, x)).ToList();
            return this;
        }

        public IBinFittingOperation Build()
        {
            if (!(_bins?.Any() ?? false))
                throw new System.ArgumentNullException($"{nameof(_bins)} is empty. At least one bin is required");

            if (!(_items?.Any() ?? false))
                throw new ArgumentNullException($"{nameof(_items)} is empty. At least one item is required");

            if (_bins.Any(x => x.Volume <= 0))
                throw new DimensionException("Volume", "You cannot have a bin with negative or 0 volume");

            if (_items.Any(x => x.Volume <= 0))
                throw new DimensionException("Volume", "You cannot have an item with negative or 0 volume");

            return this;
        }

        public BinFittingOperationResult Execute()
        {
            Bin? foundBin = null;
            int totalItemsToFit = _items.Count();

            var largestBinByVolume = (_bins.OrderByDescending(x => x.Volume).FirstOrDefault())!;
            if (_items.Sum(x => x.Volume) > largestBinByVolume.Volume)
                return BinFittingOperationResult.CreateFailedResult(BinFitFailedResultReason.TotalVolumeExceeded);

            var itemsNotFittingDueToLongestDimension = _items.Where(x => x.LongestDimension > largestBinByVolume.LongestDimension);
            if (itemsNotFittingDueToLongestDimension.Any())
                return BinFittingOperationResult.CreateFailedResult(BinFitFailedResultReason.ItemDimensionExceeded, this.Convert(itemsNotFittingDueToLongestDimension).ToList());

            _bins = _bins.OrderBy(x => x.Volume);
            _items = _items.OrderByDescending(x => x.Volume);

            foreach (var bin in _bins)
            {
                _availableSpace = new List<VolumetricItem>
                {
                    new VolumetricItem(bin)
                };

                _fittedItems = new List<Item>();

                foreach (var item in _items)
                {
                    if (!this.TryFit(item))
                        break;
                }
                if (_fittedItems.Count == totalItemsToFit)
                {
                    foundBin = bin;
                    break;
                }
            }

            if (foundBin != null)
            {
                return BinFittingOperationResult.CreateSuccessfullResult(this.Convert(foundBin), this.Convert(_fittedItems).ToList());
            }

            return BinFittingOperationResult.CreateFailedResult(BinFitFailedResultReason.DidNotFit, fittedItems: this.Convert(_fittedItems).ToList());
        }


        private Lib.Models.Item Convert(Bin bin)
        {
            return new Lib.Models.Item(bin.ID, bin);
        }

        private IEnumerable<Lib.Models.Item> Convert(IEnumerable<Item> items)
        {
            if (!(items?.Any() ?? false))
                return Enumerable.Empty<Lib.Models.Item>();

            return items.Select(x => new Lib.Models.Item(x.ID, x));
        }

        private bool TryFit(Item item)
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
            return _availableSpace.FirstOrDefault(x => x.Length >= orientation.Length && x.Width >= orientation.Width && x.Height >= orientation.Height);
        }

        private void Fit(VolumetricItem spaceQuadrant, VolumetricItem orientation, Item item)
        {
            var newAvailableSpaces = this.SplitSpaceQuadrant(spaceQuadrant, orientation);
            _availableSpace.Remove(spaceQuadrant);
            if (newAvailableSpaces.Any())
            {
                _availableSpace.AddRange(newAvailableSpaces);
            }
            _fittedItems.Add(item);
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
