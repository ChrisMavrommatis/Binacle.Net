using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Strategies.Models
{
    internal abstract class BinBase : VolumetricItem, IWithID
    {
        internal BinBase(string id, IWithReadOnlyDimensions<ushort> item) : base(item)
        {
            ID = id;
        }

        internal BinBase(string id, ushort length, ushort width, ushort height)
            : base(length, width, height)
        {
            ID = id;
        }

        public string ID { get; set; }
    }
}
