using Binacle.Lib.Abstractions.Models;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

// [Migrate-Review] drop the explicit Binacle.Geometry.IWithDimensions<int> below once lib's IWithDimensions
// shim reaches the mutable leaf generic (see .agents/plans/shared-geometry-extraction.md).
public class Bin :
	IWithID,
	IWithDimensions,
	Binacle.Geometry.IWithDimensions<int>
{
	public required string ID { get; set; }
	public required int Length { get; set; }
	public required int Width { get; set; }
	public required  int Height { get; set; }
}
