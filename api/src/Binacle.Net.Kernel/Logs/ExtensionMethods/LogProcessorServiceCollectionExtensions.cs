using System.Threading.Channels;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Net.Kernel.Logs.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net;

public static class LogProcessorServiceCollectionExtensions
{
	public static IServiceCollection AddLogProcessor<TChannelRequest, TLog>(
		this IServiceCollection services,
		Func<IServiceProvider, LogsProcessorOptions<TChannelRequest>> optionsFactory,
		Func<IServiceProvider, Channel<TChannelRequest>> channelFactory
	)
		where TChannelRequest : ILogEntryConvertible<TLog>
	{
		services.AddSingleton<Channel<TChannelRequest>>(channelFactory);
		services.AddSingleton<LogsProcessorOptions<TChannelRequest>>(optionsFactory);
		services.AddHostedService<LogsProcessor<TChannelRequest, TLog>>();
		// Retention runs as a separate hosted service so pruning never shares the processor's drain loop.
		services.AddHostedService<LogsRetentionProcessor<TChannelRequest>>();
		return services;
	}
}
