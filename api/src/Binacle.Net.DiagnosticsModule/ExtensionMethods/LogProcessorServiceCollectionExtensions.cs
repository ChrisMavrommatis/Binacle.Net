using System.Threading.Channels;
using Binacle.Net;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.Logs.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.DiagnosticsModule.ExtensionMethods;

internal static class LogProcessorServiceCollectionExtensions
{
	// Reads the packing-logs config and registers the generic processor for the packing request/entry. Fit and pack
	// both flow through the one channel; which log file they land in is chosen by optionsSelector.
	public static void AddOptionsBasedPackingLogProcessor(
		this IServiceCollection services,
		Func<PackingLogsConfigurationOptions, PackingLogOptions> optionsSelector
	)
	{
		services.AddLogProcessor<AlgorithmOperationLogChannelRequest, PackingLogEntry>(
			optionsFactory: sp =>
			{
				var options = sp.GetRequiredService<IOptions<PackingLogsConfigurationOptions>>();
				var logOptions = optionsSelector(options.Value);

				return new LogsProcessorOptions<AlgorithmOperationLogChannelRequest>
				{
					Path = logOptions.Path!,
					FileNameFormat = logOptions.FileName!,
					DateFormat = logOptions.DateFormat!,
				};
			},
			channelFactory: sp =>
			{
				var options = sp.GetRequiredService<IOptions<PackingLogsConfigurationOptions>>();
				var logOptions = optionsSelector(options.Value);
				if (logOptions.ChannelLimit is > 0)
				{
					return Channel.CreateBounded<AlgorithmOperationLogChannelRequest>(
						new BoundedChannelOptions(logOptions.ChannelLimit.Value)
						{
							FullMode = BoundedChannelFullMode.DropWrite,
							SingleReader = true,
							SingleWriter = false,
							AllowSynchronousContinuations = false
						});
				}

				return Channel.CreateUnbounded<AlgorithmOperationLogChannelRequest>(
					new UnboundedChannelOptions
					{
						SingleReader = true,
						SingleWriter = false,
						AllowSynchronousContinuations = false
					});
			});
	}
}
