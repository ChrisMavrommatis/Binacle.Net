using System.Text.Json;
using System.Threading.Channels;
using Binacle.Net.Api.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Binacle.Net.Api.Services;

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
				this.logger.LogError(ex, "An error occured while processing logs");
				exceptionsCount++;

				if (exceptionsCount >= this.options.MaxConsecutiveAllowedExceptions)
				{
					this.logger.LogCritical("Too many consecutive exceptions, stopping the processor");
					break;
				}
			}
		}
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
			this.logger.LogCritical(ex, "Could not create log directory");
		}
	}
}
