using System.Numerics;

namespace Binacle.Net.Lib.Abstractions.Models;

public interface IWithVolume : IWithVolume<int>
{
}

public interface IWithVolume<T> : IWithReadOnlyVolume<T>
	where T: INumber<T>
{
	new T Volume { get; set; }
}
