namespace Binacle.Api.Models
{
    public class QueryRequest
    {
        public List<Container> Containers { get; set; }
        public List<Box> Items { get; set; }

        public List<Lib.Components.Models.Bin> GetBinsForService()
        {
            return this.Containers.Select(container => new Lib.Components.Models.Bin(container.ID, container)).ToList();
        }

        public List<Lib.Components.Models.Item> GetItemsForService()
        {
            return this.Items.SelectMany(item =>
            {
                return Enumerable.Range(0, item.Quantity).Select(_ => new Lib.Components.Models.Item(item.ID, item));
            }).ToList();
        }
    }
}
