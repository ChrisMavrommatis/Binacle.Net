using Binacle.Lib.Abstractions.Models;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class Bin :
	IWithID,
	IWithDimensions,
	IIdentifiableBin
{
	public required string ID { get; set; }
	public required int Length { get; set; }
	public required int Width { get; set; }
	public required  int Height { get; set; }
	
	
	internal static Bin From(string id, int length, int width, int height)
	{
		return new Bin()
		{
			ID = id,
			Length = length,
			Width = width,
			Height = height
		};
	}
	internal static Bin From(PackedBin bin)
	{
		return new Bin()
		{
			ID = bin.ID,
			Height = bin.Height,
			Length = bin.Length,
			Width = bin.Width
		};
	}
}
