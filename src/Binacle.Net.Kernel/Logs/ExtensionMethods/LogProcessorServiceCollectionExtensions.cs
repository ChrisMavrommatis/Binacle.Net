using System.Threading.Channels;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Net.Kernel.Logs.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net;

public static class LogProcessorServiceCollectionExtensions
{
	public static IServiceCollection AddLogProcessor<TChannelRequest>(
		this IServiceCollection services,
		Func<IServiceProvider, LogsProcessorOptions<TChannelRequest>> optionsFactory,
		Func<IServiceProvider, Channel<TChannelRequest>> channelFactory
	)
	{
		services.AddSingleton<Channel<TChannelRequest>>(channelFactory);
		services.AddSingleton<LogsProcessorOptions<TChannelRequest>>(optionsFactory);
		services.AddHostedService<LogsProcessor<TChannelRequest>>();
		return services;
	}
}
