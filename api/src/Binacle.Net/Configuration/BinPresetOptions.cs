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
	
	private ConcurrentDictionary<string, BinOption> _binCache = new();
	
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


public class BinOption : IWithID, IWithDimensions
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
		RuleForEach(x => x.Presets).ChildRules(presetValidator =>
		{
			presetValidator.RuleFor(x => x.Value.Bins)
				.NotNull()
				.NotEmpty();

			presetValidator.RuleForEach(x => x.Value.Bins).ChildRules(binValidator =>
			{
				binValidator.RuleFor(x => x.Height).GreaterThan(0).WithMessage($"Height in Bin must be greater than 0");
				binValidator.RuleFor(x => x.Width).GreaterThan(0).WithMessage($"Width in Bin must be greater than 0");
				binValidator.RuleFor(x => x.Length).GreaterThan(0).WithMessage($"Length in Bin must be greater than 0");
			});
		});
	}
}
