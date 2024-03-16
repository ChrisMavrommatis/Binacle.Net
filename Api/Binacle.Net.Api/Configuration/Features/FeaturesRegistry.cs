using Binacle.Net.Api.Configuration.Features;

namespace Binacle.Net.Api;

public static class FeaturesRegistry
{

	private static List<IFeatureOrigin> featureOrigins = new List<IFeatureOrigin>
	{
		 new EnvironmentVariablesFeatureOrigin()
	};

	public static bool IsFeatureEnabled(string featureName)
	{
		foreach (var featureOrigin in featureOrigins)
		{
			if (featureOrigin.IsFeatureEnabled(featureName))
			{
				return true;
			}
		}
		return false;
	}
}

