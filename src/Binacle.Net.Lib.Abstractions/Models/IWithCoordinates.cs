using System.Numerics;

namespace Binacle.Net.Lib.Abstractions.Models;

public interface IWithCoordinates : IWithCoordinates<int>
{

}

public interface IWithCoordinates<T> : IWithReadOnlyCoordinates<T>
	where T : INumber<T>
{
	new T X { get; set; }
	new T Y { get; set; }
	new T Z { get; set; }
}
