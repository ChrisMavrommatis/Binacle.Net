using Binacle.ViPaq.PerformanceTests.PreReportChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.ViPaq.PerformanceTests.ExtensionMethods;

public static class PreReportCheckExtensions
{
	// The fail-fast gates that run before the reports (see IPreReportCheck). Pair with RunPreReportChecks: register
	// them on the builder, then run them from the built provider before the report TestRunner, so a broken premise
	// stops the run rather than skewing a table.
	public static IServiceCollection AddPreReportChecks(this IServiceCollection services)
	{
		services.AddTransient<IPreReportCheck, CuratedPicksCheck>();
		services.AddTransient<IPreReportCheck, ReportPathRoundTripCheck>();
		services.AddTransient<IPreReportCheck, ForcedWidthRoundTripCheck>();
		return services;
	}

	// Runs every registered gate, in registration order. Throws on the first failure.
	public static void RunPreReportChecks(this IServiceProvider serviceProvider)
	{
		foreach (var check in serviceProvider.GetServices<IPreReportCheck>())
		{
			check.Run();
		}
	}
}
