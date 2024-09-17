using Binacle.Net.Api.UIModule.Models;

namespace Binacle.Net.Api.UIModule.ApiModels.Responses;

internal class PackByCustomResponse
{
	// {"result":"Success","data":[{"result":"FullyPacked","bin":{"id":"60x40x10","length":60,"width":40,"height":10},"packedItems":[{"id":"10x15x15-q2","dimensions":{"length":15,"width":15,"height":10},"coordinates":{"x":0,"y":0,"z":0}},{"id":"10x15x15-q2","dimensions":{"length":15,"width":15,"height":10},"coordinates":{"x":15,"y":0,"z":0}},{"id":"12x15x10-q3","dimensions":{"length":12,"width":15,"height":10},"coordinates":{"x":0,"y":15,"z":0}},{"id":"12x15x10-q3","dimensions":{"length":12,"width":15,"height":10},"coordinates":{"x":30,"y":0,"z":0}},{"id":"12x15x10-q3","dimensions":{"length":12,"width":15,"height":10},"coordinates":{"x":15,"y":15,"z":0}},{"id":"2x5x10-q7","dimensions":{"length":2,"width":5,"height":10},"coordinates":{"x":12,"y":15,"z":0}},{"id":"2x5x10-q7","dimensions":{"length":2,"width":5,"height":10},"coordinates":{"x":0,"y":30,"z":0}},{"id":"2x5x10-q7","dimensions":{"length":2,"width":5,"height":10},"coordinates":{"x":42,"y":0,"z":0}},{"id":"2x5x10-q7","dimensions":{"length":2,"width":5,"height":10},"coordinates":{"x":30,"y":15,"z":0}},{"id":"2x5x10-q7","dimensions":{"length":2,"width":5,"height":10},"coordinates":{"x":27,"y":15,"z":0}},{"id":"2x5x10-q7","dimensions":{"length":2,"width":5,"height":10},"coordinates":{"x":15,"y":30,"z":0}},{"id":"2x5x10-q7","dimensions":{"length":2,"width":5,"height":10},"coordinates":{"x":12,"y":20,"z":0}}],"unpackedItems":[],"packedItemsVolumePercentage":100,"packedBinVolumePercentage":44.17}]}
	public ResponseResultType Result { get; set; }
	public List<PackingResult> Data { get; set; }

}
