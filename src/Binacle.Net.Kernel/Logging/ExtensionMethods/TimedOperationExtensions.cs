using Binacle.Net.Kernel.Logging;
using Microsoft.Extensions.Logging;

namespace Binacle.Net;

public static class TimedOperationExtensions
{
	public static TimedOperation BeginTimedOperation(
		this ILogger logger, string messageTemplate, params object[] args)
	{
		return BeginTimedOperation(logger, LogLevel.Information, messageTemplate, args);
	}

	public static TimedOperation BeginTimedOperation(
		this ILogger logger, LogLevel logLevel, string messageTemplate, params object[] args)
	{
		return new TimedOperation(logger, logLevel, messageTemplate, args);
	}
}
