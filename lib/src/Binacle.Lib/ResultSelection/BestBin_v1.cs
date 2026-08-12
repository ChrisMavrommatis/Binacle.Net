using Binacle.Lib.Abstractions;

namespace Binacle.Lib.ResultSelection;

public class BestBin_v1 : IResultSelectionStrategy
{
    public OperationResult Select(IDictionary<string, OperationResult> results)
    {
        // Prefer a fully packed result, and among those the bin left with the least room to spare.
        var fullyPacked = results.Values
            .Where(r => r.Status == OperationResultStatus.FullyPacked)
            .OrderByDescending(r => r.PackedBinVolumePercentage)
            .FirstOrDefault();

        if (fullyPacked != null)
            return fullyPacked;

        // Nothing packed fully - same rule, now over the partial results.
        return results.Values
            .OrderByDescending(r => r.PackedBinVolumePercentage)
            .First();
    }
}
