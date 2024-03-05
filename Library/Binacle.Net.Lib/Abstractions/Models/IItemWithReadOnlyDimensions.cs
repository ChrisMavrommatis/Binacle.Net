using System.Numerics;

namespace Binacle.Net.Lib.Abstractions.Models;

public interface IItemWithReadOnlyDimensions<T> : IWithID, IWithReadOnlyDimensions<T>
	where T : INumber<T>
{

}
