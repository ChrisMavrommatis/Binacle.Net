using System.Numerics;

namespace Binacle.Geometry;

public interface IWithVolume : IWithVolume<int>
{
}

public interface IWithVolume<T> : IWithReadOnlyVolume<T>
	where T: struct, IBinaryInteger<T>
{
	new T Volume { get; set; }
}
