namespace Binacle.Net;

public class FeatureOptions
{
	// A feature is a name and, for the ones that answer on a URL, where. Whoever switches it on records the
	// path, because some of them are configurable and nobody else can know where one ended up.
	private readonly Dictionary<string, string?> features = new(StringComparer.OrdinalIgnoreCase);

	public void AddFeature(string feature, string? path = null)
	{
		this.features[feature] = path;
	}

	public void RemoveFeature(string feature)
	{
		this.features.Remove(feature);
	}

	public bool IsFeatureEnabled(string feature)
	{
		return this.features.ContainsKey(feature);
	}

	public string? PathFor(string feature)
	{
		return this.features.GetValueOrDefault(feature);
	}

	public IReadOnlyCollection<string> EnabledFeatures => this.features.Keys;
}
