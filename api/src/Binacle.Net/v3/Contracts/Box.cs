using Binacle.Lib.Abstractions.Models;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class Box : IWithID, IWithDimensions, IWithQuantity
{
	public required string ID { get; set; } 
	
	public required int Quantity { get; set; }
	public required int Length { get; set; }
	public required int Width { get; set; }
	public required int Height { get; set; }
}
