using System.Threading.Channels;
using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.Logs.Models;
using Binacle.Net.Kernel.Logs.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.DiagnosticsModule.ExtensionMethods;

internal static class LogProcessorServiceCollectionExtensions
{
	public static void AddOptionsBasedPackingLogProcessor(this IServiceCollection services)
	{
		services.AddLogProcessor<AlgorithmOperationLogChannelRequest, PackingLogEntry>(
			optionsFactory: sp =>
			{
				var options = sp.GetRequiredService<IOptions<PackingLogsConfigurationOptions>>().Value;

				return new LogsProcessorOptions<AlgorithmOperationLogChannelRequest>
				{
					Path = options.Path!,
					FileNameFormat = options.FileName!,
					DateFormat = options.DateFormat!,
					RetentionDays = options.RetentionDays,
				};
			},
			channelFactory: sp =>
			{
				var options = sp.GetRequiredService<IOptions<PackingLogsConfigurationOptions>>().Value;
				if (options.ChannelLimit is > 0)
				{
					return Channel.CreateBounded<AlgorithmOperationLogChannelRequest>(
						new BoundedChannelOptions(options.ChannelLimit.Value)
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
