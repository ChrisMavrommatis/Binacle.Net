namespace Binacle.Net.Api.Models
{
    public class QueryRequest
    {
        public List<Container> Containers { get; set; }
        public List<Box> Items { get; set; }

        public List<Lib.Models.Item> GetBinsForService()
        {
            return this.Containers.Select(container => new Lib.Models.Item(container.ID, container)).ToList();
        }

        public List<Lib.Models.Item> GetItemsForService()
        {
            return this.Items.SelectMany(item =>
            {
                return Enumerable.Range(0, item.Quantity).Select(_ => new Lib.Models.Item(item.ID, item));
            }).ToList();
        }
    }
}
