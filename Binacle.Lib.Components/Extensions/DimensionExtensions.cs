using Binacle.Lib.Components.Models;

namespace Binacle.Lib.Components.Extensions
{
    public static class DimensionExtensions
    {
        public static void CopyFrom(this IWithDimensions target, IWithDimensions source)
        {
            target.Length = source.Length;
            target.Width = source.Width;
            target.Height = source.Height;
        }
    }
}
