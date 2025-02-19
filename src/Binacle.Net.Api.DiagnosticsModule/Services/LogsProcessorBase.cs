using System.Text.Json;
using System.Threading.Channels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.Api.DiagnosticsModule.Services;

internal abstract class LogsProcessorBase<TRequest> : BackgroundService
{
	private readonly Channel<TRequest> channel;
	private readonly IWebHostEnvironment environment;
	private readonly TimeProvider timeProvider;
	private readonly string logFilePrefix;

	public LogsProcessorBase(
		Channel<TRequest> channel,
		IWebHostEnvironment environment,
		TimeProvider timeProvider,
		string logFilePrefix
	)
	{
		this.channel = channel;
		this.environment = environment;
		this.timeProvider = timeProvider;
		this.logFilePrefix = logFilePrefix;
	}
	
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var logDirectory = Path.Combine(this.environment.ContentRootPath, "data", "logs");
		if (!Directory.Exists(logDirectory))
		{
			Directory.CreateDirectory(logDirectory);
		}
		
		while(await this.channel.Reader.WaitToReadAsync(stoppingToken).ConfigureAwait(false))
		{
			var request = await this.channel.Reader.ReadAsync(stoppingToken).ConfigureAwait(false);
			var log = this.ConvertToLogFormat(request);
			
			var date = this.timeProvider.GetLocalNow();
			var logFile = Path.Combine(logDirectory, $"{logFilePrefix}_{date:yyyyMMdd}.ndjson");
			var json = JsonSerializer.Serialize(log);
			using(var writer = new StreamWriter(logFile, append: true))
			{
				await writer.WriteLineAsync(json).ConfigureAwait(false);
			}
			
			// Wait for a short period to avoid constantly reopening the file
			await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
		}
	}
	
	protected abstract Dictionary<string, object> ConvertToLogFormat(TRequest request);
}
