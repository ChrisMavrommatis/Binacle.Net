namespace Binacle.Net.Kernel.Features.Services;

internal class NullFeatureManager : IFeatureManager
{
	public bool IsEnabled(string featureName)
	{
		return false;
	}
}

