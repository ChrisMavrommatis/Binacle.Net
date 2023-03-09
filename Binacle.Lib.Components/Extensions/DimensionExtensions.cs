using Binacle.Lib.Components.Models;

namespace Binacle.Lib.Components.Extensions
{
    public static class DimensionExtensions
    {
        public static void CopyDimensionsFrom(this IWithDimensions target, IWithDimensions source)
        {
            target.Length = source.Length;
            target.Width = source.Width;
            target.Height = source.Height;
        }

        public static Dimensions ToDimensions(this VolumetricItem item)
        {
            return new Dimensions
            {
                Length = item.Length,
                Width = item.Width,
                Height = item.Height
            };
        }
    }
}
