using Binacle.Api.Glockers.Models;

namespace Binacle.Api.Glockers.Extensions
{
    internal static class MapperHelpersExtensions
    {
        public static List<Lib.Components.Models.Item> GetItemsForService(this GlockersQueryRequest request)
        {
            return request.Items.SelectMany(item =>
            {
                return Enumerable.Range(0, item.Quantity).Select(_ => new Lib.Components.Models.Item(item.ID, item));
            }).ToList();
        }

        public static List<Lib.Components.Models.Bin> GetBinsForService(this GlockersOptions options)
        {
            return options.Lockers.Select(x => new Lib.Components.Models.Bin(x.Size.ToString(), x)).ToList();
        }

    }
}
