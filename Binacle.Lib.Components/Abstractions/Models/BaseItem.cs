using System.Numerics;

namespace Binacle.Lib.Components.Abstractions.Models
{
    public abstract class BaseItem<TDimensions, TVolume> : IWithID, IWithDimensions<TDimensions>
        where TDimensions : struct, IBinaryInteger<TDimensions>
        where TVolume : struct, IBinaryInteger<TVolume>
    {
        public BaseItem(string id, IWithReadOnlyDimensions<TDimensions> item) :
            this(id, item.Length, item.Width, item.Height)
        {
        }

        public BaseItem(string id, TDimensions length, TDimensions width, TDimensions height)
        {
            this.ID = id;
            this.Length = length;
            this.Width = width;
            this.Height = height;
        }

        public string ID { get; set; }
        public TDimensions Length { get; set; }
        public TDimensions Width { get; set; }
        public TDimensions Height { get; set; }

    }
}