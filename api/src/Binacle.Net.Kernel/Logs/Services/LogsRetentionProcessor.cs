using Binacle.Net.Kernel.Logs.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Binacle.Net.Kernel.Logs.Services;

// Deletes local log files past their retention age. Its own loop, never touching the write channel, so a slow
// sweep cannot stall logging. With RetentionDays null it does nothing: these logs are an archive, so keeping
// them is the safe default and deletion is opt-in.
internal class LogsRetentionProcessor<TRequest> : BackgroundService
{
	private readonly IHostEnvironment environment;
	private readonly TimeProvider timeProvider;
	private readonly LogsProcessorOptions<TRequest> options;
	private readonly ILogger<LogsRetentionProcessor<TRequest>> logger;

	public LogsRetentionProcessor(
		IHostEnvironment environment,
		TimeProvider timeProvider,
		LogsProcessorOptions<TRequest> options,
		ILogger<LogsRetentionProcessor<TRequest>> logger)
	{
		this.environment = environment;
		this.timeProvider = timeProvider;
		this.options = options;
		this.logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (this.options.RetentionDays is null)
		{
			this.logger.LogInformation(
				"Retention disabled for {LogProcessorRequest}; files are kept until removed externally",
				typeof(TRequest).Name);
			return;
		}

		var logDirectory = Path.Combine(this.environment.ContentRootPath, this.options.Path);
		// The filename format ("{0}.ndjson") becomes a glob ("*.ndjson"), so only our own files ever match.
		var searchPattern = string.Format(this.options.FileNameFormat, "*");
		var retention = TimeSpan.FromDays(this.options.RetentionDays.Value);

		// Files roll once a day, so sweeping on start and then daily is enough.
		using var timer = new PeriodicTimer(TimeSpan.FromDays(1));
		try
		{
			do
			{
				this.DeleteExpiredFiles(logDirectory, searchPattern, retention);
			} while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false));
		}
		catch (OperationCanceledException)
		{
			// Cancellation here means shutdown, not a fault.
		}
	}

	private void DeleteExpiredFiles(string logDirectory, string searchPattern, TimeSpan retention)
	{
		if (!Directory.Exists(logDirectory))
		{
			return;
		}

		var cutoff = this.timeProvider.GetUtcNow().UtcDateTime - retention;
		foreach (var file in Directory.EnumerateFiles(logDirectory, searchPattern))
		{
			try
			{
				if (File.GetLastWriteTimeUtc(file) < cutoff)
				{
					File.Delete(file);
					this.logger.LogInformation("Deleted expired log file {LogFile}", file);
				}
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Could not delete expired log file {LogFile}", file);
			}
		}
	}
}
