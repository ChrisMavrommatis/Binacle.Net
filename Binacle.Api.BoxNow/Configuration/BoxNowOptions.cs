using Binacle.Api.BoxNow.Models;

namespace Binacle.Api.BoxNow.Configuration
{
    public class BoxNowOptions
    {
        public const string SectionName = "BoxNow";
        public const string Path = "BoxNow.json";
        public List<LockerBin> Lockers { get; set; }
    }
}
