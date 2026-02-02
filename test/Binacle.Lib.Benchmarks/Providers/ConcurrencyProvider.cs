namespace Binacle.Lib.Benchmarks.Providers;

public static class ConcurrencyProvider
{
	public static int[] GetConcurrencyLevels()
		=> [Environment.ProcessorCount];
}
