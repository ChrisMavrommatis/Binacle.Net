using Binacle.Net.Kernel.Logs.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Binacle.Net.Kernel.Logs.Services;

// Sibling to LogsProcessor with its own loop — it never touches the write channel, so a slow sweep can never stall
// logging. Deletes local log files past their retention age. When RetentionDays is null it does nothing: keeping
// files is the safe default because these logs are an archive, and deletion stays the operator's job until opted in.
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
		// Turn the filename format ("{0}.ndjson") into a glob ("*.ndjson") so we only ever match our own files.
		var searchPattern = string.Format(this.options.FileNameFormat, "*");
		var retention = TimeSpan.FromDays(this.options.RetentionDays.Value);

		// Day granularity is enough — files roll once a day. Sweep on start, then once a day.
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
