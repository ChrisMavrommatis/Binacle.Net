using Binacle.Lib.Abstractions;

namespace Binacle.Lib.ResultSelection;

public class BestBin_v2 : IResultSelectionStrategy
{
    public OperationResult Select(IDictionary<string, OperationResult> results)
    {
        if (results.Count == 0)
            throw new ArgumentException("No results provided for selection.");

        if (results.Count == 1)
            return results.Values.First();

        OperationResult? best = null;
        double bestScore = -1;

        foreach (var (_, result) in results)
        {
            double score = 0;
            if (result.Status == OperationResultStatus.FullyPacked)
            {
                score += 1000;
            }

            score += (double)result.PackedBinVolumePercentage;

            if (score > bestScore)
            {
                bestScore = score;
                best = result;
            }
        }

        return best!;
    }
}