﻿using Microsoft.Extensions.Logging;

namespace ChrisMavrommatis.Logging;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public static class LoggerExtensions
{
	public static IDisposable? EnrichState(this ILogger logger, string name, Dictionary<string, object> state)
	{
		var namedState = new Dictionary<string, object> { { name, state } };
		return logger.BeginScope(namedState);
	}

	public static IDisposable? EnrichState(this ILogger logger, string name, IEnumerable<string> state)
	{
		var namedState = new Dictionary<string, object> { { name, state } };
		return logger.BeginScope(namedState);
	}
}
