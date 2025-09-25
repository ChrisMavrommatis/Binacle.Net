namespace Binacle.Net;

public class FeatureOptions
{
	private readonly HashSet<string> enabledFeatures = new();
	
	public void AddFeature(string feature)
	{
		this.enabledFeatures.Add(feature);
	}
	
	public void RemoveFeature(string feature)
	{
		this.enabledFeatures.Remove(feature);
	}
	
	public bool IsFeatureEnabled(string feature)
	{
		return this.enabledFeatures.Contains(feature);
	}
}
