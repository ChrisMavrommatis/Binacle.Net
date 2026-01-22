using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Binacle.Lib.AlgorithmCostBenchmarks;

internal class Program
{
	static void Main(string[] args)
	{
		var config = ManualConfig.Create(DefaultConfig.Instance)
			.WithOptions(ConfigOptions.DisableLogFile);

		// custom order
		// config.Orderer = new AttributeOrderer();

		BenchmarkSwitcher
			.FromAssembly(typeof(Program).Assembly)
			.Run(args, config);
	}
}
