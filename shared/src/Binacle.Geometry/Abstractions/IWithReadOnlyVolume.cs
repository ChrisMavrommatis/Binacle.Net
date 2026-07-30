using System.Numerics;

namespace Binacle.Geometry;

public interface IWithReadOnlyVolume: IWithReadOnlyVolume<int>
{
}

public interface IWithReadOnlyVolume<T> where T : struct, IBinaryInteger<T>
{
	T Volume { get; }
}
