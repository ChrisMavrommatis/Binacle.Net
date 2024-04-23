using ChrisMavrommatis.Features.Models;

namespace ChrisMavrommatis.Features.Providers;

public interface IFeatureProvider
{
	FeatureResult Get(string featureName);
}
