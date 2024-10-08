using System.Numerics;

namespace Binacle.Net.Lib.Abstractions.Models;

public interface IWithReadOnlyVolume: IWithReadOnlyVolume<int>
{
}

public interface IWithReadOnlyVolume<T> where T : INumber<T>
{
	T Volume { get; }
}
