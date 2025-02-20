using System.Threading.Channels;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.Api;

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
