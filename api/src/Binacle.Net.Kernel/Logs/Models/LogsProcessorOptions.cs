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

	// Null (the default) means never delete: these logs are an archive, so keeping them is the safe default and
	// pruning is the operator's job. Set it (e.g. 7) to auto-delete local files older than that many days.
	public int? RetentionDays { get; init; }
}
