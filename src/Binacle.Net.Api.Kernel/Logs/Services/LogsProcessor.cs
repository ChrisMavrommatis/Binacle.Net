using System.Text.Json;
using System.Threading.Channels;
using Binacle.Net.Api.Kernel.Logs.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Binacle.Net.Api.Kernel.Logs.Services;

internal class LogsProcessor<TRequest> : BackgroundService
{
	private readonly Channel<TRequest> channel;
	private readonly IHostEnvironment environment;
	private readonly TimeProvider timeProvider;
	private readonly LogsProcessorOptions<TRequest> options;
	private readonly ILogger<LogsProcessor<TRequest>> logger;

	public LogsProcessor(
		Channel<TRequest> channel,
		IHostEnvironment environment,
		TimeProvider timeProvider,
		LogsProcessorOptions<TRequest> options,
		ILogger<LogsProcessor<TRequest>> logger)
	{
		this.channel = channel;
		this.environment = environment;
		this.timeProvider = timeProvider;
		this.options = options;
		this.logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var logDirectory = Path.Combine(this.environment.ContentRootPath, this.options.Path!);
		this.EnsureDirectoryExists(logDirectory);
		var exceptionsCount = 0;
		this.logger.LogInformation("{Status} logs processor for {LogProcessorRequest}", "Starting",
			typeof(TRequest).Name);

		while (await this.channel.Reader.WaitToReadAsync(stoppingToken).ConfigureAwait(false))
		{
			try
			{
				var request = await this.channel.Reader.ReadAsync(stoppingToken).ConfigureAwait(false);
				var log = this.options.LogFormatter(request);

				var date = this.timeProvider.GetLocalNow();
				var fileName = string.Format(
					this.options.FileNameFormat,
					date.ToString(this.options.DateFormat)
				);
				var logFile = Path.Combine(logDirectory, fileName);
				var json = JsonSerializer.Serialize(log);

				await using (var writer = new StreamWriter(logFile, append: true))
				{
					await writer.WriteLineAsync(json.AsMemory(), stoppingToken).ConfigureAwait(false);
					// Ensure data is written to disk
					await writer.FlushAsync(stoppingToken).ConfigureAwait(false);
				}

				exceptionsCount = 0;
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "An error occured while processing logs for {LogProcessorRequest}",
					typeof(TRequest).Name);
				exceptionsCount++;

				if (exceptionsCount >= this.options.MaxConsecutiveAllowedExceptions)
				{
					this.logger.LogCritical(
						"Too many consecutive exceptions, stopping the processor for {LogProcessorRequest}",
						typeof(TRequest).Name);
					break;
				}
			}
		}

		this.logger.LogInformation("{Status} logs processor for {LogProcessorRequest}", "Stopped",
			typeof(TRequest).Name);
	}

	private void EnsureDirectoryExists(string logDirectory)
	{
		try
		{
			if (!Directory.Exists(logDirectory))
			{
				Directory.CreateDirectory(logDirectory);
			}
		}
		catch (Exception ex)
		{
			this.logger.LogCritical(ex, "Could not create log directory {LogDirectory}", logDirectory);
		}
	}
}
