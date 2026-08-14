using Binacle.Lib.Abstractions;

namespace Binacle.Lib.ResultSelection;

public class BestAlgorithm_v1 : IResultSelectionStrategy
{
    public OperationResult Select(IDictionary<string, OperationResult> results)
    {
        // Any fully packed result wins outright, so the first one found is good enough.
        var fullyPacked = results.Values
            .FirstOrDefault(r => r.Status == OperationResultStatus.FullyPacked);

        if (fullyPacked != null)
            return fullyPacked;

        // Nothing packed fully - take whichever algorithm got the most of the order in.
        return results.Values
            .OrderByDescending(r => r.PackedItemsVolumePercentage)
            .First();
    }
}
