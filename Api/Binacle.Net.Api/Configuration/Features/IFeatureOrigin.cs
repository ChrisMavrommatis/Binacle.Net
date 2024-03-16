namespace Binacle.Net.Api.Configuration.Features;

public interface IFeatureOrigin
{
	public bool IsFeatureEnabled(string featureName);
}

