using Binacle.Net.Kernel.OpenApi.Attributes;
using Binacle.Net.Configuration;
using System.ComponentModel;

namespace Binacle.Net.v4.Contracts.Presets;

#pragma warning disable CS1591

[Description("A preset and the bins it contains.")]
[OpenApiRequireNonNullable]
public class PresetResponse
{
	[Description(SchemaDescriptions.PresetName)]
	public required string Name { get; init; }
	
	[Description(SchemaDescriptions.Bins)]
	public required List<Bin> Bins { get; init; }

	public static PresetResponse From(string name, List<Bin> bins)
	{
		return new PresetResponse
		{
			Name = name,
			Bins = bins
		};
	}

	public static PresetResponse From(string name, BinPresetOption presetOption)
	{
		var bins = presetOption.Bins
			.Select(bin => new Bin()
			{
				ID = bin.ID,
				Length = bin.Length,
				Height = bin.Height,
				Width = bin.Width
			}).ToList();

		return From(name, bins);
	}
}
