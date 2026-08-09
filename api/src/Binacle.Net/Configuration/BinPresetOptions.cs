using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Binacle.Lib.Abstractions.Models;
using Binacle.Net.Kernel.Configuration.Models;
using FluentValidation;

namespace Binacle.Net.Configuration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class BinPresetOptions : IConfigurationOptions
{
	public static string FilePath => "Presets.json";
	public static string SectionName => "PresetOptions";
	public static bool Optional => false;
	public static bool ReloadOnChange => true;
	public static string? GetEnvironmentFilePath(string environment) => null;


	public Dictionary<string, BinPresetOption> Presets { get; set; } = new();
	
	private readonly ConcurrentDictionary<string, BinOption> _binCache = new();
	
	public bool TryGetPreset(string presetName, [NotNullWhen(true)] out BinPresetOption? presetOption)
	{
		return this.Presets.TryGetValue(presetName, out presetOption);
	}
	
	public bool TryGetPresetBin(string presetName, string bin, [NotNullWhen(true)] out BinOption? binOption)
	{
		var key = $"{presetName}:{bin}";
		if (_binCache.TryGetValue(key, out binOption))
		{
			return true;
		}
		
		if (!this.Presets.TryGetValue(presetName, out var presetOption))
		{
			binOption = null;
			return false;
		}
		
		var foundBin = presetOption.Bins.FirstOrDefault(b => b.ID == bin);
		if (foundBin is null)
		{
			binOption = null;
			return false;
		}
		
		binOption = foundBin;
		_binCache[key] = binOption;
		return true;
	}
}

public class BinPresetOption
{
	public List<BinOption> Bins { get; set; } = new();
}


public class BinOption : IWithID, IWithDimensions, IIdentifiableBin
{
	public string ID { get; set; } = string.Empty;
	public int Length { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
}

internal class BinPresetOptionsOptionsValidator : AbstractValidator<BinPresetOptions>
{
	public BinPresetOptionsOptionsValidator()
	{
		// Every message names the preset and the bin it came from. The property path carries indexes
		// (Presets[0].Value.Bins[2].Length), which does not tell an operator which preset in their file that is.
		RuleForEach(x => x.Presets).ChildRules(presetValidator =>
		{
			presetValidator.RuleFor(x => x.Value.Bins)
				.NotEmpty()
				.WithMessage(preset => $"Preset '{preset.Key}' has no bins. Remove the preset or give it at least one.");

			// A preset is addressed as {preset}/{bin}, so two bins sharing an ID make one of them unreachable —
			// the lookup returns the first match and the second is silently dead.
			presetValidator.RuleFor(x => x.Value.Bins)
				.Must(bins => bins is null || bins.Select(bin => bin.ID).Distinct().Count() == bins.Count)
				.WithMessage(preset =>
					$"Preset '{preset.Key}' has more than one bin with the same ID: "
					+ $"{string.Join(", ", preset.Value.Bins.GroupBy(bin => bin.ID).Where(group => group.Count() > 1).Select(group => $"'{group.Key}'"))}."
				);

			presetValidator.RuleForEach(x => x.Value.Bins).ChildRules(binValidator =>
			{
				binValidator.RuleFor(x => x.ID)
					.NotEmpty()
					.WithMessage("A bin has no ID. Every bin needs one - it is how the bin is addressed in a route.");
				binValidator.RuleFor(x => x.Length)
					.GreaterThan(0)
					.WithMessage(bin => $"Length of bin '{bin.ID}' must be greater than 0. You entered {bin.Length}.");
				binValidator.RuleFor(x => x.Width)
					.GreaterThan(0)
					.WithMessage(bin => $"Width of bin '{bin.ID}' must be greater than 0. You entered {bin.Width}.");
				binValidator.RuleFor(x => x.Height)
					.GreaterThan(0)
					.WithMessage(bin => $"Height of bin '{bin.ID}' must be greater than 0. You entered {bin.Height}.");
			});
		});
	}
}
