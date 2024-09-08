using Binacle.Net.Lib.Abstractions.Models;
using System.ComponentModel.DataAnnotations;

namespace Binacle.Net.Api.UIModule.Models;

public class Bin : IWithDimensions
{
	public Bin(int length, int width, int height)
	{
		this.Length = length;
		this.Width = width;
		this.Height = height;
	}
	public int Length { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
}
