using System.Numerics;

namespace Binacle.Net.Lib.Abstractions.Models;

public interface IItemWithDimensions<T> : IItemWithReadOnlyDimensions<T>
	where T : INumber<T>
{

}
