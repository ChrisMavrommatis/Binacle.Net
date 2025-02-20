using System.Text.Json;
using System.Threading.Channels;
using Binacle.Net.Api.Models;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.Api.Services;

internal class LogsProcessor<TRequest> : BackgroundService
{
	private readonly Channel<TRequest> channel;
	private readonly IHostEnvironment environment;
	private readonly TimeProvider timeProvider;
	private readonly LogsProcessorOptions<TRequest> options;

	public LogsProcessor(
		Channel<TRequest> channel,
		IHostEnvironment environment,
		TimeProvider timeProvider,
		LogsProcessorOptions<TRequest> options
	)
	{
		this.channel = channel;
		this.environment = environment;
		this.timeProvider = timeProvider;
		this.options = options;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var logDirectory = Path.Combine(this.environment.ContentRootPath, this.options.Path!);
		if (!Directory.Exists(logDirectory))
		{
			Directory.CreateDirectory(logDirectory);
		}
		while (await this.channel.Reader.WaitToReadAsync(stoppingToken).ConfigureAwait(false))
		{
			var request = await this.channel.Reader.ReadAsync(stoppingToken).ConfigureAwait(false);
			var log = this.options.LogFormatter(request);

			var date = this.timeProvider.GetLocalNow();
			var fileName = string.Format(this.options.FileNameFormat, date.ToString("yyyyMMdd"));
			var logFile = Path.Combine(logDirectory, fileName);
			var json = JsonSerializer.Serialize(log);

			using (var writer = new StreamWriter(logFile, append: true))
			{
				await writer.WriteLineAsync(json).ConfigureAwait(false);
			}

			// Wait for a short period to avoid constantly reopening the file
			await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
		}
	}
}
