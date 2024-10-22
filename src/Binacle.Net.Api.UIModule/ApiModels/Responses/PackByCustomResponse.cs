using Binacle.Net.Api.UIModule.Models;

namespace Binacle.Net.Api.UIModule.ApiModels.Responses;

internal class PackByCustomResponse
{
	public ResponseResultType Result { get; set; }
	public List<PackingResult>? Data { get; set; }

}
