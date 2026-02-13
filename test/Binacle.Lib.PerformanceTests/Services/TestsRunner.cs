using Binacle.Lib.PerformanceTests.Models;
using Binacle.Lib.PerformanceTests.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Binacle.Lib.PerformanceTests.Services;

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
		var fileWriters = this.serviceProvider.GetServices<IFileWriter>().ToArray();
		var tasks = new TaskList<TestResult>();

		foreach (var test in tests)
		{
			tasks.Add(Task.Run(() => test.Run()));
		}

		var testResultList = (await Task.WhenAll(tasks)).GroupBy(x => x.File);

		foreach (var testResultGroup in testResultList)
		{
			var file = testResultGroup.Key;
			var results = testResultGroup.ToArray();
			foreach (var result in results)
			{
				this.logger.LogInformation(result.ConsolePrint());
			}

			foreach (var fileWriter in fileWriters)
			{
				await fileWriter.WriteAsync(file, results);
			}
		}
	}
}
