using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Binacle.Lib.Benchmarks.Order;

namespace Binacle.Lib.Benchmarks;

internal class Program
{
	static void Main(string[] args)
	{
		// Start from the defaults but export only the GitHub markdown report — no csv/html clutter to curate.
		var defaults = DefaultConfig.Instance;
		var config = ManualConfig.CreateEmpty()
			.AddColumnProvider(defaults.GetColumnProviders().ToArray())
			.AddLogger(defaults.GetLoggers().ToArray())
			.AddAnalyser(defaults.GetAnalysers().ToArray())
			.AddValidator(defaults.GetValidators().ToArray())
			.AddExporter(MarkdownExporter.GitHub)
			.WithOptions(ConfigOptions.DisableLogFile)
			.WithBuildTimeout(TimeSpan.FromMinutes(20))
			.WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(50));
		// custom order
		config.Orderer = new AttributeOrderer();
		// Pin BDN output next to the project, so reports land in the same place no matter where you launch from.
		config.ArtifactsPath = Path.GetFullPath(
			Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "BenchmarkDotNet.Artifacts"));

		BenchmarkSwitcher
			.FromAssembly(typeof(Program).Assembly)
			.Run(args, config);
	}
}
