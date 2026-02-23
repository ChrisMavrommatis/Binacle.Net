using System.Threading.Channels;
using Binacle.Lib.Abstractions.Models;
using Binacle.Net.Kernel.Logs.Models;

namespace Binacle.Net.ExtensionMethods;

internal static class LogChannelExtensions
{
    internal static async ValueTask WriteToChannelAsync<TBin, TBox, TParams>(
        this Channel<AlgorithmOperationLogChannelRequest>? logChannel,
        List<TBin> bins,
        List<TBox> items,
        TParams parameters,
        IDictionary<string, OperationResult> results,
        ILogger? logger = null
    )
        where TBin : class, IWithID, IWithReadOnlyDimensions
        where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
        where TParams : class, ILogConvertible
    {
        using var channelActivity = Diagnostics.ActivitySource.StartActivity("Send Channel Request");

        if (logChannel is null)
        {
            return;
        }

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
            if(logger is null)
			{
				return;
			}
            logger.LogError(ex, "Error while writing to channel");
        }
    }
}
