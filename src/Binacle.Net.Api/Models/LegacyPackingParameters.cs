﻿
namespace Binacle.Net.Api.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class LegacyPackingParameters
{
	public required bool OptInToEarlyFails { get; init; }
	public required bool ReportPackedItemsOnlyWhenFullyPacked { get; init; }
	public required bool NeverReportUnpackedItems { get; init; }
	public required bool StopAtSmallestBin { get; init; }
}
