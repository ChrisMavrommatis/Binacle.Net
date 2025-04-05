using Binacle.Net.Api.v2.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.Api.v2.Responses.Examples;

internal class PresetListResponseExample : ISingleOpenApiExamplesProvider<PresetListResponse>
{
	public IOpenApiExample<PresetListResponse> GetExample()
	{
		var presets = new Dictionary<string, List<Bin>>()
		{
			{
				"preset1", [
					new Bin { ID = "preset1_bin1", Length = 10, Width = 10, Height = 10 },
					new Bin { ID = "preset1_bin2", Length = 20, Width = 20, Height = 20 },
					new Bin { ID = "preset1_bin3", Length = 30, Width = 30, Height = 30 },
				]
			},
			{
				"preset2", [
					new Bin { ID = "preset2_bin1", Length = 10, Width = 20, Height = 30 },
					new Bin { ID = "preset2_bin2", Length = 30, Width = 60, Height = 60 },
				]
			}
		};
		return OpenApiExample.Create(
			"Preset List Response",
			PresetListResponse.Create(presets)
		);
	}
}
