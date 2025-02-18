using System.Threading.Channels;
using Binacle.Net.Api.Kernel.Models;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.Api.DiagnosticsModule.Services;

public class FittingLogsProcessor  : BackgroundService
{
	private readonly Channel<FittingLogChannelRequest> channel;
	
	public FittingLogsProcessor(Channel<FittingLogChannelRequest> channel)
	{
		this.channel = channel;
	}
	
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while(await this.channel.Reader.WaitToReadAsync(stoppingToken))
		{
			var request = await this.channel.Reader.ReadAsync(stoppingToken);
			foreach (var result in request.Results)
			{
				Console.WriteLine($"Fitting result for bin {result.Key}: {result.Value}");
			}
		}
	}
}
internal class PackingLogsProcessor : BackgroundService
{
	private readonly Channel<PackingLogChannelRequest> channel;

	public PackingLogsProcessor(Channel<PackingLogChannelRequest> channel)
	{
		this.channel = channel;
	}
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (await this.channel.Reader.WaitToReadAsync(stoppingToken))
		{
			var request = await this.channel.Reader.ReadAsync(stoppingToken);
			foreach (var result in request.Results)
			{
				Console.WriteLine($"Packing result for bin {result.Key}: {result.Value}");
			}
			
		}
	}
}
