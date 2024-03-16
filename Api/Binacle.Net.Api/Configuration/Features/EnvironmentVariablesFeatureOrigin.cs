namespace Binacle.Net.Api.Configuration.Features;

public class EnvironmentVariablesFeatureOrigin : IFeatureOrigin
{
	public bool IsFeatureEnabled(string featureName)
	{
		return bool.TrueString == Environment.GetEnvironmentVariable(featureName);
	}
}

