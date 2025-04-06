using Binacle.Net.UIModule.ExtensionMethods;
using Binacle.Lib.Abstractions.Models;
using System.ComponentModel.DataAnnotations;
using Binacle.Lib;

namespace Binacle.Net.UIModule.ViewModels;

internal class Bin :
	IWithID,
	IWithDimensions
{
	public Bin(int length, int width, int height)
	{
		this.Length = length;
		this.Width = width;
		this.Height = height;
	}

	public string ID 
	{ 
		get => this.FormatDimensions();
		set => _ = value;
	}

	[Required]
	[Range(1, ushort.MaxValue)]
	public int Length { get; set; }

	[Required]
	[Range(1, ushort.MaxValue)]
	public int Width { get; set; }

	[Required]
	[Range(1, ushort.MaxValue)]
	public int Height { get; set; }
}
