using Binacle.Net.Kernel.Features.Models;

namespace Binacle.Net.Kernel.Features.Providers;

public interface IFeatureProvider
{
	FeatureResult Get(string featureName);
}
