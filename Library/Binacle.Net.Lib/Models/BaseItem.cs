using Binacle.Net.Lib.Abstractions.Models;
using System.Numerics;

namespace Binacle.Net.Lib.Models;

public abstract class BaseItem<TDimensions> : IWithID, IWithDimensions<TDimensions>
	where TDimensions : struct, INumber<TDimensions>
{
	internal BaseItem(string id, IWithReadOnlyDimensions<TDimensions> item) :
		this(id, item.Length, item.Width, item.Height)
	{
	}

	internal BaseItem(string id, TDimensions length, TDimensions width, TDimensions height)
	{
		this.ID = id;
		this.Length = length;
		this.Width = width;
		this.Height = height;
	}

	public string ID { get; set; }
	public TDimensions Length { get; set; }
	public TDimensions Width { get; set; }
	public TDimensions Height { get; set; }
}
