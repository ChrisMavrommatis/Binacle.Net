using Binacle.Net.Api.UIModule.ExtensionMethods;
using Binacle.Net.Lib.Abstractions.Models;
using System.ComponentModel.DataAnnotations;
using Binacle.Net.Lib;

namespace Binacle.Net.Api.UIModule.ViewModels;

internal class Bin : IWithID, IWithDimensions
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
