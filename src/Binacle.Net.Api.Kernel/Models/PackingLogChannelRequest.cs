using Binacle.Net.Lib.Packing.Models;

namespace Binacle.Net.Api.Kernel.Models;

public class PackingLogChannelRequest
{
	public Dictionary<string, PackingResult> Results { get; set; }
}
