using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Binacle.Net.Kernel.Logging;

public class TimedOperation : IDisposable
{
	private readonly ILogger logger;
	private readonly LogLevel logLevel;
	private readonly string messageTemplate;
	private readonly object[] args;
	private readonly long startingTimestamp;

	public TimedOperation(ILogger logger, LogLevel logLevel, string messageTemplate, object[]? args)
	{
		var argsLength = args?.Length ?? 0;
		this.logger = logger;
		this.logLevel = logLevel;
		this.messageTemplate = messageTemplate;
		this.args = new object[argsLength + 1];
		if (args is not null)
		{
			Array.Copy(args, this.args, args.Length);
		}
		this.startingTimestamp = Stopwatch.GetTimestamp();
	}

	public void Dispose()
	{
		var delta = Stopwatch.GetElapsedTime(this.startingTimestamp);
		this.args[^1] = delta.TotalMilliseconds;
		this.logger.Log(this.logLevel, $"{this.messageTemplate} completed in {{OperationDurationMs}}ms", this.args);
	}
}
