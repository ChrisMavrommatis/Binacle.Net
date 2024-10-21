using Binacle.Net.Lib.PerformanceTests.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Binacle.Net.Lib.PerformanceTests.Services;

internal class TestRunner
{
	private readonly IServiceProvider serviceProvider;
	private readonly ILogger<TestRunner> logger;

	public TestRunner(
		IServiceProvider serviceProvider,
		ILogger<TestRunner> logger
		)
	{
		this.serviceProvider = serviceProvider;
		this.logger = logger;
	}

	public async Task RunAsync()
	{
		var tests = this.serviceProvider.GetServices<ITest>();
		var tasks = new List<Task<List<TestSummaryAction>>>();

		foreach (var test in tests)
		{
			tasks.Add(Task.Run(() => test.Run()));

		}
		var testSummaries = await Task.WhenAll(tasks);

		this.logger.LogInformation("=========================================================");
		foreach (var summaries in testSummaries)
		{
			foreach (var summary in summaries)
			{
				this.logger.LogInformation("=========================================================");
				this.logger.LogInformation(summary.Name);
				this.logger.LogInformation("---------------------------------------------------------");

				summary.Action();
				this.logger.LogInformation("=========================================================");
			}
		}
	}
}
