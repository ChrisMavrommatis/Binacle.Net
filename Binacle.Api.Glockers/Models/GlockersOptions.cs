namespace Binacle.Api.Glockers.Models
{
    public class GlockersOptions
    {
        public const string SectionName = "Glockers";
        public const string Path = "Glockers.json";
        public List<Locker> Lockers { get; set; }
    }
}
