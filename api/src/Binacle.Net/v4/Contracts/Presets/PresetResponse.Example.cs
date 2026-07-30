using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v4.Contracts.Presets;

internal class PresetResponseExample : ISingleOpenApiExamplesProvider<PresetResponse>
{
	public IOpenApiExample<PresetResponse> GetExample()
	{
		return OpenApiExample.Create(
			"presetResponse",
			"Preset Response",
			PresetResponse.From(
				"preset1",
				[
					new Bin { ID = "preset1_bin1", Length = 10, Width = 10, Height = 10 },
					new Bin { ID = "preset1_bin2", Length = 20, Width = 20, Height = 20 },
					new Bin { ID = "preset1_bin3", Length = 30, Width = 30, Height = 30 },
				]
			)
		);
	}
}
