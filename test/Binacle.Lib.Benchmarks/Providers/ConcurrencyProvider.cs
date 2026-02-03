namespace Binacle.Lib.Benchmarks.Providers;

public static class ConcurrencyProvider
{
	public static int[] GetProcessorCount() =>
		[Environment.ProcessorCount];
}
