using Binacle.Lib.Abstractions;

namespace Binacle.Lib.ResultSelection;

public class SmallestBin_v2 : IResultSelectionStrategy
{
    public OperationResult Select(IDictionary<string, OperationResult> results)
    {
        if (results.Count == 0)
            throw new ArgumentException("No results provided for selection.");

        if (results.Count == 1)
            return results.Values.First();
        
        OperationResult? best = null;
        int bestVolume = -1;
        double bestTiebreaker = -1;
        
        foreach(var (_, result) in results)
        {
            bool isFullyPacked = result.Status == OperationResultStatus.FullyPacked;
            bool bestIsFullyPacked = best?.Status == OperationResultStatus.FullyPacked;
            int volume = result.Bin.Volume;
            double tiebreaker = (double)result.PackedItemsVolumePercentage;

            var isBetter =
                best == null
                || (!bestIsFullyPacked && isFullyPacked)
                || (
                    isFullyPacked == bestIsFullyPacked && (
                        volume < bestVolume ||
                        (volume == bestVolume && tiebreaker > bestTiebreaker)
                    )
                );
            
            if (isBetter)
            {
                best = result;
                bestVolume = volume;
                bestTiebreaker = tiebreaker;
            }
        }

        return best!;
    }
}