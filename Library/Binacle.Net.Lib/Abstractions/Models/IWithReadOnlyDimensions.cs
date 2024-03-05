using System.Numerics;

namespace Binacle.Net.Lib.Abstractions.Models;

public interface IWithReadOnlyDimensions<T>
	where T : INumber<T>
{
	T Length { get; }
	T Width { get; }
	T Height { get; }
}
