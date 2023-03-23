using Binacle.Lib.Components.Models;

namespace Binacle.Lib.Extensions
{
    internal static class VolumetricItemExtensions
    {
        internal static IEnumerable<VolumetricItem> GetOrientations(this VolumetricItem item)
        {
            // Length  Width  Height
            //  8      45     62
            //  8      62     45
            // 45       8     62
            // 45      62      8
            // 62       8     45
            // 62      45      8

            // L W H
            // L H W
            // W L H
            // W H L
            // H L W
            // H W L
            yield return new VolumetricItem(item.Length, item.Width, item.Height);
            yield return new VolumetricItem(item.Length, item.Height, item.Width);

            yield return new VolumetricItem(item.Width, item.Length, item.Height);
            yield return new VolumetricItem(item.Width, item.Height, item.Length);

            yield return new VolumetricItem(item.Height, item.Length, item.Width);
            yield return new VolumetricItem(item.Height, item.Width, item.Length);
        }

        internal static IEnumerable<VolumetricItemStruct> GetOrientationsStruct(this VolumetricItem item)
        {
            // Length  Width  Height
            //  8      45     62
            //  8      62     45
            // 45       8     62
            // 45      62      8
            // 62       8     45
            // 62      45      8

            // L W H
            // L H W
            // W L H
            // W H L
            // H L W
            // H W L
            yield return new VolumetricItemStruct(item.Length, item.Width, item.Height);
            yield return new VolumetricItemStruct(item.Length, item.Height, item.Width);

            yield return new VolumetricItemStruct(item.Width, item.Length, item.Height);
            yield return new VolumetricItemStruct(item.Width, item.Height, item.Length);

            yield return new VolumetricItemStruct(item.Height, item.Length, item.Width);
            yield return new VolumetricItemStruct(item.Height, item.Width, item.Length);
        }
    }
}
