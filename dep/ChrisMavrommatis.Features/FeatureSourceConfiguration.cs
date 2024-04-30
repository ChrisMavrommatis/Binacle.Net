using ChrisMavrommatis.Features.Providers;
using ChrisMavrommatis.Features.Providers.Configuration;
using ChrisMavrommatis.Features.Providers.EnvironmentVariable;
using Microsoft.Extensions.Configuration;

namespace ChrisMavrommatis.Features;

public class FeatureSourceConfiguration
{
	private readonly FeatureManagerConfiguration parent;
	private HashSet<Type> providerTypes;
	private List<IFeatureProvider> providers;
	internal IReadOnlyList<IFeatureProvider> Providers => this.providers.AsReadOnly();

	public FeatureSourceConfiguration(FeatureManagerConfiguration parent)
	{
		this.providerTypes = new HashSet<Type>();
		this.providers = new List<IFeatureProvider>();
		this.parent = parent;
	}

	public FeatureManagerConfiguration EnvironmentVariables() 
	{
		return this.Provider<EnvironmentVariableFeatureProvider>();
	}
	public FeatureManagerConfiguration Configuration(IConfiguration configuration)
	{
		var provider = new ConfigurationFeatureProvider(configuration);
		return this.Provider(provider);
	}

	public FeatureManagerConfiguration Provider<TProvider>()
		where TProvider : IFeatureProvider
	{
		var provider = Activator.CreateInstance<TProvider>();
		this.AddProviderInternal(provider);
		return this.parent;
	}
	public FeatureManagerConfiguration Provider<TProvider>(TProvider provider) 
		where TProvider : IFeatureProvider
	{
		this.AddProviderInternal(provider);
		return this.parent;
	}

	private void AddProviderInternal<TProvider>(TProvider provider)
		where TProvider : IFeatureProvider

	{
		if (!this.providerTypes.Add(typeof(TProvider)))
		{
			throw new InvalidOperationException($"Provider of type {typeof(TProvider).Name} has already been added.");
		}

		this.providers.Add(provider);
	}
}
