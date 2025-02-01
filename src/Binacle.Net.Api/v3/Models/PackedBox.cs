using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.v3.Models;

#pragma warning disable CS1591
public class PackedBox : 
	IWithID, 
	IWithDimensions, 
	IWithCoordinates, 
	ViPaq.Abstractions.IWithDimensions<int>,
	ViPaq.Abstractions.IWithCoordinates<int>
{
	public required string ID { get; set; }
	public int Length { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
}
