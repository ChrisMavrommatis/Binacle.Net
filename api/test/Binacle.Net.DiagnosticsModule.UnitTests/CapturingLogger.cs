using Microsoft.Extensions.Logging;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

// Several of these middlewares exist to say something rather than to do something, so what they logged is the
// assertion. NullLogger cannot answer that and a mocking library would be a dependency for one interface.
internal sealed class CapturingLogger<T> : ILogger<T>
{
	private readonly List<(LogLevel Level, string Message)> entries = [];

	public IReadOnlyList<string> Warnings =>
		this.entries
			.Where(entry => entry.Level == LogLevel.Warning)
			.Select(entry => entry.Message)
			.ToArray();

	public void Log<TState>(
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception?, string> formatter
	) => this.entries
		.Add((logLevel, formatter(state, exception)));

	public bool IsEnabled(LogLevel logLevel) => true;

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
}
