using Binacle.Lib;
using Binacle.Lib.Abstractions.Models;
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

	public string ID => this.FormatDimensions();

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
	
	public int Volume
	{
		get
		{
			return this.CalculateVolume();
		}
		set
		{

		}
	}
}
