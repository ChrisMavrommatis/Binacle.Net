using Binacle.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Binacle.Api.BoxNow.Models
{
    public class LockerBin : IWithDimensions
    {
        public int Size { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
    }

    public class BoxItem: IWithID, IWithDimensions
    {
        public string ID { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
    }
}
