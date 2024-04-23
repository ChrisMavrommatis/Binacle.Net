using ChrisMavrommatis.Features.Models;
using ChrisMavrommatis.Features.Providers;

namespace ChrisMavrommatis.Features.Services;


public interface IFeatureManager
{
	bool IsEnabled(string featureName);
}

internal class FeatureManager : IFeatureManager
{
	private readonly IEnumerable<IFeatureProvider> providers;
	private readonly FeatureBehavior defaultNotFoundBehavior;

	public FeatureManager(
		IEnumerable<IFeatureProvider> providers, 
		FeatureBehavior defaultNotFoundBehavior
		)
	{
		this.providers = providers;
		this.defaultNotFoundBehavior = defaultNotFoundBehavior;
	}

	internal static IFeatureManager None = new NullFeatureManager();

	public bool IsEnabled(string featureName)
	{
		foreach(var provider in this.providers)
		{
			var feature = provider.Get(featureName);
			if(feature == FeatureResult.NotFound)
				continue;

			return feature switch
			{
				FeatureResult.Enabled => true,
				FeatureResult.Disabled => false,
				_ => throw new InvalidOperationException("Unknown feature result")
			};
		}

		return this.defaultNotFoundBehavior switch
		{
			FeatureBehavior.Enabled => true,
			FeatureBehavior.Disabled => false,
			_ => throw new InvalidOperationException("Unknown default behavior")
		};
		
	}
}
