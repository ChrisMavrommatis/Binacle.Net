using System.Text.Json.Serialization;
using Binacle.CompactNotation;
using Binacle.Lib.Abstractions.Models;
using Binacle.Net.Kernel.Logs.Models;

namespace Binacle.Net.DiagnosticsModule.Logs.Models;

// The packing-log channel message. Published by the API after each fit/pack; the generic LogsProcessor asks it to
// become a PackingLogEntry (ILogEntryConvertible) in the background — the request thread only enqueued references.
public class AlgorithmOperationLogChannelRequest : ILogEntryConvertible<PackingLogEntry>
{
	public required IReadOnlyCollection<IIdentifiableBin> Bins { get; init; }
	public required IReadOnlyCollection<IIdentifiableItem> Items { get; init; }
	public ILogParametersProvider? Parameters { get; init; }
	public required IDictionary<string, OperationResult> Results { get; init; }

	// Future attribution hook. The operation timestamp is stamped by the processor (its TimeProvider), not here —
	// the request thread captures no clock.
	public string? UserId { get; init; }

	internal AlgorithmOperationLogChannelRequest()
	{
	}

	// Builds the channel message with no copy: List<TBin> is an IReadOnlyCollection<TBin>, and
	// IReadOnlyCollection<out T> is covariant, so a list of IIdentifiable* flows straight in. We retain references
	// (not snapshots) — safe because the stored types are read-only and the algorithm has finished reading by the
	// time we enqueue. All shaping happens later, in ToLogEntry (background).
	public static AlgorithmOperationLogChannelRequest From<TBin, TItem, TParams>(
		List<TBin> bins,
		List<TItem> items,
		TParams parameters,
		IDictionary<string, OperationResult> results
	)
		where TBin : class, IIdentifiableBin
		where TItem : class, IIdentifiableItem
		where TParams : ILogParametersProvider
	{
		return new AlgorithmOperationLogChannelRequest
		{
			Bins = bins,
			Items = items,
			Parameters = parameters,
			Results = results,
		};
	}

	// Maps this request to its log line, in the background (the processor supplies the timestamp). Bins / items /
	// results become compact strings; parameters are the loose string list the request's parameter type provided.
	public PackingLogEntry ToLogEntry(DateTimeOffset timestamp)
	{
		return new PackingLogEntry
		{
			Timestamp = timestamp,
			UserId = this.UserId,
			Parameters = this.Parameters?.ToLogParameters(),
			Bins = MapCompact(this.Bins, bin => CompactNotationFormatter.FormatDimensions(bin)),
			Items = MapCompact(this.Items, item => CompactNotationFormatter.FormatDimensionsAndQuantity(item)),
			Results = this.Results.ToDictionary(entry => entry.Key, entry => ToLogResult(entry.Value)),
		};
	}

	private static LogResult ToLogResult(OperationResult result)
	{
		return new LogResult
		{
			Status = result.Status.ToString(),
			PackedBinVolumePercentage = result.PackedBinVolumePercentage,
			PackedItemsVolumePercentage = result.PackedItemsVolumePercentage,
			PackedItems = GroupCompact(result.PackedItems, item => CompactNotationFormatter.FormatItem(item)),
			UnpackedItems = GroupCompact(result.UnpackedItems, item => CompactNotationFormatter.FormatDimensionsAndQuantity(item)),
		};
	}

	// One compact string per id. Grouped defensively: ids are validated unique upstream, but the log must never
	// throw and stall the processor, so a stray duplicate simply takes the first.
	private static IReadOnlyDictionary<string, string> MapCompact<T>(
		IEnumerable<T> source,
		Func<T, string> toCompact)
		where T : IWithReadOnlyID
	{
		return source
			.GroupBy(item => item.ID)
			.ToDictionary(group => group.Key, group => toCompact(group.First()));
	}

	// Group items by id into compact strings. Several items can share an id (e.g. multiple packed units of the
	// same box), so each id maps to an array.
	private static IReadOnlyDictionary<string, string[]> GroupCompact<T>(
		IEnumerable<T> items,
		Func<T, string> toCompact)
		where T : IWithReadOnlyID
	{
		return items
			.GroupBy(item => item.ID)
			.ToDictionary(group => group.Key, group => group.Select(toCompact).ToArray());
	}
}

// One packing log line — the typed shape written as JSON by the background LogsProcessor.
// Bins / items are compact strings keyed by id ("small-box" -> "10x10x10").
public sealed record PackingLogEntry
{
	public required DateTimeOffset Timestamp { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? UserId { get; init; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public IReadOnlyList<string>? Parameters { get; init; }

	public required IReadOnlyDictionary<string, string> Bins { get; init; }
	public required IReadOnlyDictionary<string, string> Items { get; init; }
	public required IReadOnlyDictionary<string, LogResult> Results { get; init; }
}

// One algorithm's result within a packing log line. Packed / unpacked items are grouped by id and rendered as
// compact strings ("LxWxH (X,Y,Z)" for packed, "LxWxH [Q]" for unpacked).
public sealed record LogResult
{
	public required string Status { get; init; }
	public required decimal PackedBinVolumePercentage { get; init; }
	public required decimal PackedItemsVolumePercentage { get; init; }
	public required IReadOnlyDictionary<string, string[]> PackedItems { get; init; }
	public required IReadOnlyDictionary<string, string[]> UnpackedItems { get; init; }
}
