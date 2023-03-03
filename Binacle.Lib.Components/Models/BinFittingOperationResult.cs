namespace Binacle.Lib.Components.Models
{
    public class BinFittingOperationResult
    {
        public BinFitResult Result { get; private set; }
        public Bin FoundBin { get; private set; }
        public List<Item> FittedItems { get; private set; }
        public List<Item> NotFittedItems { get; private set; }
    }
}
