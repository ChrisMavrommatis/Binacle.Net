namespace Binacle.Net.Kernel.Logs.Models;

// TChannelRequest isn't used in the body — it keys the DI registration, so each log processor gets its own
// options singleton rather than sharing one.
public class LogsProcessorOptions<TChannelRequest>
{
	public required string Path { get; init; }
	public required string FileNameFormat { get; init; }
	public required string DateFormat { get; init; }
	// Safety valve: after this many consecutive failures the processor stops itself. Sensible default; override if a
	// caller needs to.
	public int MaxConsecutiveAllowedExceptions { get; init; } = 10;
}
