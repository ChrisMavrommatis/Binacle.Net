using Binacle.Net.Kernel.Features.Models;

namespace Binacle.Net.Kernel.Features.Providers.EnvironmentVariable;

internal class EnvironmentVariableFeatureProvider : IFeatureProvider
{
	public FeatureResult Get(string featureName)
	{
		var variable = Environment.GetEnvironmentVariable(featureName);
		if (variable is null)
			return FeatureResult.NotFound;

		var isTruthy = bool.TrueString == variable;
		if (isTruthy)
			return FeatureResult.Enabled;

		var isFalsy = bool.FalseString == variable;
		if (isFalsy)
			return FeatureResult.Disabled;

		return FeatureResult.NotFound;
	}

}
