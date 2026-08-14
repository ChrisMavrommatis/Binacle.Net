using Binacle.CompactNotation;
using System.ComponentModel.DataAnnotations;

namespace Binacle.Net.UIModule.ViewModels;

internal class Item :
	IWithDimensions,
	IWithQuantity
{
	public Item(int length, int width, int height) :
		this(length, width, height, 1)
	{

	}
	public Item(int length, int width, int height, int quantity)
	{
		this.Length = length;
		this.Width = width;
		this.Height = height;
		this.Quantity = quantity;
	}

	public string ID => CompactNotationFormatter.FormatDimensionsAndQuantity(this);

	[Required]
	[Range(1, ushort.MaxValue)]
	public int Length { get; set; }

	[Required]
	[Range(1, ushort.MaxValue)]
	public int Width { get; set; }

	[Required]
	[Range(1, ushort.MaxValue)]
	public int Height { get; set; }

	[Required]
	[Range(1, ushort.MaxValue)]
	public int Quantity { get; set; }

	public int Volume => this.CalculateVolume();
}
