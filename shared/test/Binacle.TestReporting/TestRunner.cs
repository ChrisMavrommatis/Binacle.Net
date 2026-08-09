using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Binacle.TestReporting;

// Runs every registered ITest, groups the results by their file, logs each to the console, and hands each
// file's results to every registered IFileWriter. Register the tests and writers in DI; this ties them
// together.
public class TestRunner
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

		var testResultList = (await Task.WhenAll(tasks)).GroupBy(result => result.File);

		foreach (var testResultGroup in testResultList)
		{
			var file = testResultGroup.Key;
			var results = testResultGroup.ToArray();
			foreach (var result in results)
			{
				this.logger.LogInformation("{TestResult}", result.ConsolePrint());
			}

			foreach (var fileWriter in fileWriters)
			{
				await fileWriter.WriteAsync(file, results);
			}
		}
	}
}
