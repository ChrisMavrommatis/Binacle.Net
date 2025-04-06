using Binacle.Net.UIModule.Models;

namespace Binacle.Net.UIModule.ApiModels.Responses;

internal class PackByCustomResponse
{
	public ResponseResultType Result { get; set; }
	public List<PackingResult>? Data { get; set; }
}
