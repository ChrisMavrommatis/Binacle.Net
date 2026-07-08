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
		var resultsDirectory = RepositoryRoot.Bind().Find("results", "vipaq");
		builder.Services.AddSingleton<IFileWriter>(new MarkdownFileWriter(resultsDirectory));
		builder.Services.AddTransient<TestRunner>();

		// Each report is its own file, and we always run everything.
		var sizeFile = new ResultFile
		{
			Filename = "SizeComparison",
			Title = "ViPaq vs Protobuf — stored size",
			Description = "Raw byte counts and base64 character counts, ViPaq vs protobuf (with proto gzip), "
				+ "with the round-trip gate. Real placed data (offline FFD), split into custom and Bischoff tables. "
				+ "Synthetic random data is not size-measured here — gzip has nothing to grip, so it misreads "
				+ "compression; it is kept only for the BDN speed/memory benchmarks."
		};

		// Real sets: placed results from Binacle.ViPaq.PackedDataGenerator (FFD). Custom and Bischoff are shown as
		// two separate tables in the same file — they are different problem shapes and read more clearly apart.
		builder.Services.AddTransient<ITest, SizeComparisonTest>(serviceProvider =>
			new SizeComparisonTest(
				RealDataProvider.Custom,
				"Real samples — custom packs (offline FFD)",
				"Same rules, on real placed data — small hand-authored packs.",
				sizeFile,
				serviceProvider.GetRequiredService<ILogger<SizeComparisonTest>>()));

		builder.Services.AddTransient<ITest, SizeComparisonTest>(serviceProvider =>
			new SizeComparisonTest(
				RealDataProvider.Bischoff,
				"Real samples — Bischoff suite (offline FFD)",
				"Same rules, on real placed data. Structured results, so compression actually pays.",
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
