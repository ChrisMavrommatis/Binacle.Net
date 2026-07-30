namespace Binacle.TestsKernel;

public class PercentageComparer : IComparer<decimal>
{
	public int Compare(decimal actual, decimal expected)
	{
		if (PercentagesMatch(expected, actual))
		{
			return 0;
		}
		return actual.CompareTo(expected);
	}
	
	private const decimal TOLERANCE = 0.1m;  // 0.1% tolerance
	public static bool PercentagesMatch(decimal expected, decimal actual)
	{
		return Math.Abs(expected - actual) <= TOLERANCE;
	}
}
