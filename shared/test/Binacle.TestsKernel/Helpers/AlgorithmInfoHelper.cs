
namespace Binacle.TestsKernel.Helpers;

public static class AlgorithmInfoHelper
{
    public static AlgorithmInfo ParseFromCompactString(string compactString)
    {
        // e.g. "FFD_v1"
        var algorithmParts =compactString.Split('_');
        if (algorithmParts.Length != 2)
        {
            throw new ArgumentException(
                $"Invalid algorithm info format. Value {compactString} should have format 'AlgorithmName_vVersion'.");
        }
        if (!Enum.TryParse<Algorithm>(algorithmParts[0], out var parsedAlgorithm))
        {
            throw new ArgumentException(
                $"Invalid algorithm info format. Value {compactString} should have format 'AlgorithmName_vVersion'.");
        }

        if (!algorithmParts[1].StartsWith('v'))
        {
            throw new ArgumentException(
                $"Invalid algorithm info format. Value {compactString} should have format 'AlgorithmName_vVersion'.");
        }

        if (!int.TryParse(algorithmParts[1][1..], out int versionNumber))
        {
            throw new ArgumentException(
                $"Invalid algorithm info format. Value {compactString} should have format 'AlgorithmName_vVersion'.");
        }
			
        return new AlgorithmInfo(parsedAlgorithm, versionNumber);
    }
}
