using ChrisMavrommatis.Features.Models;

namespace ChrisMavrommatis.Features.Providers.EnvironmentVariable;

internal class EnvironmentVariableFeatureProvider : IFeatureProvider
{
	public FeatureResult Get(string featureName)
	{
		var variable = Environment.GetEnvironmentVariable(featureName);
		if (variable is null)
			return FeatureResult.NotFound;

		if (bool.TrueString == variable)
			return FeatureResult.Enabled;

		return FeatureResult.Disabled;
	}
}
