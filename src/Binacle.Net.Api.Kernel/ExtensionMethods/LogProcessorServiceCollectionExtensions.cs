using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.Api.Kernel;

public static class LogProcessorServiceCollectionExtensions
{
	public static IServiceCollection AddUnboundedLogProcessor<THostedService, TChannelRequest>(
		this IServiceCollection services,
		Action<UnboundedChannelOptions>? configureOptions = null
	)
		where THostedService : class, IHostedService
	{
		var channelOptions = new UnboundedChannelOptions();
		configureOptions?.Invoke(channelOptions);
		services.AddSingleton<Channel<TChannelRequest>>(
			_ => Channel.CreateUnbounded<TChannelRequest>(channelOptions)
		);
		
		services.AddHostedService<THostedService>();

		return services;
	}
	
	public static IServiceCollection AddBoundedLogProcessor<THostedService, TChannelRequest>(
		this IServiceCollection services,
		int capacity,
		Action<BoundedChannelOptions>? configureOptions = null
	)
		where THostedService : class, IHostedService
	{
		var channelOptions = new BoundedChannelOptions(capacity);
		configureOptions?.Invoke(channelOptions);
		services.AddSingleton<Channel<TChannelRequest>>(
			_ => Channel.CreateBounded<TChannelRequest>(channelOptions)
		);
		
		services.AddHostedService<THostedService>();

		return services;
	}
}
