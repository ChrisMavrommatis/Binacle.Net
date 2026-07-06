namespace Binacle.Net.Kernel.Logs.Models;

// TChannelRequest isn't used in the body — it keys the DI registration, so each log processor gets its own
// options singleton rather than sharing one.
public class LogsProcessorOptions<TChannelRequest>
{
	public required string Path { get; init; }
	public required string FileNameFormat { get; init; }
	public required string DateFormat { get; init; }
	public required int MaxConsecutiveAllowedExceptions { get; init; }
}
