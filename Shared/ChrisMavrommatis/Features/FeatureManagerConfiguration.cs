using ChrisMavrommatis.Features.Models;
using ChrisMavrommatis.Features.Services;

namespace ChrisMavrommatis.Features;

public class FeatureManagerConfiguration
{
	private bool managerCreated;

	public FeatureManagerConfiguration()
	{
		this.ReadFrom = new FeatureSourceConfiguration(this);
	}

	public FeatureSourceConfiguration ReadFrom { get; }

	public FeatureBehavior DefaultNotFoundBehavior { get; set; } = FeatureBehavior.Disabled;

	public IFeatureManager CreateManager()
	{
		if (this.managerCreated)
		{
			throw new InvalidOperationException("CreateManager() was previously called and can only be called once.");
		}

		this.managerCreated = true;
		return new FeatureManager(
			this.ReadFrom.Providers,
			this.DefaultNotFoundBehavior
			);
	}
}
