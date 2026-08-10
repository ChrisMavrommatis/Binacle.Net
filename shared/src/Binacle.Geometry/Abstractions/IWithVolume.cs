using System.Numerics;

namespace Binacle.Geometry;

// Non-generic int shortcut.
public interface IWithVolume : IWithVolume<int>
{
}

// A thing whose volume can be set.
public interface IWithVolume<T> : IWithReadOnlyVolume<T>
	where T: struct, IBinaryInteger<T>
{
	new T Volume { get; set; }
}
