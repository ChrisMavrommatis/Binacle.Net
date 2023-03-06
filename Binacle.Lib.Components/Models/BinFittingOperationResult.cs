namespace Binacle.Lib.Components.Models
{
    public class BinFittingOperationResult
    {
        private BinFittingOperationResult()
        {
        }


        public BinFitResultStatus Status { get; private set; }
        public BinFitFailedResultReason? Reason { get; private set; }
        public Bin FoundBin { get; private set; }
        public List<Item> FittedItems { get; private set; }

        public static BinFittingOperationResult CreateFailedResult(BinFitFailedResultReason? reason = null, List<Item>? fittedItems = null, List<Item>? notFittedItems = null)
        {
            return new BinFittingOperationResult()
            {
                Status = BinFitResultStatus.Fail,
                Reason = reason.HasValue ? reason.Value : BinFitFailedResultReason.Unspecified,
                FittedItems = (fittedItems?.Any() ?? false) ? fittedItems : new List<Item>()
            };
        }

        public static BinFittingOperationResult CreateSuccessfullResult(Bin foundBin, List<Item> fittedItems)
        {
            if (foundBin == null)
                throw new ArgumentNullException(nameof(foundBin));

            if(!(fittedItems?.Any() ?? false))
                throw new ArgumentNullException(nameof(fittedItems));


            return new BinFittingOperationResult()
            {
                Status = BinFitResultStatus.Success,
                FoundBin = foundBin,
                FittedItems = (fittedItems?.Any() ?? false) ? fittedItems : new List<Item>(),
            };
        }
    }
}
