namespace Binacle.Net.Kernel.Logs.Models;

// A channel request that knows how to become its log entry. The generic LogsProcessor calls this in the
// background, supplying the timestamp — so the concrete request/entry types live with the feature (a module),
// not in this generic infrastructure.
public interface ILogEntryConvertible<out TLogEntry>
{
	TLogEntry ToLogEntry(DateTimeOffset timestamp);
}
