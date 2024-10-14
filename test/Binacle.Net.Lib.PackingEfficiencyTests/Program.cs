using Binacle.Net.Lib.PackingEfficiencyTests.Data;
using Binacle.Net.Lib.PackingEfficiencyTests.Data.Providers.PackingEfficiency;
using Binacle.Net.Lib.PackingEfficiencyTests.Services;
using Binacle.Net.TestsKernel.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Binacle.Net.Lib.PackingEfficiencyTests;

internal class Program
{
	static void Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.Enrich.FromLogContext()
			.Enrich.WithMachineName()
			.Enrich.WithThreadId()
			.WriteTo.Console(
				outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}",
				theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code
			)
			.CreateBootstrapLogger();

		var builder = Host.CreateApplicationBuilder();
		builder.Logging.ClearProviders();
		builder.Logging.AddSerilog();

		builder.Services.AddSingleton<BinCollectionsTestDataProvider>(sp =>
		{
			return new BinCollectionsTestDataProvider(solutionRootPath: Constants.SolutionRootBasePath);
		});
		builder.Services.AddSingleton<ORLibraryScenarioTestDataProvider>();
		builder.Services.AddTransient<ITestsRunner, PackingEfficiencyTestsRunner>();

		IHost host = builder.Build();

		var packedBinVolumePercentages = new Dictionary<string, List<decimal>>();
		var maxPotentialPackingEfficiencyPercentages = new List<decimal>();

		using (var scope = host.Services.CreateScope())
		{
			var services = scope.ServiceProvider.GetServices<ITestsRunner>();

			foreach (var service in services)
			{
				service.Run();
			}
		}
	}
}

