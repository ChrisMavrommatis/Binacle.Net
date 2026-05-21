using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.ResultSelection;

public class SmallestBin_v1 : IResultSelectionStrategy
{
    public OperationResult Select(IDictionary<string, OperationResult> results)
    {
        // best algorithm for this bin
        var fullyPacked = results.Values
            .Where(r => r.Status == OperationResultStatus.FullyPacked)
            .OrderBy(r => r.Bin.Volume)
            .FirstOrDefault();

        if (fullyPacked != null)
            return fullyPacked;

        // fallback: most of the order packed
        return results.Values
            .OrderBy(r => r.Bin.Volume)
            .ThenByDescending(r => r.PackedItemsVolumePercentage)
            .First();
    }
}
