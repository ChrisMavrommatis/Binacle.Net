using ChrisMavrommatis.Features.Services;

namespace ChrisMavrommatis.Features;

public static class Feature
{
	private static IFeatureManager _manager = FeatureManager.None;

	public static IFeatureManager Manager
	{
		get
		{
			return _manager;
		}
		set
		{
			ArgumentNullException.ThrowIfNull("value");
			_manager = value;
		}
	}
	public static bool IsEnabled(string featureName)
	{
		return Manager.IsEnabled(featureName);
	}

}
