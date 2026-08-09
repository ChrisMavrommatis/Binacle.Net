using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Binacle.Net.Kernel.Logging;

public class TimedActivityOperation : TimedOperation
{
	private readonly Activity? activity;

	public TimedActivityOperation(ILogger logger, LogLevel logLevel, string message, ActivitySource activitySource)
		:base(logger, logLevel, message, null)
	{
		this.activity = activitySource?.StartActivity(message);
	}

	public override void Dispose()
	{
		base.Dispose();
		this.activity?.Dispose();
		GC.SuppressFinalize(this);
	}
}
