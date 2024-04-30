using ChrisMavrommatis.Features.Models;
using Microsoft.Extensions.Configuration;

namespace ChrisMavrommatis.Features.Providers.Configuration;

internal class ConfigurationFeatureProvider : IFeatureProvider
{
	internal const string SectionName = "Features";
	private Dictionary<string, object> features = new Dictionary<string, object>();

	public ConfigurationFeatureProvider(IConfiguration configuration)
	{
		var section = configuration.GetSection(SectionName);
		section.Bind(this.features);
	}

	public FeatureResult Get(string featureName)
	{
		if (!this.features.TryGetValue(featureName, out var value))
			return FeatureResult.NotFound;

		if(value is bool booleanValue)
			return EvaluateResult(booleanValue);

		if (value is string stringValue)
			return EvaluateResult(bool.TrueString == stringValue);

		return FeatureResult.NotFound;
	}

	private static FeatureResult EvaluateResult(bool isTruthy)
	{
		if(isTruthy)
			return FeatureResult.Enabled;

		return FeatureResult.Disabled;
	}
}
