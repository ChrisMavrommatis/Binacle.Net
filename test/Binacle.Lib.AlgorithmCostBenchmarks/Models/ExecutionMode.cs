namespace Binacle.Lib.AlgorithmCostBenchmarks.Models;

public enum ExecutionMode
{
	Loop,
	Parallel
}

public enum BinSize
{
	Small,
	Medium,
	Large,
	XLarge
}

public enum ItemSizeDistribution
{
	Uniform,
	Mixed,
	MostlySmall,
	MostlyLarge
}
