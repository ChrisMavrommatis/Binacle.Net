using System.Numerics;

namespace Binacle.Lib.Components.Abstractions.Models
{
    public interface IWithReadOnlyVolume<T> where T : IBinaryInteger<T>
    {
        T Volume { get; }
    }
}
