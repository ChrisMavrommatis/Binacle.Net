using Binacle.Net.Lib.Abstractions.Models;
using System.Numerics;

namespace Binacle.Net.Lib.Strategies.Models;

internal abstract class VolumetricItemBase<TDimensions, TVolume> : IWithReadOnlyDimensions<TDimensions>, IWithReadOnlyVolume<TVolume>
	where TDimensions : struct, INumber<TDimensions>
	where TVolume : struct, INumber<TVolume>
{
	protected private TDimensions length;
	protected private TDimensions width;
	protected private TDimensions height;

	private TDimensions longestDimension;
	private TVolume volume;

	internal VolumetricItemBase(IWithReadOnlyDimensions<TDimensions> item) : this(item.Length, item.Width, item.Height)
	{

	}

	internal VolumetricItemBase(TDimensions length, TDimensions width, TDimensions height)
	{
		this.length = length;
		this.width = width;
		this.height = height;

		volume = CalculateVolume();
		longestDimension = CalculateLongestDimension();
	}

	public TDimensions Length { get => length; }
	public TDimensions Width { get => width; }
	public TDimensions Height { get => height; }

	internal TDimensions LongestDimension { get => longestDimension; }
	public TVolume Volume { get => volume; }

	internal abstract TVolume CalculateVolume();
	internal abstract TDimensions CalculateLongestDimension();
	internal abstract TDimensions CalculateShortestDimension();
}
