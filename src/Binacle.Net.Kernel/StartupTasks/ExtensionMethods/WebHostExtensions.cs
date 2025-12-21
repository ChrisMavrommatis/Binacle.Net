using Binacle.Net.Kernel.StartupTasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Binacle.Net;

public static class WebHostExtensions
{
	public static async Task RunStartupTasksAsync(this IHost host, CancellationToken cancelationToken = default)
	{
		var logger = host.Services.GetRequiredService<ILogger<IStartupTask>>();

		var startupTasks = host.Services.GetServices<IStartupTask>().ToArray();

		logger.LogInformation("Found {StartupTasksNumber} startup tasks", startupTasks.Length);

		foreach (var startupTask in startupTasks)
		{
			var startupTaskName = startupTask.GetType().Name;
			logger.LogInformation("Running {StartupTaskName}...", startupTaskName);
			await startupTask.ExecuteAsync(cancelationToken);
			logger.LogInformation("{StartupTaskName} Completed!", startupTaskName);
		}
	}
}
