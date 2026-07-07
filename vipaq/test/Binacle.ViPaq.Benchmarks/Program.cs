using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Binacle.ViPaq.Benchmarks;

internal class Program
{
	static void Main(string[] args)
	{
		var config = ManualConfig.Create(DefaultConfig.Instance)
			.WithOptions(ConfigOptions.DisableLogFile)
			.WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(40));

		// Each benchmark class runs on its own with --filter, e.g. --filter *EncodeBenchmarks*.
		BenchmarkSwitcher
			.FromAssembly(typeof(Program).Assembly)
			.Run(args, config);
	}
}
