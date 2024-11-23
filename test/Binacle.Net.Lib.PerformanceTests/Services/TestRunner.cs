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
		var tasks = new TaskList<TestResultList>();

		foreach (var test in tests)
		{
			tasks.Add(Task.Run(() => test.Run()));
		}
		var testResultLists = await Task.WhenAll(tasks);

		foreach (var testResultList in testResultLists)
		{
			foreach (var testResult in testResultList)
			{
				var text = testResult.ConsolePrint();
				this.logger.LogInformation(text);
			}
		}
	}
}
