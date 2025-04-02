using System.Threading.Channels;
using Binacle.Net.Api.DiagnosticsModule.Configuration.Models;
using Binacle.Net.Api.Kernel.Logs.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Api.DiagnosticsModule.ExtensionMethods;

internal static class LogProcessorServiceCollectionExtensions
{
	public static void AddOptionsBasedLogProcessor<TChannelRequest>(
		this IServiceCollection services,
		Func<PackingLogsConfigurationOptions, PackingLogOptions> optionsSelector,
		Func<TChannelRequest, Dictionary<string, object>> logFormatter
	)
	{
		services
			.AddLogProcessor<TChannelRequest>(
				optionsFactory: sp =>
				{
					var options = sp.GetRequiredService<IOptions<PackingLogsConfigurationOptions>>();
					var logOptions = optionsSelector(options.Value);

					return new LogsProcessorOptions<TChannelRequest>()
					{
						Path = logOptions.Path!,
						FileNameFormat = logOptions.FileName!,
						DateFormat = logOptions.DateFormat!,
						LogFormatter = logFormatter,
						MaxConsecutiveAllowedExceptions = 10
					};
				},
				channelFactory: sp =>
				{
					var options = sp.GetRequiredService<IOptions<PackingLogsConfigurationOptions>>();
					var logOptions = optionsSelector(options.Value);
					if (logOptions.ChannelLimit is > 0)
					{
						return Channel.CreateBounded<TChannelRequest>(new BoundedChannelOptions(logOptions.ChannelLimit.Value)
						{
							FullMode = BoundedChannelFullMode.DropWrite,
							SingleReader = true,
							SingleWriter = false,
							AllowSynchronousContinuations = false
						});
					}

					return Channel.CreateUnbounded<TChannelRequest>(new UnboundedChannelOptions
					{
						SingleReader = true,
						SingleWriter = false,
						AllowSynchronousContinuations = false
					});
				});
	}
}
