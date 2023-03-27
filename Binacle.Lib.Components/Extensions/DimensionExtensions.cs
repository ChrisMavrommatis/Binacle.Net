using Binacle.Lib.Components.Abstractions.Models;
using System.Numerics;

namespace Binacle.Lib.Components.Extensions
{
    public static class DimensionExtensions
    {
        public static void CopyDimensionsFrom<T>(this IWithDimensions<T> target, IWithReadOnlyDimensions<T> source)
            where T: struct, IBinaryInteger<T>
        {
            target.Length = source.Length;
            target.Width = source.Width;
            target.Height = source.Height;
        }
    }
}
