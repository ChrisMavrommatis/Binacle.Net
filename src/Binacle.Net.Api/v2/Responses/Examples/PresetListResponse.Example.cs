using ChrisMavrommatis.SwaggerExamples;

namespace Binacle.Net.Api.v2.Responses.Examples;

internal class PresetListResponseExample : SingleSwaggerExamplesProvider<PresetListResponse>
{
	public override PresetListResponse GetExample()
	{
		var presets = new Dictionary<string, List<v2.Models.Bin>>()
		{
			{
				"preset1", [
					new  v2.Models.Bin{ ID = "preset1_bin1", Length = 10, Width = 10, Height = 10 },
					new v2.Models.Bin{ ID = "preset1_bin2", Length = 20, Width = 20, Height = 20 },
					new v2.Models.Bin { ID = "preset1_bin3", Length = 30, Width = 30, Height = 30 },
				]
			},
			{
				"preset2", [
					new v2.Models.Bin { ID = "preset2_bin1", Length = 10, Width = 20, Height = 30 },
					new v2.Models.Bin { ID = "preset2_bin2", Length = 30, Width = 60, Height = 60 },
				]

			}
		};
		return PresetListResponse.Create(presets);
	}
}
