using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Binacle.Lib.Benchmarks.Order;

namespace Binacle.Lib.Benchmarks;

internal class Program
{
	static void Main(string[] args)
	{
		var config = ManualConfig.Create(DefaultConfig.Instance)
			.WithOptions(ConfigOptions.DisableLogFile)
			.WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(50));
		// custom order
		config.Orderer = new AttributeOrderer();

		BenchmarkSwitcher
			.FromAssembly(typeof(Program).Assembly)
			.Run(args, config);
	}
}
