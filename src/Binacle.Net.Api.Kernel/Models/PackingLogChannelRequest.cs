using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.Lib.Packing.Models;

namespace Binacle.Net.Api.Kernel.Models;

public class PackingLogChannelRequest
{
	public Dictionary<string, PackingResult> Results { get; set; }
}

public class FittingLogChannelRequest
{
	public Dictionary<string, FittingResult> Results { get; set; }

}
