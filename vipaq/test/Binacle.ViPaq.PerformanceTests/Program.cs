using Binacle.TestReporting;
using Binacle.ViPaq.PerformanceTests.Tests;
using Binacle.ViPaq.TestsKernel.Samples;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Binacle.ViPaq.PerformanceTests;

internal class Program
{
	static async Task Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.Enrich.FromLogContext()
			.Enrich.WithMachineName()
			.Enrich.WithThreadId()
			.WriteTo.Console(
				outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}",
				theme: AnsiConsoleTheme.Code
			)
			.CreateBootstrapLogger();

		var builder = Host.CreateApplicationBuilder();
		builder.Logging.ClearProviders();
		builder.Logging.AddSerilog();

		// Reports land in the repo's results/vipaq folder, next to the committed baselines.
		var resultsDirectory = Path.Combine(RepositoryRoot.Find(), "results", "vipaq");
		builder.Services.AddSingleton<IFileWriter>(new MarkdownFileWriter(resultsDirectory));
		builder.Services.AddTransient<TestRunner>();

		// Each report is its own file, and we always run everything.
		var sizeFile = new ResultFile
		{
			Filename = "SizeComparison",
			Title = "ViPaq vs Protobuf — stored size",
			Description = "Raw byte counts and base64 character counts, ViPaq vs protobuf (with proto gzip), "
				+ "with the round-trip gate. Two sample sets: synthetic (generated) and real (packed via the API)."
		};

		// Synthetic set: the generated matrix.
		builder.Services.AddTransient<ITest, SizeComparisonTest>(serviceProvider =>
			new SizeComparisonTest(
				SampleProvider.All,
				"Synthetic samples — generated matrix",
				"Lower ViPaq/Proto is better. Protobuf is compressed only when ViPaq is, so the ratio "
					+ "compares like against like. Generated random values, so gzip has little to grip.",
				sizeFile,
				serviceProvider.GetRequiredService<ILogger<SizeComparisonTest>>()));

		// Real set: results packed via the API (custom + Bischoff). 'vs API' cross-checks our re-encode.
		builder.Services.AddTransient<ITest, SizeComparisonTest>(serviceProvider =>
			new SizeComparisonTest(
				RealDataProvider.All,
				"Real samples — packed via the API (custom + Bischoff)",
				"Same rules, on real placed data. Structured results, so compression actually pays. "
					+ "'vs API' compares our re-encode to the token the API emitted (validation only).",
				sizeFile,
				serviceProvider.GetRequiredService<ILogger<SizeComparisonTest>>()));

		var crossoverFile = new ResultFile
		{
			Filename = "CompressionCrossover",
			Title = "Compression crossover",
			Description = "PROVISIONAL — may be phased out. Where compressing the token starts to beat the raw "
				+ "token, by item count. Swept over the real samples (packed via the API), one width family at "
				+ "a time. Kept for now; the size report already shows where compression starts to pay, and the "
				+ "real data has gaps so the crossover point is coarse."
		};

		// Sweep the real data, split by width family so each run is like-for-like: 8-bit (custom packs)
		// and 16-bit (Bischoff packs).
		var realFamilies = new[]
		{
			(bits: 8, label: "8-bit real (custom packs)"),
			(bits: 16, label: "16-bit real (Bischoff packs)"),
		};

		foreach (var (bits, label) in realFamilies)
		{
			var family = RealDataProvider.All
				.Where(sample => sample.WidthBits == bits)
				.ToList();

			if (family.Count == 0)
			{
				continue;
			}

			builder.Services.AddTransient<ITest, CompressionCrossoverTest>(serviceProvider =>
				new CompressionCrossoverTest(
					family,
					label,
					crossoverFile,
					serviceProvider.GetRequiredService<ILogger<CompressionCrossoverTest>>()));
		}

		IHost host = builder.Build();

		using var scope = host.Services.CreateScope();
		var testRunner = scope.ServiceProvider.GetRequiredService<TestRunner>();
		await testRunner.RunAsync();
	}
}
