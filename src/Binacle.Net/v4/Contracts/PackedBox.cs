using Binacle.Lib.Abstractions.Models;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class PackedBox : 
	IWithID, 
	IWithDimensions, 
	IWithCoordinates, 
	ViPaq.Abstractions.IWithDimensions<int>, 
	ViPaq.Abstractions.IWithCoordinates<int>
{
	public required string ID { get; set; }
	public required int Length { get; set; }
	public required int Width { get; set; }
	public required int Height { get; set; }
	public required int X { get; set; }
	public required int Y { get; set; }
	public required int Z { get; set; }
}
