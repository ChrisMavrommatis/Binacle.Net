using System.Numerics;

namespace Binacle.Geometry;

// Non-generic int shortcut.
public interface IWithReadOnlyVolume: IWithReadOnlyVolume<int>
{
}

// A thing that has a volume. Read-only — consumers that only read (e.g. formatting) use this.
public interface IWithReadOnlyVolume<T> where T : struct, IBinaryInteger<T>
{
	T Volume { get; }
}
