using System.Diagnostics;
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
	
	public static TimedActivityOperation BeginTimedActivityOperation(
		this ILogger logger, string message)
	{
		return BeginTimedActivityOperation(logger, LogLevel.Information, message, Binacle.Net.Diagnostics.ActivitySource);
	}
	
	public static TimedActivityOperation BeginTimedActivityOperation(
		this ILogger logger, string message, ActivitySource activitySource)
	{
		return BeginTimedActivityOperation(logger, LogLevel.Information, message, activitySource);
	}
	
	public static TimedActivityOperation BeginTimedActivityOperation(
		this ILogger logger, LogLevel logLevel, string message,  ActivitySource activitySource)
	{
		return new TimedActivityOperation(logger, logLevel, message, activitySource);
	}
}
