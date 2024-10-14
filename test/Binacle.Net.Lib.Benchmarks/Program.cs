using BenchmarkDotNet.Running;

namespace Binacle.Net.Lib.Benchmarks;

internal class Program
{
	static void Main(string[] args)
	{
		// Get Type of Benchmark
		var benchmark = Environment.GetEnvironmentVariable("RUN_BENCHMARKS");
		if (benchmark == null)
		{
			Console.WriteLine("No benchmark specified. Exiting...");
			return;
		}

		// Run Benchmark
		if (benchmark == "FittingScalingBenchmarks")
		{
			BenchmarkRunner.Run<FittingScalingBenchmarks>();
		}
		else if (benchmark == "PackingScalingBenchmarks")
		{
			BenchmarkRunner.Run<PackingScalingBenchmarks>();
		}
		else
		{
			Console.WriteLine("Invalid benchmark specified. Exiting...");
			return;
		}
	}
}
