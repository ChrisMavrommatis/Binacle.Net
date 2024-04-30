using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ChrisMavrommatis.Logging.Operations;

public class TimedOperation : IDisposable
{
	private readonly ILogger logger;
	private readonly LogLevel logLevel;
	private readonly string messageTemplate;
	private readonly object[] args;
	private readonly long startingTimestamp;

	public TimedOperation(ILogger logger, LogLevel logLevel, string messageTemplate, object[]? args)
	{
		this.logger = logger;
		this.logLevel = logLevel;
		this.messageTemplate = messageTemplate;
		this.args = new object[args.Length + 1];
		Array.Copy(args, this.args, args.Length);
		this.startingTimestamp = Stopwatch.GetTimestamp();
	}

	public TimedOperation WithScope(string messageFormat, params object?[] args)
	{
		this.logger.BeginScope(messageFormat, args);
		return this;
	}

	public TimedOperation WithNamedState<TKey, TValue>(string name, IEnumerable<KeyValuePair<TKey, TValue>> state)
	{
		var namedState = new Dictionary<string, IEnumerable<KeyValuePair<TKey, TValue>>> { { name, state } };
		this.logger.BeginScope(namedState);
		return this;
	}

	public void Dispose()
	{
		var delta = Stopwatch.GetElapsedTime(this.startingTimestamp);
		this.args[^1] = delta.TotalMilliseconds;
		this.logger.Log(this.logLevel, $"{this.messageTemplate} completed in {{OperationDurationMs}}ms", this.args);
	}
}
