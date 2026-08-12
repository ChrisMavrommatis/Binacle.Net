using Binacle.Lib.Abstractions;

namespace Binacle.Lib.ResultSelection;

public class SmallestBin_v1 : IResultSelectionStrategy
{
    public OperationResult Select(IDictionary<string, OperationResult> results)
    {
        // Prefer a fully packed result, and among those the smallest bin.
        var fullyPacked = results.Values
            .Where(r => r.Status == OperationResultStatus.FullyPacked)
            .OrderBy(r => r.Bin.Volume)
            .FirstOrDefault();

        if (fullyPacked != null)
            return fullyPacked;

        // Nothing packed fully - smallest bin still wins, ties go to whichever packed the most.
        return results.Values
            .OrderBy(r => r.Bin.Volume)
            .ThenByDescending(r => r.PackedItemsVolumePercentage)
            .First();
    }
}
