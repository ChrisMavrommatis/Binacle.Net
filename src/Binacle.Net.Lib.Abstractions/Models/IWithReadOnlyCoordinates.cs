using System.Numerics;

namespace Binacle.Net.Lib.Abstractions.Models;

public interface IWithReadOnlyCoordinates : IWithReadOnlyCoordinates<int>
{

}

public interface IWithReadOnlyCoordinates<T> 
	where T: INumber<T>
{
	T X { get; }
	T Y { get; }
	T Z { get; }
}
