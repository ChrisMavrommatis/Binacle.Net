namespace Binacle.Net.Api.Models;

public class LogsProcessorOptions<TChannelRequest>
{
	public required string Path { get; init; }
	public required string FileNameFormat { get; init; }
	public required Func<TChannelRequest, Dictionary<string, object>> LogFormatter { get; init; }
}
