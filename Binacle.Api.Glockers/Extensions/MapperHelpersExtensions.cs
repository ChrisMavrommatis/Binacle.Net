using Binacle.Api.Glockers.Models;

namespace Binacle.Api.Glockers.Extensions
{
    internal static class MapperHelpersExtensions
    {
        public static List<Binacle.Lib.Components.Models.Item> GetItemsForService(this GlockersQueryRequest request)
        {
            return request.Items.Select(x => new Binacle.Lib.Components.Models.Item(x.ID, x)).ToList();
        }

        public static List<Binacle.Lib.Components.Models.Bin> GetBinsForService(this GlockersOptions options)
        {
            return options.Lockers.Select(x => new Binacle.Lib.Components.Models.Bin(x.Size.ToString(), x)).ToList();
        }

    }
}
