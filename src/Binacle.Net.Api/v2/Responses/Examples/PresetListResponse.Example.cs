using Binacle.Net.Api.Models;
using ChrisMavrommatis.SwaggerExamples;

namespace Binacle.Net.Api.v2.Responses.Examples;

internal class PresetListResponseExample : SingleSwaggerExamplesProvider<Response<Dictionary<string, List<Bin>>>>
{
	public override Response<Dictionary<string, List<Bin>>> GetExample()
	{
		var presets = new Dictionary<string, List<Bin>>()
		{
			{
				"preset1",
				new List<Bin>()
				{
					new Bin { ID = "preset1_bin1", Length = 10, Width = 10, Height = 10 },
					new Bin { ID = "preset1_bin2", Length = 20, Width = 20, Height = 20 },
					new Bin { ID = "preset1_bin3", Length = 30, Width = 30, Height = 30 },
				}
			},
			{
				"preset2",
				new List<Bin>()
				{
					new Bin { ID = "preset2_bin1", Length = 10, Width = 20, Height = 30 },
					new Bin { ID = "preset2_bin2", Length = 30, Width = 60, Height = 60 },
				}

			}
		};
		return Response.Success(presets);
	}
}
