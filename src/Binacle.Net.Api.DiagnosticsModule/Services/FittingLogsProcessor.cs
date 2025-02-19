using System.Threading.Channels;
using Binacle.Net.Api.DiagnosticsModule.ExtensionMethods;
using Binacle.Net.Api.Kernel.Models;
using Microsoft.AspNetCore.Hosting;

namespace Binacle.Net.Api.DiagnosticsModule.Services;

internal class LegacyFittingLogsProcessor  : LogsProcessorBase<LegacyFittingLogChannelRequest>
{

	public LegacyFittingLogsProcessor(
		Channel<LegacyFittingLogChannelRequest> channel,
		IWebHostEnvironment environment,
		TimeProvider timeProvider
		) : base(channel, environment, timeProvider, "fitting_legacy")
	{
	}

	protected override Dictionary<string, object> ConvertToLogFormat(LegacyFittingLogChannelRequest request)
	{
		var log = new Dictionary<string, object>();
		log.Add("Bins", request.Bins.ConvertToLogFormat());
		log.Add("Items", request.Items.ConvertToLogFormat());
		log.Add("Results", request.Results.ConvertToLogFormat());
		return log;
	}
}

internal class LegacyPackingLogsProcessor : LogsProcessorBase<LegacyPackingLogChannelRequest>
{
	public LegacyPackingLogsProcessor(	
		Channel<LegacyPackingLogChannelRequest> channel,
		IWebHostEnvironment environment,
		TimeProvider timeProvider
	) : base(channel, environment, timeProvider, "fitting_packing")
	{
	}
	
	protected override Dictionary<string, object> ConvertToLogFormat(LegacyPackingLogChannelRequest request)
	{
		var log = new Dictionary<string, object>();
			
		log.Add("Bins", request.Bins.ConvertToLogFormat());
		log.Add("Items", request.Items.ConvertToLogFormat());
		log.Add("Results", request.Results.ConvertToLogFormat());
		return log;
	}
}

internal class PackingLogsProcessor : LogsProcessorBase<PackingLogChannelRequest>
{
	public PackingLogsProcessor(	
		Channel<PackingLogChannelRequest> channel,
		IWebHostEnvironment environment,
		TimeProvider timeProvider
	) : base(channel, environment, timeProvider, "packing")
	{
	}
	
	protected override Dictionary<string, object> ConvertToLogFormat(PackingLogChannelRequest request)
	{
		var log = new Dictionary<string, object>();
			
		log.Add("Bins", request.Bins.ConvertToLogFormat());
		log.Add("Items", request.Items.ConvertToLogFormat());
		log.Add("Results", request.Results.ConvertToLogFormat());
		return log;
	}
}
