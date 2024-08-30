using System.Numerics;

namespace Binacle.Net.Lib.Abstractions.Models;

public interface IItemWithDimensions : IWithID, IWithDimensions, IWithReadOnlyDimensions
{

}

public interface IItemWithDimensions<T> : IWithID, IWithDimensions<T>, IWithReadOnlyDimensions<T>
	where T : INumber<T>
{

}
