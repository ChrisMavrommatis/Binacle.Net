using Binacle.Net.Kernel.Features.Models;
using Microsoft.Extensions.Configuration;

namespace Binacle.Net.Kernel.Features.Providers.Configuration;

internal class ConfigurationFeatureProvider : IFeatureProvider
{
	internal const string DefaultSectionName = "Features";
	private Dictionary<string, object> features = new Dictionary<string, object>();

	public ConfigurationFeatureProvider(IConfiguration configuration, string? sectionName = null)
	{
		var section = configuration.GetSection(sectionName ?? DefaultSectionName);
		section.Bind(this.features);
	}

	public FeatureResult Get(string featureName)
	{
		if (!this.features.TryGetValue(featureName, out var value))
			return FeatureResult.NotFound;

		if (value is bool booleanValue)
			return EvaluateResult(booleanValue);

		if (value is string stringValue)
			return EvaluateResult(stringValue);

		return FeatureResult.NotFound;
	}

	private static FeatureResult EvaluateResult(bool isTruthy)
	{
		if (isTruthy)
			return FeatureResult.Enabled;

		return FeatureResult.Disabled;
	}

	private static FeatureResult EvaluateResult(string value)
	{
		var isTruthy = bool.TrueString == value;
		if (isTruthy)
			return FeatureResult.Enabled;

		var isFalsy = bool.FalseString == value;
		if (isFalsy)
			return FeatureResult.Disabled;

		return FeatureResult.NotFound;
	}
}
