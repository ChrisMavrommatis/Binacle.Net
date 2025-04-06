using System.Numerics;

namespace Binacle.Lib.Abstractions.Models;

public interface IWithReadOnlyDimensions : IWithReadOnlyDimensions<int>
{
}

public interface IWithReadOnlyDimensions<T>
	where T : INumber<T>
{
	T Length { get; }
	T Width { get; }
	T Height { get; }
}
