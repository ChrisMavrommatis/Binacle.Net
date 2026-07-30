using System.Threading.Channels;
using Binacle.Lib.Abstractions.Models;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Net.DiagnosticsModule.Logs.Models;

namespace Binacle.Net.ExtensionMethods;

internal static class LogChannelExtensions
{
	// Single-result convenience: wrap the (key, result) into a one-entry dictionary.
	internal static ValueTask WriteToChannelAsync<TBin, TBox, TParams>(
		this Channel<AlgorithmOperationLogChannelRequest>? logChannel,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		(string Key, OperationResult Result) result,
		ILogger? logger = null
	)
		where TBin : class, IIdentifiableBin
		where TBox : class, IIdentifiableItem
		where TParams : class, ILogParametersProvider
	{
		if (logChannel is null)
		{
			return ValueTask.CompletedTask;
		}

		return logChannel.WriteToChannelAsync(
			bins,
			items,
			parameters,
			new Dictionary<string, OperationResult> { { result.Key, result.Result } },
			logger
		);
	}

	internal static async ValueTask WriteToChannelAsync<TBin, TBox, TParams>(
		this Channel<AlgorithmOperationLogChannelRequest>? logChannel,
		List<TBin> bins,
		List<TBox> items,
		TParams parameters,
		IDictionary<string, OperationResult> results,
		ILogger? logger = null
	)
		where TBin : class, IIdentifiableBin
		where TBox : class, IIdentifiableItem
		where TParams : class, ILogParametersProvider
	{
		// Logging off (null channel) — bail before any work so a disabled feature costs the request nothing.
		if (logChannel is null)
		{
			return;
		}

		using var channelActivity = Diagnostics.ActivitySource.StartActivity("Send Channel Request");

		try
		{
			await logChannel
				.Writer
				.WriteAsync(
					AlgorithmOperationLogChannelRequest.From(bins, items, parameters, results)
				);
		}
		catch (Exception ex)
		{
			if (logger is null)
			{
				return;
			}
			logger.LogError(ex, "Error while writing to channel");
		}
	}
}
