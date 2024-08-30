using System.Numerics;

namespace Binacle.Net.Lib.Abstractions.Models;

public interface IWithDimensions : IWithReadOnlyDimensions
{
	new int Length { get; set; }
	new int Width { get; set; }
	new int Height { get; set; }
}

public interface IWithDimensions<T> : IWithReadOnlyDimensions<T>
	 where T : INumber<T>
{
	new T Length { get; set; }
	new T Width { get; set; }
	new T Height { get; set; }
}
