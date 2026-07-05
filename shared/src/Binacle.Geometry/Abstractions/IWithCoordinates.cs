using System.Numerics;

namespace Binacle.Geometry;

// A thing whose position can be set. Consumers that write (e.g. vipaq deserialize) use this.
public interface IWithCoordinates<T> : IWithReadOnlyCoordinates<T>
	where T : struct, IBinaryInteger<T>
{
	new T X { get; set; }
	new T Y { get; set; }
	new T Z { get; set; }
}

// Non-generic int shortcut.
public interface IWithCoordinates : IWithCoordinates<int>
{
}
